#if false
using FluentAssertions;
using NUnit.Framework;
using static FluentAssertions.FluentActions;

namespace MuchAdo.Tests;

[TestFixture]
internal sealed class DbParameterSourceTests
{
	[Test]
	public void Empty()
	{
		DbParameterSource.Empty.Count().Should().Be(0);
	}

	[Test]
	public void CreateSingle()
	{
		DbParameterSource.Create("one", 1).Enumerate().Should().Equal(("one", 1));
	}

	[Test]
	public void CreateFromPairParams()
	{
		DbParameterSource.Create().Count().Should().Be(0);
		DbParameterSource.Create(("one", 1)).Enumerate().Should().Equal(("one", 1));
		DbParameterSource.Create(("one", 1), ("two", 2L)).Enumerate().Should().Equal(("one", 1L), ("two", 2L));
		DbParameterSource.Create<object>(("one", 1), ("two", 2L)).Enumerate().Should().Equal(("one", 1), ("two", 2L));
		DbParameterSource.Create<object?>(("one", 1), ("null", null)).Enumerate().Should().Equal(("one", 1), ("null", null));
	}

	[Test]
	public void CreateFromPairList()
	{
		DbParameterSource.Create([("one", "1"), ("two", "2")]).Enumerate().Should().Equal(("one", "1"), ("two", "2"));
		DbParameterSource.Create([("one", 1), ("two", 2L)]).Enumerate().Should().Equal(("one", 1L), ("two", 2L));
		var array1 = new (string, object)[] { ("one", 1), ("two", 2L) };
		DbParameterSource.Create(array1).Enumerate().Should().Equal(("one", 1), ("two", 2L));
		var array2 = new (string, object?)[] { ("one", 1), ("two", 2L) };
		DbParameterSource.Create(array2).Enumerate().Should().Equal(("one", 1), ("two", 2L));
		var array3 = new[] { ("one", (object) 1), ("two", 2L) };
		DbParameterSource.Create(array3).Enumerate().Should().Equal(("one", 1), ("two", 2L));
		var array4 = new[] { ("one", (object?) 1), ("two", 2L) };
		DbParameterSource.Create(array4).Enumerate().Should().Equal(("one", 1), ("two", 2L));
	}

	[Test]
	public void CreateFromDictionary()
	{
		DbParameterSource.Create(new Dictionary<string, long> { { "one", 1 }, { "two", 2L } }).Enumerate().Should().Equal(("one", 1L), ("two", 2L));
		DbParameterSource.Create(new Dictionary<string, int?> { { "one", 1 }, { "null", null } }).Enumerate().Should().Equal(("one", 1), ("null", null));
		DbParameterSource.Create(new Dictionary<string, object> { { "one", 1 }, { "two", 2L } }).Enumerate().Should().Equal(("one", 1), ("two", 2L));
		DbParameterSource.Create(new Dictionary<string, object?> { { "one", 1 }, { "null", null } }).Enumerate().Should().Equal(("one", 1), ("null", null));
	}

	[Test]
	public void CreateFromDto()
	{
		var parameters = DbParameterSource.Create(DbParameterSource.FromDto(new { one = 1 }), DbParameterSource.FromDto(new HasTwo()));
		parameters.Count().Should().Be(2);
		parameters.Enumerate().Should().Equal(("one", 1), ("Two", 2));
	}

	[Test]
	public void CreateFromDtoRenamed()
	{
		var parameters = DbParameterSource.FromDto(new { one = 1, Two = 2 }).Renamed(x => $"it's {x}");
		parameters.Count().Should().Be(2);
		parameters.Enumerate().Should().Equal(("it's one", 1), ("it's Two", 2));
	}

	[Test]
	public void CreateFromDtoWhere()
	{
		var parameters = DbParameterSource.FromDto(new { one = 1, two = 2, three = 3 }).Where(x => x[0] == 't');
		parameters.Count().Should().Be(2);
		parameters.Enumerate().Should().Equal(("two", 2), ("three", 3));
	}

	[Test]
	public void CreateFromDtoWhereRenamedWhereRenamed()
	{
		var parameters = DbParameterSource.FromDto(new { one = 1, Two = 2, three = 3 }).Where(x => x[0] == 't').Renamed(x => x.ToUpperInvariant()).Where(x => x[0] == 'T').Renamed(x => x.ToLowerInvariant());
		parameters.Count().Should().Be(1);
		parameters.Enumerate().Should().Equal(("three", 3));

		parameters = DbParameterSource.FromDto(new { one = 10, Two = 20, three = 30 }).Where(x => x[0] == 't').Renamed(x => x.ToUpperInvariant()).Where(x => x[0] == 'T').Renamed(x => x.ToLowerInvariant());
		parameters.Count().Should().Be(1);
		parameters.Enumerate().Should().Equal(("three", 30));
	}

	[Test]
	public void CreateFromDtoNamedWhereNamedWhere()
	{
		var parameters = DbParameterSource.FromDto(new { one = 1, Two = 2, three = 3 }).Renamed(x => x.ToUpperInvariant()).Where(x => x[0] == 'T').Renamed(x => x.ToLowerInvariant()).Where(x => x[0] == 't');
		parameters.Count().Should().Be(2);
		parameters.Enumerate().Should().Equal(("two", 2), ("three", 3));
	}

	[Test]
	public void Count()
	{
		DbParameterSource.Create(("one", 1)).Count().Should().Be(1);
	}

	[Test]
	public void Nulls()
	{
		Invoking(() => DbParameterSource.Create(default(IEnumerable<SqlParamSource>)!)).Should().Throw<ArgumentNullException>();
		Invoking(() => DbParameterSource.Create(default((string, string)[])!)).Should().Throw<ArgumentNullException>();
		Invoking(() => DbParameterSource.Create(default(Dictionary<string, string>)!)).Should().Throw<ArgumentNullException>();
		Invoking(() => DbParameterSource.FromDto(default(object?))).Should().Throw<ArgumentNullException>();
	}

	private sealed class HasTwo
	{
		public int Two { get; } = 2;
	}
}
#endif
