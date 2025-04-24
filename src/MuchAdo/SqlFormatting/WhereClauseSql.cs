namespace MuchAdo.SqlFormatting;

internal sealed class WhereClauseSql(SqlSource sql) : OptionalClauseSql(sql)
{
	public override string Lowercase => "where ";

	public override string Uppercase => "WHERE ";
}
