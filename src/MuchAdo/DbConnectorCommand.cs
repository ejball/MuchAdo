using System.Data;

namespace MuchAdo;

/// <summary>
/// The type, text, and parameters of a database command.
/// </summary>
public readonly struct DbConnectorCommand(CommandType type, string text, IDbParameterSource parameters)
{
	/// <summary>
	/// The command type.
	/// </summary>
	public CommandType Type => type;

	/// <summary>
	/// Gets the command text.
	/// </summary>
	public string Text => text;

	/// <summary>
	/// Gets the parameters.
	/// </summary>
	public IDbParameterSource Parameters => parameters;
}
