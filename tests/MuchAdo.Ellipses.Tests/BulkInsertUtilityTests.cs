using FluentAssertions;
using Microsoft.Data.Sqlite;
using MuchAdo.SqlFormatting;
using NUnit.Framework;
using static FluentAssertions.FluentActions;

namespace MuchAdo.Ellipses.Tests;

[TestFixture]
internal sealed class BulkInsertUtilityTests
{
	[Test]
	public void BulkInsertTests()
	{
		using var connector = CreateConnector();
		var tableName = Sql.Raw(nameof(BulkInsertTests));
		connector.CommandFormat($"create table {tableName} (ItemId integer primary key, Name text not null);").Execute();
		connector.CommandFormat($"insert into {tableName} (Name) values (@name)...;")
			.BulkInsert(Enumerable.Range(1, 100).Select(x => Sql.NamedParam("name", $"item{x}")));
		connector.CommandFormat($"select count(*) from {tableName};").QuerySingle<long>().Should().Be(100);
	}

	[Test]
	public async Task BulkInsertAsyncTests()
	{
		await using var connector = CreateConnector();
		var tableName = Sql.Raw(nameof(BulkInsertAsyncTests));
		await connector.CommandFormat($"create table {tableName} (ItemId integer primary key, Name text not null);").ExecuteAsync();
		await connector.CommandFormat($"insert into {tableName} (Name) values (@name)...;")
			.BulkInsertAsync(Enumerable.Range(1, 100).Select(x => Sql.NamedParam("name", $"item{x}")));
		(await connector.CommandFormat($"select count(*) from {tableName};").QuerySingleAsync<long>()).Should().Be(100);
	}

