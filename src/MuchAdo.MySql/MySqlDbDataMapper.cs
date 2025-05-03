namespace MuchAdo.MySql;

public static class MySqlDbDataMapper
{
	public static DbDataMapper Default { get; } = DbDataMapper.Default.WithMySqlTypeMapperFactory();

	public static DbDataMapper WithMySqlTypeMapperFactory(this DbDataMapper dataMapper) =>
		dataMapper.WithTypeMapperFactories([.. dataMapper.TypeMapperFactories, new MySqlDbTypeMapperFactory()]);
}
