namespace MuchAdo.Sources;

internal sealed class NameSqlSource(string identifier) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendText(builder.Syntax.QuoteName(identifier));
}
