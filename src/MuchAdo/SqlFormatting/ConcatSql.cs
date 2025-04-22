namespace MuchAdo.SqlFormatting;

internal sealed class ConcatSql(IReadOnlyList<Sql> sqls) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		foreach (var sql in sqls)
			sql.Render(builder);
	}
}
