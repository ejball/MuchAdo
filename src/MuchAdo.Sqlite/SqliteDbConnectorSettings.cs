namespace MuchAdo.Sqlite;

public class SqliteDbConnectorSettings : DbConnectorSettings
{
	public SqliteDbConnectorSettings()
	{
		SqlSyntax = SqlSyntax.Sqlite;
	}

	internal static SqliteDbConnectorSettings Default { get; } = new();
}
