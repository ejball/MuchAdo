using MuchAdo.SqlFormatting;

namespace MuchAdo;

internal readonly struct DbConnectorBracketScope(DbConnectorQueryBuilder commandBuilder) : IDisposable
{
	public void Dispose() => commandBuilder.EndBracket();
}
