namespace MuchAdo.SqlFormatting;

internal sealed class ConcatSql(IReadOnlyList<SqlSource> sqls) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		foreach (var sql in sqls)
			sql.Render(builder);
	}
}
