namespace MuchAdo.SqlFormatting;

internal sealed class AndOperatorSql(IReadOnlyList<SqlSource> sqls) : BinaryOperatorSql(sqls)
{
	public override string Lowercase => " and ";

	public override string Uppercase => " AND ";
}
