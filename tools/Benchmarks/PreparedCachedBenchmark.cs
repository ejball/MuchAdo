using System.Data;
using System.Data.Common;
using BenchmarkDotNet.Attributes;
using MuchAdo;
#if SQLSERVER
using Microsoft.Data.SqlClient;
#endif
using Microsoft.Data.Sqlite;
using MuchAdo.MySql;
using MuchAdo.Npgsql;
using MuchAdo.Sqlite;
#if MYSQL
using MySqlConnector;
#endif
#if NPGSQL
using Npgsql;
#endif

namespace Benchmarks;

[MemoryDiagnoser]
public class PreparedCachedBenchmark
{
#pragma warning disable SA1114
	[Params(
#if MYSQL
		DbProvider.MySql,
#endif
#if NPGSQL
		DbProvider.Npgsql,
#endif
#if SQLSERVER
		DbProvider.SqlServer,
#endif
		DbProvider.Sqlite)]
	public DbProvider Provider { get; set; }
#pragma warning restore SA1114

	[Params(false, true)]
	public bool Connector { get; set; }

	[Params(false, true)]
	public bool Cached { get; set; }

	[Params(false, true)]
	public bool Prepared { get; set; }

	[Benchmark]
	public void Insert()
	{
		if (Connector)
		{
			var sql = Sql.Format($"""
				insert into PreparedCachedBenchmark (Value)
				values ({Sql.Join(" + ", Enumerable.Range(0, 100).Select(Sql.Param))})
				""");

			for (var recordIndex = 0; recordIndex < m_recordCount; recordIndex++)
			{
				m_connector!
					.Command(sql)
					.Prepare(Prepared)
					.Cache(Cached)
					.Execute();
			}
		}
		else
		{
			var sql = $"""
				insert into PreparedCachedBenchmark (Value)
				values ({string.Join(" + ", Enumerable.Range(0, 100).Select(x => $"@item{x}"))})
				""";
			var parameters = Enumerable.Range(0, 100).Select(x => CreateParameter($"item{x}", x)).ToList();
			var command = m_connector!.Connection.CreateCommand();
			command.CommandText = sql;
			foreach (var parameter in parameters)
				command.Parameters.Add(parameter);
			if (Prepared)
				command.Prepare();

			for (var recordIndex = 0; recordIndex < m_recordCount; recordIndex++)
				command.ExecuteNonQuery();
		}
	}

	[GlobalSetup]
	public void GlobalSetup()
	{
		var columnsSql = Provider switch
		{
			DbProvider.Sqlite => "ItemId integer primary key, Value integer not null",
			DbProvider.MySql => "ItemId int not null auto_increment primary key, Value int not null",
			DbProvider.SqlServer => "ItemId int not null identity primary key, Value int not null",
			DbProvider.Npgsql => "ItemId serial primary key, Value int not null",
			_ => throw new NotSupportedException($"Provider {Provider} is not supported."),
		};

		m_connector = CreateConnector();
		m_connector
			.Command("drop table if exists PreparedCachedBenchmark")
			.Execute();
		m_connector
			.CommandFormat($"create table PreparedCachedBenchmark ({columnsSql:raw})")
			.Execute();

		m_recordCount = Provider switch
		{
			DbProvider.Sqlite => 1000,
			DbProvider.MySql => 100,
			DbProvider.SqlServer => 100,
			DbProvider.Npgsql => 100,
			_ => throw new NotSupportedException($"Provider {Provider} is not supported."),
		};
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		m_connector?.Dispose();
	}

	private IDbConnection CreateConnnection() =>
		Provider switch
		{
			////DbProvider.Sqlite => new SqliteConnection("Data Source=PreparedCachedBenchmark;Mode=Memory;Cache=Shared"),
			DbProvider.Sqlite => new SqliteConnection("Data Source=:memory:"),
			DbProvider.MySql => new MySqlConnection("Server=localhost;User Id=root;Password=test;SSL Mode=none;Database=test;Ignore Prepare=false"),
			DbProvider.SqlServer => new SqlConnection("data source=localhost;user id=sa;password=P@ssw0rd;initial catalog=test;TrustServerCertificate=True"),
			DbProvider.Npgsql => new NpgsqlConnection("host=localhost;user id=root;password=test;database=test"),
			_ => throw new NotSupportedException($"Provider {Provider} is not supported."),
		};

	private DbConnector CreateConnector() => Provider switch
	{
		DbProvider.Sqlite => new SqliteDbConnector((SqliteConnection) CreateConnnection()),
		DbProvider.MySql => new MySqlDbConnector((MySqlConnection) CreateConnnection()),
		DbProvider.Npgsql => new NpgsqlDbConnector((NpgsqlConnection) CreateConnnection()),
		DbProvider.SqlServer => new DbConnector(CreateConnnection()),
		_ => throw new InvalidOperationException(),
	};

	private DbParameter CreateParameter<T>(string name, T value) => Provider switch
	{
		DbProvider.Sqlite => new SqliteParameter(name, value),
		DbProvider.MySql => new MySqlParameter(name, value),
		DbProvider.Npgsql => new NpgsqlParameter<T>(name, value),
		DbProvider.SqlServer => new SqlParameter(name, value),
		_ => throw new InvalidOperationException(),
	};

	public enum DbProvider
	{
		Sqlite,
		MySql,
		Npgsql,
		SqlServer,
	}

	private DbConnector? m_connector;
	private int m_recordCount;
}
