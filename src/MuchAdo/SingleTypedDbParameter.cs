using MuchAdo.SqlFormatting;

namespace MuchAdo;

public sealed class SingleTypedDbParameter<T>(string name, T value, IDbParameterType? type) : Sql, IDbParameterSource
{
	public new string Name { get; set; } = name;

	public T Value { get; set; } = value;

	public IDbParameterType? Type { get; set; } = type;

	public void SubmitParameters(IDbParameterTarget target) => target.AcceptParameter(Name, Value, Type);

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		builder.AppendText(builder.Syntax.NamedParameterPrefix);
		builder.AppendText(Name);
		builder.SubmitParameters(this);
	}
}
