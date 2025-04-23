using MuchAdo.SqlFormatting;

namespace MuchAdo;

public sealed class SingleDbParameter<T>(string name, T value) : Sql, IDbParameterSource
{
	public new string Name { get; set; } = name;

	public T Value { get; set; } = value;

	public void SubmitParameters(IDbParameterTarget target) => target.AcceptParameter(Name, Value, type: null);

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		builder.AppendText(builder.Syntax.NamedParameterPrefix);
		builder.AppendText(Name);
		builder.SubmitParameters(this);
	}
}