	[Test]
	public void EmptySql_Throws()
	{
		Invoking(() => BulkInsertUtility.GetBulkInsertCommands("", SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList())
			.Should().Throw<ArgumentException>();
	}

	[Test]
	public void NoValues_Throws()
	{
		Invoking(() => BulkInsertUtility.GetBulkInsertCommands("VALUE (@foo)...", SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList())
			.Should().Throw<ArgumentException>();
	}

	[Test]
	public void ValuesSuffix_Throws()
	{
		Invoking(() => BulkInsertUtility.GetBulkInsertCommands("1VALUES (@foo)...", SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList())
			.Should().Throw<ArgumentException>();
	}

	[Test]
	public void NoEllipsis_Throws()
	{
		Invoking(() => BulkInsertUtility.GetBulkInsertCommands("VALUE (@foo)..", SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList())
			.Should().Throw<ArgumentException>();
	}

	[Test]
	public void MultipleValues_Throws()
	{
		Invoking(() => BulkInsertUtility.GetBulkInsertCommands("VALUES (@foo)... VALUES (@foo)...", SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList())
			.Should().Throw<ArgumentException>();
	}

	[Test]
	public void ZeroBatchSize_Throws()
	{
		Invoking(() => BulkInsertUtility.GetBulkInsertCommands("VALUES (@foo)...",
				SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })], new BulkInsertSettings { MaxRowsPerBatch = 0 }).ToList())
			.Should().Throw<ArgumentException>();
	}

	[Test]
	public void NegativeBatchSize_Throws()
	{
		Invoking(() => BulkInsertUtility.GetBulkInsertCommands("VALUES (@foo)...",
				SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })], new BulkInsertSettings { MaxRowsPerBatch = -1 }).ToList())
			.Should().Throw<ArgumentException>();
	}

	[Test]
	public void MinimalInsert()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("INSERT INTO t (foo)VALUES(@foo)...;",
			SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("INSERT INTO t (foo)VALUES(@foo_0);");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "foo_0", Value: 1));
	}

	[Test]
	public void InsertNotRequired()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("VALUES (@foo)...",
			SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("VALUES (@foo_0)");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "foo_0", Value: 1));
	}

	[Test]
	public void MultipleInserts()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("INSERT INTO t VALUES (@t); INSERT INTO u VALUES (@u)...; INSERT INTO v VALUES (@v);",
			SqlParamSource.Empty, [Sql.NamedDtoParams(new { t = 1, u = 2, v = 3 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("INSERT INTO t VALUES (@t); INSERT INTO u VALUES (@u_0); INSERT INTO v VALUES (@v);");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "u_0", Value: 2));
	}

	[Test]
	public void CommonAndInsertedParameters()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("VALUES (@a, @b, @c, @d)...",
			Sql.NamedDtoParams(new { a = 1, b = 2 }), [Sql.NamedDtoParams(new { c = 3, d = 4 }), Sql.NamedDtoParams(new { c = 5, d = 6 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("VALUES (@a, @b, @c_0, @d_0), (@a, @b, @c_1, @d_1)");
		EnumerateTuples(commands[0].Parameters).Should().Equal(
			(Name: "a", Value: 1),
			(Name: "b", Value: 2),
			(Name: "c_0", Value: 3),
			(Name: "d_0", Value: 4),
			(Name: "c_1", Value: 5),
			(Name: "d_1", Value: 6));
	}

	[TestCase(3, null)]
	[TestCase(null, 6)]
	[TestCase(3, 10)]
	[TestCase(10, 6)]
	public void EightRowsInThreeBatches(int? maxRecordsPerBatch, int? maxParametersPerBatch)
	{
		var settings = new BulkInsertSettings
		{
			MaxRowsPerBatch = maxRecordsPerBatch,
			MaxParametersPerBatch = maxParametersPerBatch,
		};
		var commands = BulkInsertUtility.GetBulkInsertCommands("VALUES(@foo,@bar)...",
			SqlParamSource.Empty, Enumerable.Range(0, 8).Select(x => Sql.NamedDtoParams(new { foo = x, bar = x * 2 })), settings).ToList();
		commands.Count.Should().Be(3);
		commands[0].Sql.Should().Be("VALUES(@foo_0,@bar_0), (@foo_1,@bar_1), (@foo_2,@bar_2)");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "foo_0", Value: 0), (Name: "bar_0", Value: 0), (Name: "foo_1", Value: 1), (Name: "bar_1", Value: 2), (Name: "foo_2", Value: 2), (Name: "bar_2", Value: 4));
		commands[1].Sql.Should().Be("VALUES(@foo_0,@bar_0), (@foo_1,@bar_1), (@foo_2,@bar_2)");
		EnumerateTuples(commands[1].Parameters).Should().Equal((Name: "foo_0", Value: 3), (Name: "bar_0", Value: 6), (Name: "foo_1", Value: 4), (Name: "bar_1", Value: 8), (Name: "foo_2", Value: 5), (Name: "bar_2", Value: 10));
		commands[2].Sql.Should().Be("VALUES(@foo_0,@bar_0), (@foo_1,@bar_1)");
		EnumerateTuples(commands[2].Parameters).Should().Equal((Name: "foo_0", Value: 6), (Name: "bar_0", Value: 12), (Name: "foo_1", Value: 7), (Name: "bar_1", Value: 14));
	}

	[Test]
	public void CaseInsensitiveValues()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("VaLueS(@foo)...",
			SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("VaLueS(@foo_0)");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "foo_0", Value: 1));
	}

	[Test]
	public void CaseInsensitiveNames()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("values (@foo, @Bar, @BAZ, @bam)...",
			SqlParamSource.Empty, [Sql.NamedDtoParams(new { Foo = 1, BAR = 2, baz = 3 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("values (@foo_0, @Bar_0, @BAZ_0, @bam)");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "Foo_0", Value: 1), (Name: "BAR_0", Value: 2), (Name: "baz_0", Value: 3));
	}

	[Test]
	public void PunctuatedNames()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("values (@foo, @bar)...",
			SqlParamSource.Empty, [new SqlParamSources(Sql.NamedParam("@foo", 1), Sql.NamedParam("@Bar", 2))]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("values (@foo_0, @bar_0)");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "@foo_0", Value: 1), (Name: "@Bar_0", Value: 2));
	}

	[Test]
	public void SubstringNames()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("values (@a, @aa, @aaa, @aaaa)...",
			Sql.NamedDtoParams(new { a = 1, aaa = 3 }), [Sql.NamedDtoParams(new { aa = 2, aaaa = 4 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("values (@a, @aa_0, @aaa, @aaaa_0)");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "a", Value: 1), (Name: "aaa", Value: 3), (Name: "aa_0", Value: 2), (Name: "aaaa_0", Value: 4));
	}

	[Test]
	public void WhitespaceEverywhere()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("\r\n\t VALUES\n\t \r(\t \r\n@foo \r\n\t)\r\n\t ...\t\r\n",
			SqlParamSource.Empty, [Sql.NamedDtoParams(new { foo = 1 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("\r\n\t VALUES\n\t \r(\t \r\n@foo_0 \r\n\t)\t\r\n");
		EnumerateTuples(commands[0].Parameters).Should().Equal((Name: "foo_0", Value: 1));
	}

	[Test]
	public void NothingToInsert()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("VALUES(@foo)...", SqlParamSource.Empty, []).ToList();
		commands.Count.Should().Be(0);
	}

	[Test]
	public void NoParameterNameValidation()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("VALUES (@a, @b, @c, @d)...",
			Sql.NamedDtoParams(new { e = 1, f = 2 }), [Sql.NamedDtoParams(new { g = 3, h = 4 }), Sql.NamedDtoParams(new { g = 5, h = 6 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("VALUES (@a, @b, @c, @d), (@a, @b, @c, @d)");
		EnumerateTuples(commands[0].Parameters).Should().Equal(
			(Name: "e", Value: 1),
			(Name: "f", Value: 2));
	}

	[Test]
	public void ComplexValues()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("VALUES (@a + (@d * @c) -\r\n\t@d)...",
			Sql.NamedDtoParams(new { a = 1, b = 2 }), [Sql.NamedDtoParams(new { c = 3, d = 4 }), Sql.NamedDtoParams(new { c = 5, d = 6 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("VALUES (@a + (@d_0 * @c_0) -\r\n\t@d_0), (@a + (@d_1 * @c_1) -\r\n\t@d_1)");
		EnumerateTuples(commands[0].Parameters).Should().Equal(
			(Name: "a", Value: 1),
			(Name: "b", Value: 2),
			(Name: "c_0", Value: 3),
			(Name: "d_0", Value: 4),
			(Name: "c_1", Value: 5),
			(Name: "d_1", Value: 6));
	}

	[Test]
	public void DifferentParameters()
	{
		var commands = BulkInsertUtility.GetBulkInsertCommands("VALUES (@a, @b)...",
			Sql.NamedDtoParams(new { a = 1, b = 2 }),
			[Sql.NamedDtoParams(new { b = 4 }), SqlParamSource.Empty, Sql.NamedDtoParams(new { a = 3 })]).ToList();
		commands.Count.Should().Be(1);
		commands[0].Sql.Should().Be("VALUES (@a, @b_0), (@a, @b), (@a_2, @b)");
		EnumerateTuples(commands[0].Parameters).Should().Equal(
			(Name: "a", Value: 1),
			(Name: "b", Value: 2),
			(Name: "b_0", Value: 4),
			(Name: "a_2", Value: 3));
	}

	private static IEnumerable<(string Name, object? Value)> EnumerateTuples(SqlParamSource parameters) =>
		parameters.Enumerate().Select(x => (x.Name, x.Value));

	private static DbConnector CreateConnector() => new(new SqliteConnection("Data Source=:memory:"));
}
