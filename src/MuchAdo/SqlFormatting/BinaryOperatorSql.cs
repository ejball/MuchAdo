namespace MuchAdo.SqlFormatting;

internal sealed class BinaryOperatorSql(string lowercase, string uppercase, IReadOnlyList<Sql> sqls) : Sql
{
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
			using var innerScope = builder.Prefix(builder.TextLength != oldTextLength ? (builder.Syntax.LowercaseKeywords ? lowercase : uppercase) : "");
			sql.Render(builder);
		}
	}
}
