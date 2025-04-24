namespace MuchAdo;

public class SqlParam<T> : SqlParamSource
{
	public T Value { get; set; }

	public new virtual string Name => "";

	public virtual SqlParamType? Type => null;

#pragma warning disable CA2225
	public static implicit operator SqlParam<T>((string Name, T Value) tuple) => NamedParam(tuple.Name, tuple.Value);
#pragma warning restore CA2225

	internal override void SubmitParameters(ISqlParamTarget target) => target.AcceptParameter(Name, Value, Type);

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
