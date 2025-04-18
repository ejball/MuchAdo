using System.Data;

namespace MuchAdo;

/// <summary>
/// Command type, text, and parameters for a database query.
/// </summary>
public readonly struct DbConnectorQuery(CommandType commandType, string commandText, IDbParameterSource parameterSource)
{
	/// <summary>
	/// The command type.
	/// </summary>
	public CommandType CommandType => commandType;

	/// <summary>
	/// Gets the command text.
	/// </summary>
	public string CommandText => commandText;

	/// <summary>
	/// Gets the parameters.
	/// </summary>
	public IDbParameterSource ParameterSource => parameterSource;
}
