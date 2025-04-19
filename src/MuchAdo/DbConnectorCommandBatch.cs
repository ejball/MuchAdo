using System.Data;
using MuchAdo.SqlFormatting;

namespace MuchAdo;

/// <summary>
/// Encapsulates a database command, or a batch of database commands.
/// </summary>
public sealed class DbConnectorCommandBatch
{
	/// <summary>
	/// The timeout.
	/// </summary>
	/// <remarks>If not specified, the default timeout for the connection is used.</remarks>
	public TimeSpan? Timeout { get; private set; }

	/// <summary>
	/// True after <see cref="Cache"/> is called.
	/// </summary>
	public bool IsCached { get; private set; }

	/// <summary>
	/// True after <see cref="Prepare"/> is called.
	/// </summary>
	public bool IsPrepared { get; private set; }

	/// <summary>
	/// The connector.
	/// </summary>
	public DbConnector Connector { get; }

	/// <summary>
	/// The number of queries in the command batch.
	/// </summary>
	public int CommandCount => 1 + (m_batchedCommands?.Count ?? 0);

	/// <summary>
	/// Executes the command, returning the number of rows affected.
	/// </summary>
	/// <seealso cref="ExecuteAsync" />
	public int Execute() =>
		Connector.ExecuteCommand(this);

	/// <summary>
	/// Executes the command, returning the number of rows affected.
	/// </summary>
	/// <seealso cref="Execute" />
	public ValueTask<int> ExecuteAsync(CancellationToken cancellationToken = default) =>
		Connector.ExecuteCommandAsync(this, cancellationToken);

	/// <summary>
	/// Executes the query, reading every record and converting it to the specified type.
	/// </summary>
	/// <seealso cref="QueryAsync{T}(CancellationToken)" />
	public IReadOnlyList<T> Query<T>() =>
		Connector.Query<T>(this, map: null);

	/// <summary>
	/// Executes the query, reading every record and converting it to the specified type with the specified delegate.
	/// </summary>
	/// <seealso cref="QueryAsync{T}(Func{DbConnectorRecord, T}, CancellationToken)" />
	public IReadOnlyList<T> Query<T>(Func<DbConnectorRecord, T> map) =>
		Connector.Query(this, map ?? throw new ArgumentNullException(nameof(map)));

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned.</remarks>
	/// <seealso cref="QueryFirstAsync{T}(CancellationToken)" />
	public T QueryFirst<T>() =>
		Connector.QueryFirst<T>(this, map: null, single: false, orDefault: false);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned.</remarks>
	/// <seealso cref="QueryFirstAsync{T}(Func{DbConnectorRecord, T}, CancellationToken)" />
	public T QueryFirst<T>(Func<DbConnectorRecord, T> map) =>
		Connector.QueryFirst(this, map ?? throw new ArgumentNullException(nameof(map)), single: false, orDefault: false);

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.</remarks>
	/// <seealso cref="QueryFirstOrDefaultAsync{T}(CancellationToken)" />
	public T QueryFirstOrDefault<T>() =>
		Connector.QueryFirst<T>(this, map: null, single: false, orDefault: true);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.</remarks>
	/// <seealso cref="QueryFirstOrDefaultAsync{T}(Func{DbConnectorRecord, T}, CancellationToken)" />
	public T QueryFirstOrDefault<T>(Func<DbConnectorRecord, T> map) =>
		Connector.QueryFirst(this, map ?? throw new ArgumentNullException(nameof(map)), single: false, orDefault: true);

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned, or if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingleAsync{T}(CancellationToken)" />
	public T QuerySingle<T>() =>
		Connector.QueryFirst<T>(this, map: null, single: true, orDefault: false);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned, or if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingleAsync{T}(Func{DbConnectorRecord, T}, CancellationToken)" />
	public T QuerySingle<T>(Func<DbConnectorRecord, T> map) =>
		Connector.QueryFirst(this, map ?? throw new ArgumentNullException(nameof(map)), single: true, orDefault: false);

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.
	/// Throws <see cref="InvalidOperationException"/> if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingleOrDefaultAsync{T}(CancellationToken)" />
	public T QuerySingleOrDefault<T>() =>
		Connector.QueryFirst<T>(this, map: null, single: true, orDefault: true);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.
	/// Throws <see cref="InvalidOperationException"/> if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingleOrDefaultAsync{T}(Func{DbConnectorRecord, T}, CancellationToken)" />
	public T QuerySingleOrDefault<T>(Func<DbConnectorRecord, T> map) =>
		Connector.QueryFirst(this, map ?? throw new ArgumentNullException(nameof(map)), single: true, orDefault: true);

