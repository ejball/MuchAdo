namespace MuchAdo.SqlFormatting;

internal sealed class LikeParamStartsWithSql(string prefix) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(this, builder.Syntax.EscapeLikeFragment(prefix) + "%");
}
