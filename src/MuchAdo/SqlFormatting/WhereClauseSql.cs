namespace MuchAdo.SqlFormatting;

internal sealed class WhereClauseSql(Sql sql) : OptionalClauseSql(sql)
{
	public override string Lowercase => "where ";

	public override string Uppercase => "WHERE ";
}
