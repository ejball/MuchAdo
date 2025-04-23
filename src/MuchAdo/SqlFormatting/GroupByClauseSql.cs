namespace MuchAdo.SqlFormatting;

internal sealed class GroupByClauseSql(Sql sql) : OptionalClauseSql(sql)
{
	public override string Lowercase => "group by ";

	public override string Uppercase => "GROUP BY ";
}
