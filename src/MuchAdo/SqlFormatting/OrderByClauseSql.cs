namespace MuchAdo.SqlFormatting;

internal sealed class OrderByClauseSql(SqlSource sql) : OptionalClauseSql(sql)
{
	public override string Lowercase => "order by ";

	public override string Uppercase => "ORDER BY ";
}
