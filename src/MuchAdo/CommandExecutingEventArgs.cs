namespace MuchAdo;

public sealed class CommandExecutingEventArgs(DbConnectorCommandBatch connectorCommandBatch) : EventArgs
{
	public DbConnectorCommandBatch ConnectorCommandBatch { get; } = connectorCommandBatch;
}
