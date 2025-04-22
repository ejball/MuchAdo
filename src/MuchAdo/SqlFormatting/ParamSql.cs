namespace MuchAdo.SqlFormatting;

internal sealed class ParamSql<T>(T value, IDbParameterType? type) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(this, value, type);
}
