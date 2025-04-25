namespace MuchAdo.Sources;

internal sealed class EmptySqlSource : SqlSource
{
	internal override void Render(DbConnectorCommandBuilder builder)
	{
	}
}
