namespace MuchAdo.Sources;

internal sealed class AndOperatorSqlSource(IReadOnlyList<SqlSource> sqls) : BinaryOperatorSqlSource(sqls)
{
	public override string Lowercase => " and ";

	public override string Uppercase => " AND ";
}
