using MuchAdo.SqlFormatting;

namespace MuchAdo.MySql;

public class MySqlDbConnectorSettings : DbConnectorSettings
{
	public MySqlDbConnectorSettings()
	{
		SqlSyntax = SqlSyntax.MySql;
		DataMapper = MySqlDbDataMapper.Default;
	}

	internal static MySqlDbConnectorSettings Default { get; } = new();
}
