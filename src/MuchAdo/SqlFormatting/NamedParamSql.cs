namespace MuchAdo.SqlFormatting;

public sealed class NamedParamSql<T> : Sql, IDbParameterSource
{
	public new string Name { get; set; }

	public T Value { get; set; }

	public void SubmitParameters(IDbParameterTarget target) => target.AcceptParameter(Name, Value, type: null);

	internal NamedParamSql(string name, T value)
	{
		Name = name;
		Value = value;
	}

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		builder.AppendText(builder.Syntax.NamedParameterPrefix);
		builder.AppendText(Name);
		builder.SubmitParameters(this);
	}
}
