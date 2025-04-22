namespace MuchAdo.SqlFormatting;

internal sealed class LikeParamStartsWithSql(string prefix) : Sql
{
	internal override void Render(DbConnectorCommandBuilder builder) => builder.AppendParameterValue(this, builder.Syntax.EscapeLikeFragment(prefix) + "%", type: null);
}
