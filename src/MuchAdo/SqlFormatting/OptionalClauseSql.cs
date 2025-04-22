namespace MuchAdo.SqlFormatting;

internal sealed class OptionalClauseSql(string lowercase, string uppercase, Sql sql) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		using var scope = builder.Prefix(builder.Syntax.LowercaseKeywords ? lowercase : uppercase);
		sql.Render(builder);
	}
}
