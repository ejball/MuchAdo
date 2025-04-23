namespace MuchAdo.SqlFormatting;

public sealed class NamedTypedParamSql<T> : Sql, IDbParameterSource
{
	public new string Name { get; set; }

	public T Value { get; set; }

	public IDbParameterType? Type { get; set; }

	public void SubmitParameters(IDbParameterTarget target) => target.AcceptParameter(Name, Value, Type);

	internal NamedTypedParamSql(string name, T value, IDbParameterType? type)
	{
		Name = name;
		Value = value;
		Type = type;
	}

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		builder.AppendText(builder.Syntax.NamedParameterPrefix);
		builder.AppendText(Name);
		builder.SubmitParameters(this);
	}
}
