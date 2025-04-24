namespace MuchAdo.SqlFormatting;

internal sealed class NameSql(string identifier) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendText(builder.Syntax.QuoteName(identifier));
}
