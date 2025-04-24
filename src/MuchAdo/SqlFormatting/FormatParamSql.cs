namespace MuchAdo.SqlFormatting;

internal sealed class FormatParamSql<T>(T value) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(identity: null, value);
}
