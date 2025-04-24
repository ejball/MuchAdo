namespace MuchAdo.Sources;

internal sealed class LikeParamStartsWithSqlSource(string prefix) : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(this, builder.Syntax.EscapeLikeFragment(prefix) + "%");
}
