namespace MuchAdo.Sources;

internal sealed class ListOrEmptySqlSource(IEnumerable<SqlSource> sqls) : JoinSqlSource(sqls)
{
	public override string Separator => ", ";
}
