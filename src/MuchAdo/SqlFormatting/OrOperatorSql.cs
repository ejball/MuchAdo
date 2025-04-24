namespace MuchAdo.SqlFormatting;

internal sealed class OrOperatorSql(IReadOnlyList<SqlSource> sqls) : BinaryOperatorSql(sqls)
{
	public override string Lowercase => " or ";

	public override string Uppercase => " OR ";
}
