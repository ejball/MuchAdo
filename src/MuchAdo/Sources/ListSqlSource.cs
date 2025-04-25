namespace MuchAdo.Sources;

internal sealed class ListSqlSource(IEnumerable<SqlSource> sqls) : JoinSqlSource(sqls)
{
	public override string Separator => ", ";

	public override string ThrowMessageIfEmpty => "Sql.List was empty.";
}
