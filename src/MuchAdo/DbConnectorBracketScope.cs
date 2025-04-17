using MuchAdo.SqlFormatting;

namespace MuchAdo;

internal readonly struct DbConnectorBracketScope(SqlCommandBuilder commandBuilder) : IDisposable
{
	public void Dispose() => commandBuilder.EndBracket();
}
