namespace MuchAdo.Npgsql;

public class NpgsqlDbConnectorSettings : DbConnectorSettings
{
	public NpgsqlDbConnectorSettings()
	{
		SqlSyntax = SqlSyntax.Postgres;
	}

	internal static NpgsqlDbConnectorSettings Default { get; } = new();
}
