namespace MuchAdo.SqlFormatting;

internal sealed class AddSql(SqlSource a, SqlSource b) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		a.Render(builder);
		b.Render(builder);
	}
}
