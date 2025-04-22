namespace MuchAdo.SqlFormatting;

internal sealed class NamedTypedParamSql<T>(string name, T value, IDbParameterType? type) : Sql, IDbParameterSource
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		builder.AppendText(builder.Syntax.NamedParameterPrefix);
		builder.AppendText(name);
		builder.SubmitParameters(this);
	}

	public void SubmitParameters(IDbParameterTarget target) => target.AcceptParameter(name, value, type);
}
