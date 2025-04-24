namespace MuchAdo.SqlFormatting;

internal sealed class RawSql(string text) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendText(text);
}
