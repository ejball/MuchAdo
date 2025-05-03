namespace MuchAdo.MySql;

public static class MySqlDbDataMapper
{
	public static DbDataMapper Default { get; } = new(DbDataMapper.Default.Settings.WithMySqlTypeMapperFactory());

	public static DbDataMapperSettings WithMySqlTypeMapperFactory(this DbDataMapperSettings settings) =>
		settings.WithTypeMapperFactory(new MySqlDbTypeMapperFactory());
}
