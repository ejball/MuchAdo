namespace MuchAdo;

public sealed class DbConnectorExecutingEventArgs(DbConnectorCommandBatch commandBatch) : EventArgs
{
	public DbConnectorCommandBatch CommandBatch { get; } = commandBatch;
}