	/// <summary>
	/// Executes the query, converting each record to the specified type.
	/// </summary>
	/// <seealso cref="Query{T}()" />
	public ValueTask<IReadOnlyList<T>> QueryAsync<T>(CancellationToken cancellationToken = default) =>
		Connector.QueryAsync<T>(this, map: null, cancellationToken);

	/// <summary>
	/// Executes the query, converting each record to the specified type with the specified delegate.
	/// </summary>
	/// <seealso cref="Query{T}(Func{DbConnectorRecord, T})" />
	public ValueTask<IReadOnlyList<T>> QueryAsync<T>(Func<DbConnectorRecord, T> map, CancellationToken cancellationToken = default) =>
		Connector.QueryAsync(this, map ?? throw new ArgumentNullException(nameof(map)), cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned.</remarks>
	/// <seealso cref="QueryFirst{T}()" />
	public ValueTask<T> QueryFirstAsync<T>(CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync<T>(this, map: null, single: false, orDefault: false, cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned.</remarks>
	/// <seealso cref="QueryFirst{T}(Func{DbConnectorRecord, T})" />
	public ValueTask<T> QueryFirstAsync<T>(Func<DbConnectorRecord, T> map, CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync(this, map ?? throw new ArgumentNullException(nameof(map)), single: false, orDefault: false, cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.</remarks>
	/// <seealso cref="QueryFirstOrDefault{T}()" />
	public ValueTask<T> QueryFirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync<T>(this, map: null, single: false, orDefault: true, cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.</remarks>
	/// <seealso cref="QueryFirstOrDefault{T}(Func{DbConnectorRecord, T})" />
	public ValueTask<T> QueryFirstOrDefaultAsync<T>(Func<DbConnectorRecord, T> map, CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync(this, map ?? throw new ArgumentNullException(nameof(map)), single: false, orDefault: true, cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned, or if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingle{T}()" />
	public ValueTask<T> QuerySingleAsync<T>(CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync<T>(this, map: null, single: true, orDefault: true, cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> if no records are returned, or if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingle{T}(Func{DbConnectorRecord, T})" />
	public ValueTask<T> QuerySingleAsync<T>(Func<DbConnectorRecord, T> map, CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync(this, map ?? throw new ArgumentNullException(nameof(map)), single: true, orDefault: false, cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.
	/// Throws <see cref="InvalidOperationException"/> if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingleOrDefault{T}()" />
	public ValueTask<T> QuerySingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync<T>(this, map: null, single: true, orDefault: true, cancellationToken);

	/// <summary>
	/// Executes the query, converting the first record to the specified type with the specified delegate.
	/// </summary>
	/// <remarks>Returns <c>default(T)</c> if no records are returned.
	/// Throws <see cref="InvalidOperationException"/> if more than one record is returned.</remarks>
	/// <seealso cref="QuerySingleOrDefault{T}(Func{DbConnectorRecord, T})" />
	public ValueTask<T> QuerySingleOrDefaultAsync<T>(Func<DbConnectorRecord, T> map, CancellationToken cancellationToken = default) =>
		Connector.QueryFirstAsync(this, map ?? throw new ArgumentNullException(nameof(map)), single: true, orDefault: true, cancellationToken);

	/// <summary>
	/// Executes the query, reading one record at a time and converting it to the specified type.
	/// </summary>
	/// <seealso cref="EnumerateAsync{T}(CancellationToken)" />
	public IEnumerable<T> Enumerate<T>() =>
		Connector.Enumerate<T>(this, map: null);

	/// <summary>
	/// Executes the query, reading one record at a time and converting it to the specified type with the specified delegate.
	/// </summary>
	/// <seealso cref="EnumerateAsync{T}(Func{DbConnectorRecord, T}, CancellationToken)" />
	public IEnumerable<T> Enumerate<T>(Func<DbConnectorRecord, T> map) =>
		Connector.Enumerate(this, map ?? throw new ArgumentNullException(nameof(map)));

	/// <summary>
	/// Executes the query, reading one record at a time and converting it to the specified type.
	/// </summary>
	/// <seealso cref="Enumerate{T}()" />
	public IAsyncEnumerable<T> EnumerateAsync<T>(CancellationToken cancellationToken = default) =>
		Connector.EnumerateAsync<T>(this, map: null, cancellationToken);

	/// <summary>
	/// Executes the query, reading one record at a time and converting it to the specified type with the specified delegate.
	/// </summary>
	/// <seealso cref="Enumerate{T}(Func{DbConnectorRecord, T})" />
	public IAsyncEnumerable<T> EnumerateAsync<T>(Func<DbConnectorRecord, T> map, CancellationToken cancellationToken = default) =>
		Connector.EnumerateAsync(this, map ?? throw new ArgumentNullException(nameof(map)), cancellationToken);

	/// <summary>
	/// Executes the query, preparing to read multiple result sets.
	/// </summary>
	/// <seealso cref="QueryMultipleAsync" />
	public DbConnectorResultSets QueryMultiple() => Connector.QueryMultiple(this);

	/// <summary>
	/// Executes the query, preparing to read multiple result sets.
	/// </summary>
	/// <seealso cref="QueryMultiple" />
	public ValueTask<DbConnectorResultSets> QueryMultipleAsync(CancellationToken cancellationToken = default) => Connector.QueryMultipleAsync(this, cancellationToken);

	/// <summary>
	/// Sets the timeout.
	/// </summary>
	/// <remarks>Use <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> (not <see cref="TimeSpan.Zero" />) for infinite timeout.</remarks>
	/// <exception cref="ArgumentOutOfRangeException"><c>timeSpan</c> is not positive or <see cref="System.Threading.Timeout.InfiniteTimeSpan" />.</exception>
	public DbConnectorCommandBatch WithTimeout(TimeSpan timeSpan)
	{
		if (timeSpan <= TimeSpan.Zero && timeSpan != System.Threading.Timeout.InfiniteTimeSpan)
			throw new ArgumentOutOfRangeException(nameof(timeSpan), "Must be positive or 'Timeout.InfiniteTimeSpan'.");

		Timeout = timeSpan;
		return this;
	}

	public DbConnectorCommandBatch WithParameter<T>(string key, T value) =>
		WithParameters(DbParameterSource.Create(key, value));

	public DbConnectorCommandBatch WithParameters(IDbParameterSource source)
	{
		ParameterSources.Add(source);
		return this;
	}

	public DbConnectorCommandBatch WithParameters(DbParameterSources sources)
	{
		ParameterSources.Add(sources);
		return this;
	}

	public DbConnectorCommandBatch WithParameters(params ReadOnlySpan<IDbParameterSource> sources)
	{
		foreach (var source in sources)
			ParameterSources.Add(source);
		return this;
	}

	public DbConnectorCommandBatch WithParameters(IEnumerable<IDbParameterSource> sources)
	{
		foreach (var source in sources)
			ParameterSources.Add(source);
		return this;
	}

	public DbConnectorCommandBatch WithParameters<T>(params ReadOnlySpan<(string Name, T Value)> parameters) =>
		WithParameters(DbParameterSource.Create(parameters));

	public DbConnectorCommandBatch WithParameters<T>(IEnumerable<(string Name, T Value)> parameters) =>
		WithParameters(DbParameterSource.Create(parameters));

	public DbConnectorCommandBatch WithParameters<T>(IEnumerable<KeyValuePair<string, T>> parameters) =>
		WithParameters(DbParameterSource.Create(parameters));

	public DbConnectorCommandBatch WithParametersFromDto<T>(T dto, Func<string, bool>? where = null, Func<string, string>? renamed = null)
	{
		var parameters = DbParameterSource.FromDto(dto);
		if (where is not null)
			parameters = parameters.Where(where);
		if (renamed is not null)
			parameters = parameters.Renamed(renamed);
		return WithParameters(parameters);
	}

	/// <summary>
	/// Caches the command batch.
	/// </summary>
	public DbConnectorCommandBatch Cache(bool cache = true)
	{
		IsCached = cache;
		return this;
	}

	/// <summary>
	/// Prepares the command batch.
	/// </summary>
	public DbConnectorCommandBatch Prepare(bool prepare = true)
	{
		IsPrepared = prepare;
		return this;
	}

	/// <summary>
	/// Creates the next command.
	/// </summary>
	/// <param name="text">The text of the command.</param>
	public DbConnectorCommandBatch Command(string text) => StartNextCommand(CommandType.Text, text);

	/// <summary>
	/// Creates the next command from parameterized SQL.
	/// </summary>
	/// <param name="sql">The parameterized SQL.</param>
	public DbConnectorCommandBatch Command(Sql sql)
	{
		var builder = new DbConnectorCommandBuilder(Connector.SqlSyntax);
		sql.Render(builder);
		var command = builder.Build(CommandType.Text);
		return StartNextCommand(command.Type, command.Text, command.Parameters);
	}

	/// <summary>
	/// Creates the next command from a formatted SQL string.
	/// </summary>
	/// <param name="sql">The formatted SQL string.</param>
	/// <remarks>Shorthand for <c>Command(Sql.Format($"..."))</c>.</remarks>
	public DbConnectorCommandBatch CommandFormat(SqlFormatStringHandler sql) => Command(Sql.Format(sql));

	/// <summary>
	/// Creates the next command to access a stored procedure.
	/// </summary>
	/// <param name="name">The name of the stored procedure.</param>
	public DbConnectorCommandBatch StoredProcedure(string name) => StartNextCommand(CommandType.StoredProcedure, name);

	/// <summary>
	/// Gets the current command.
	/// </summary>
	public DbConnectorCommand CurrentCommand => new(m_commandType, m_text, m_parameterSource ?? m_parameterSources ?? DbParameterSource.Empty);

	/// <summary>
	/// Gets the command at the specified index.
	/// </summary>
	public DbConnectorCommand GetCommand(int index)
	{
		if (index == (m_batchedCommands?.Count ?? 0))
			return CurrentCommand;

		if (m_batchedCommands is null)
			throw new ArgumentOutOfRangeException(nameof(index));

		return m_batchedCommands[index];
	}

	/// <summary>
	/// Replaces the command at the specified index.
	/// </summary>
	public DbConnectorCommandBatch SetCommand(int index, DbConnectorCommand command)
	{
		if (index == (m_batchedCommands?.Count ?? 0))
		{
			m_commandType = command.Type;
			m_text = command.Text;
			m_parameterSource = command.Parameters;
			m_parameterSources = null;
		}
		else
		{
			if (m_batchedCommands is null)
				throw new ArgumentOutOfRangeException(nameof(index));

			m_batchedCommands[index] = command;
		}

		return this;
	}

	internal DbConnectorCommandBatch(DbConnector connector, CommandType commandType, string commandText, IDbParameterSource? parameterSource = null)
	{
		Connector = connector;
		m_commandType = commandType;
		m_text = commandText;
		m_parameterSource = parameterSource;
	}

	private DbParameterSources ParameterSources
	{
		get
		{
			if (m_parameterSources is null)
			{
				m_parameterSources = m_parameterSource is not null ? [m_parameterSource] : [];
				m_parameterSource = null;
			}

			return m_parameterSources;
		}
	}

	private DbConnectorCommandBatch StartNextCommand(CommandType commandType, string commandText, IDbParameterSource? parameterSource = null)
	{
		m_batchedCommands ??= [];
		m_batchedCommands.Add(CurrentCommand);

		m_commandType = commandType;
		m_text = commandText;
		m_parameterSource = parameterSource;
		m_parameterSources = null;

		return this;
	}

	private CommandType m_commandType;
	private string m_text;
	private IDbParameterSource? m_parameterSource;
	private DbParameterSources? m_parameterSources;
	private List<DbConnectorCommand>? m_batchedCommands;
}
