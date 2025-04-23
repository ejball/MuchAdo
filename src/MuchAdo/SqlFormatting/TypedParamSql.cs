namespace MuchAdo.SqlFormatting;

public sealed class TypedParamSql<T> : Sql
{
	public T Value { get; set; }

	public IDbParameterType? Type { get; set; }

	internal TypedParamSql(T value, IDbParameterType? type)
	{
		Value = value;
		Type = type;
	}

	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(this, Value, Type);
}
