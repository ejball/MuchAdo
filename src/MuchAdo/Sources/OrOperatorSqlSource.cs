namespace MuchAdo.Sources;

internal sealed class OrOperatorSqlSource(IReadOnlyList<SqlSource> sqls) : BinaryOperatorSqlSource(sqls)
{
	public override string Lowercase => " or ";

	public override string Uppercase => " OR ";
}
