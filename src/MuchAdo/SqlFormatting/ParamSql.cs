namespace MuchAdo.SqlFormatting;

public sealed class ParamSql<T> : Sql
{
	public T Value { get; set; }

	internal ParamSql(T value)
	{
		Value = value;
	}

	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(this, Value);
}
