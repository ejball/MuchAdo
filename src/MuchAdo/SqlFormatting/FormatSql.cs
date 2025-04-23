namespace MuchAdo.SqlFormatting;

internal sealed class FormatSql(List<object> parts) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
		foreach (var part in parts)
		{
			if (part is string text)
				builder.AppendText(text);
			else
				((Sql) part).Render(builder);
		}
	}
}
