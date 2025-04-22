namespace MuchAdo.SqlFormatting;

internal sealed class AddSql(Sql a, Sql b) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		a.Render(builder);
		b.Render(builder);
	}
}
