namespace MuchAdo.Sources;

internal sealed class ClausesSqlSource(IEnumerable<SqlSource> sqls) : JoinSqlSource(sqls)
{
	public override string Separator => "\n";
}
