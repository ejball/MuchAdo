namespace MuchAdo.SqlFormatting;

internal sealed class JoinSql(string separator, IReadOnlyList<SqlSource> sqls, string? throwMessageIfEmpty = null) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		var oldTextLength = builder.TextLength;

		foreach (var sql in sqls)
		{
			using var scope = builder.Prefix(builder.TextLength != oldTextLength ? separator : "");
			sql.Render(builder);
		}

		if (throwMessageIfEmpty is not null && builder.TextLength == oldTextLength)
			throw new InvalidOperationException(throwMessageIfEmpty);
	}
}
