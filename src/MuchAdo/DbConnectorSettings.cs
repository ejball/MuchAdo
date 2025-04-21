using System.Data;
using MuchAdo.SqlFormatting;

namespace MuchAdo;

/// <summary>
/// Settings when creating a <see cref="DbConnector"/>.
/// </summary>
public class DbConnectorSettings
{
	/// <summary>
	/// The SQL syntax to use when formatting SQL.
	/// </summary>
	public SqlSyntax? SqlSyntax { get; init; }

	/// <summary>
	/// Maps data record values to objects.
	/// </summary>
	public DbDataMapper? DataMapper { get; init; }

	/// <summary>
	/// The isolation level used when <c>BeginTransaction(Async)</c> is called without one.
	/// </summary>
	/// <remarks>If not specified, the behavior is provider-specific.</remarks>
	public IsolationLevel? DefaultIsolationLevel { get; init; }

	/// <summary>
	/// If true, does not dispose the connection when the connector is disposed.
	/// </summary>
	public bool NoDisposeConnection { get; init; }

	/// <summary>
	/// If true, the command or batch is cancelled when the active reader is not read to the end.
	/// </summary>
	/// <remarks>This can occur when an exception is thrown or when breaking early out of
	/// an <see cref="DbConnector.Enumerate{T}" /> or <see cref="DbConnector.EnumerateAsync{T}" />
	/// loop.</remarks>
	public bool CancelUnfinishedCommands { get; init; }

	internal static DbConnectorSettings Default { get; } = new();
}
