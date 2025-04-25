namespace MuchAdo.Sources;

internal abstract class JoinSqlSource(IEnumerable<SqlSource> sqls) : SqlSource
{
	public abstract string Separator { get; }

	public virtual string ThrowMessageIfEmpty => "";

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		var oldTextLength = builder.TextLength;

		foreach (var sql in sqls)
		{
			using var scope = builder.Prefix(builder.TextLength != oldTextLength ? Separator : "");
			sql.Render(builder);
		}

		if (ThrowMessageIfEmpty.Length != 0 && builder.TextLength == oldTextLength)
			throw new InvalidOperationException(ThrowMessageIfEmpty);
	}
}
