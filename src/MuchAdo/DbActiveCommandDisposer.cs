namespace MuchAdo;

internal readonly struct DbActiveCommandDisposer(DbConnector? connector) : IDisposable, IAsyncDisposable
{
	public void Dispose() => connector?.DisposeActiveCommand();

	public ValueTask DisposeAsync() => connector?.DisposeActiveCommandAsync() ?? default;
}
