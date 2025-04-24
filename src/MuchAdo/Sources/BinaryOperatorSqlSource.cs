namespace MuchAdo.Sources;

internal abstract class BinaryOperatorSqlSource(IReadOnlyList<SqlSource> sqls) : SqlSource
{
	public abstract string Lowercase { get; }

	public abstract string Uppercase { get; }

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		if (sqls.Count == 0)
			return;

		if (sqls.Count == 1)
		{
			sqls[0].Render(builder);
			return;
		}

		var oldTextLength = builder.TextLength;
		using var outerScope = builder.Bracket("(", ")");

		foreach (var sql in sqls)
		{
			using var innerScope = builder.Prefix(builder.TextLength != oldTextLength ? builder.Syntax.LowercaseKeywords ? Lowercase : Uppercase : "");
			sql.Render(builder);
		}
	}
}
