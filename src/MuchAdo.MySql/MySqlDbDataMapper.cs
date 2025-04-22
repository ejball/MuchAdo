namespace MuchAdo.MySql;

public static class MySqlDbDataMapper
{
	public static DbDataMapper Default { get; } = new(new DefaultDbTypeMapperFactory(), new MySqlDbTypeMapperFactory());
}
