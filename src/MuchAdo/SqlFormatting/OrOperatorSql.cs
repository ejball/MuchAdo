namespace MuchAdo.SqlFormatting;

internal sealed class OrOperatorSql(IReadOnlyList<Sql> sqls) : BinaryOperatorSql(sqls)
{
	public override string Lowercase => " or ";

	public override string Uppercase => " OR ";
}
