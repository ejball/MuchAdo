namespace MuchAdo.SqlFormatting;

internal sealed class ParamSql<T>(T value) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(this, value, type: null);
}
