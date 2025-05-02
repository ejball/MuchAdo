namespace MuchAdo;

public class SqlParam<T> : SqlParamSource
{
	public T Value { get; set; }

	public virtual string Name => "";

	public virtual SqlParamType? Type => null;

	internal SqlParam(T value) => Value = value;

	internal virtual bool IsReused => false;

	internal override void SubmitParameters(ISqlParamTarget target) => target.AcceptParameter(Name, Value, Type);

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		if (string.IsNullOrEmpty(Name))
		{
			builder.AppendParameterValue(Value, Type, identity: IsReused ? this : null);
		}
		else
		{
			builder.AppendText(builder.Syntax.NamedParameterPrefix);
			builder.AppendText(Name);
			builder.SubmitParameters(this);
		}
	}
}
