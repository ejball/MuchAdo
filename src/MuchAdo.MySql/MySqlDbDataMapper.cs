namespace MuchAdo.MySql;

public static class MySqlDbDataMapper
{
	public static DbDataMapper Default { get; } = new(DbDataMapperSettings.Default.WithMySqlTypeMapperFactory());

	public static DbDataMapperSettings WithMySqlTypeMapperFactory(this DbDataMapperSettings settings) =>
		settings.WithTypeMapperFactory(new MySqlDbTypeMapperFactory());
}
