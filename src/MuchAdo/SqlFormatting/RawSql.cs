namespace MuchAdo.SqlFormatting;

internal sealed class RawSql(string text) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendText(text);
}
