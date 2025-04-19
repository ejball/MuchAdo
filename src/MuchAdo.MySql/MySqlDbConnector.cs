using System.Data;
using MySqlConnector;

namespace MuchAdo.MySql;

public class MySqlDbConnector : DbConnector
{
	public MySqlDbConnector(MySqlConnection connection)
		: this(connection, MySqlDbConnectorSettings.Default)
	{
	}

	public MySqlDbConnector(MySqlConnection connection, MySqlDbConnectorSettings settings)
		: base(connection, settings)
	{
	}

	public new MySqlConnection Connection => (MySqlConnection) base.Connection;

	public new MySqlTransaction? Transaction => (MySqlTransaction?) base.Transaction;

	public new MySqlCommand? ActiveCommand => (MySqlCommand?) base.ActiveCommand;

#if NETSTANDARD2_0
	public MySqlBatch? ActiveBatch => ActiveCommandOrBatch as MySqlBatch;
#else
	public new MySqlBatch? ActiveBatch => ActiveCommandOrBatch as MySqlBatch;
#endif

	public new MySqlDataReader? ActiveReader => (MySqlDataReader?) base.ActiveReader;

	public new MySqlConnection GetOpenConnection() => (MySqlConnection) base.GetOpenConnection();

	public new ValueTask<MySqlConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
	{
		var task = base.GetOpenConnectionAsync(cancellationToken);
		return task.IsCompletedSuccessfully ? new ValueTask<MySqlConnection>((MySqlConnection) task.Result) : DoAsync(task);
		static async ValueTask<MySqlConnection> DoAsync(ValueTask<IDbConnection> t) => (MySqlConnection) await t.ConfigureAwait(false);
	}

#if NETSTANDARD2_0
	protected override ValueTask CloseConnectionCoreAsync() => new(Connection.CloseAsync());

	protected override ValueTask DisposeConnectionCoreAsync() => new(Connection.DisposeAsync());

	protected override async ValueTask<IDbTransaction> BeginTransactionCoreAsync(CancellationToken cancellationToken) =>
		await Connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

	protected override async ValueTask<IDbTransaction> BeginTransactionCoreAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken) =>
		await Connection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

	protected override ValueTask CommitTransactionCoreAsync(CancellationToken cancellationToken) => new(Transaction!.CommitAsync(cancellationToken));

	protected override ValueTask RollbackTransactionCoreAsync(CancellationToken cancellationToken) => new(Transaction!.RollbackAsync(cancellationToken));

	protected override ValueTask DisposeTransactionCoreAsync() => new(Transaction!.DisposeAsync());

	protected override int ExecuteNonQueryCore()
	{
		if (ActiveBatch is { } batch)
			return batch.ExecuteNonQuery();
		else
			return base.ExecuteNonQueryCore();
	}

	protected override ValueTask<int> ExecuteNonQueryCoreAsync(CancellationToken cancellationToken)
	{
		if (ActiveBatch is { } batch)
			return new ValueTask<int>(batch.ExecuteNonQueryAsync(cancellationToken));
		else
			return base.ExecuteNonQueryCoreAsync(cancellationToken);
	}

	protected override IDataReader ExecuteReaderCore()
	{
		if (ActiveBatch is { } batch)
			return batch.ExecuteReader();
		else
			return base.ExecuteReaderCore();
	}

	protected override async ValueTask<IDataReader> ExecuteReaderCoreAsync(CancellationToken cancellationToken)
	{
		if (ActiveBatch is { } batch)
			return await batch.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
		else
			return await base.ExecuteReaderCoreAsync(cancellationToken).ConfigureAwait(false);
	}

	protected override IDataReader ExecuteReaderCore(CommandBehavior commandBehavior)
	{
		if (ActiveBatch is { } batch)
			return batch.ExecuteReader(commandBehavior);
		else
			return base.ExecuteReaderCore(commandBehavior);
	}

	protected override async ValueTask<IDataReader> ExecuteReaderCoreAsync(CommandBehavior commandBehavior, CancellationToken cancellationToken)
	{
		if (ActiveBatch is { } batch)
			return await batch.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
		else
			return await base.ExecuteReaderCoreAsync(commandBehavior, cancellationToken).ConfigureAwait(false);
	}

	protected override void PrepareCore()
	{
		if (ActiveBatch is { } batch)
			batch.Prepare();
		else
			base.PrepareCore();
	}

	protected override ValueTask PrepareCoreAsync(CancellationToken cancellationToken)
	{
		if (ActiveBatch is { } batch)
			return new ValueTask(batch.PrepareAsync(cancellationToken));
		else
			return base.PrepareCoreAsync(cancellationToken);
	}

	protected override void DisposeCommandOrBatchCore()
	{
		if (ActiveBatch is { } batch)
			batch.Dispose();
		else
			base.DisposeCommandOrBatchCore();
	}

	protected override ValueTask DisposeCommandOrBatchCoreAsync()
	{
		if (ActiveCommand is { } command)
			return new ValueTask(command.DisposeAsync());
		else
			return base.DisposeCommandOrBatchCoreAsync();
	}

	protected override object CreateBatchCore() => Connection.CreateBatch();

	protected override void AddBatchCommandCore(object batch, CommandType commandType, string commandText)
	{
		if (batch is MySqlBatch dbBatch)
		{
			var command = new MySqlBatchCommand();

			if (commandType != CommandType.Text)
				command.CommandType = commandType;

			command.CommandText = commandText;

			dbBatch.BatchCommands.Add(command);

			return;
		}

		base.AddBatchCommandCore(batch, commandType, commandText);
	}

	protected override void SetTimeoutCore(int timeout)
	{
		if (ActiveCommandOrBatch is MySqlBatch dbBatch)
		{
			dbBatch.Timeout = timeout;
			return;
		}

		base.SetTimeoutCore(timeout);
	}

	protected override void SetTransactionCore(IDbTransaction? transaction)
	{
		if (ActiveCommandOrBatch is MySqlBatch dbBatch && transaction is MySqlTransaction dbTransaction)
		{
			dbBatch.Transaction = dbTransaction;
			return;
		}

		base.SetTransactionCore(transaction);
	}

	protected override IDataParameterCollection GetParameterCollectionCore(int commandIndex)
	{
		if (ActiveCommandOrBatch is MySqlBatch dbBatch)
			return dbBatch.BatchCommands[commandIndex].Parameters;

		return base.GetParameterCollectionCore(commandIndex);
	}

	protected override IDataParameter CreateParameterCore<T>(string name, T value)
	{
		var parameter = new MySqlParameter();
		if (name.Length != 0)
			parameter.ParameterName = name;
		parameter.Value = value is null ? DBNull.Value : value;
		return parameter;
	}

	protected override ValueTask DisposeReaderCoreAsync() => new(ActiveReader!.DisposeAsync());
#endif
}
