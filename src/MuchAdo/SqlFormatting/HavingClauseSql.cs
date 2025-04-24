namespace MuchAdo.SqlFormatting;

internal sealed class HavingClauseSql(SqlSource sql) : OptionalClauseSql(sql)
{
	public override string Lowercase => "having ";

	public override string Uppercase => "HAVING ";
}
