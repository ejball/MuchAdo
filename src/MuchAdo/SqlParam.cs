namespace MuchAdo;

public class SqlParam<T> : SqlParamSource
{
	public T Value { get; set; }

	public new virtual string Name => "";

	public virtual SqlParamType? Type => null;

	internal override void Submit(ISqlParamTarget target) => target.AcceptParameter(Name, Value, Type);

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		if (string.IsNullOrEmpty(Name))
		{
			builder.AppendParameterValue(this, Value, Type);
		}
		else
		{
			builder.AppendText(builder.Syntax.NamedParameterPrefix);
			builder.AppendText(Name);
			builder.SubmitParameters(this);
		}
	}

	internal SqlParam(T value) => Value = value;
}
