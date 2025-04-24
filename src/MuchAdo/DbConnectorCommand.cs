using System.Data;

namespace MuchAdo;

/// <summary>
/// The type, text, and parameters of a database command.
/// </summary>
public readonly struct DbConnectorCommand
{
	public DbConnectorCommand(CommandType type, string text, SqlParamSource parameters)
		: this(type, (object) text, parameters)
	{
	}

	public DbConnectorCommand(CommandType type, SqlSource sql, SqlParamSource parameters)
		: this(type, (object) sql, parameters)
	{
	}

	/// <summary>
	/// The command type.
	/// </summary>
	public CommandType Type { get; }

	/// <summary>
	/// Gets the text of the command.
	/// </summary>
	public string? Text => m_textOrSql as string;

	/// <summary>
	/// Gets the parameterized SQL for the command.
	/// </summary>
	public SqlSource? Sql => m_textOrSql as SqlSource;

	/// <summary>
	/// Gets the parameters.
	/// </summary>
	public SqlParamSource Parameters { get; }

	/// <summary>
	/// Gets the text of the command, building it from parameterized SQL as needed.
	/// </summary>
	public string BuildText(SqlSyntax sqlSyntax) => Text ?? Sql!.ToString(sqlSyntax);

	internal DbConnectorCommand(CommandType type, object textOrSql, SqlParamSource parameters)
	{
		Type = type;
		m_textOrSql = textOrSql;
		Parameters = parameters;
	}

	internal object TextOrSql => m_textOrSql;

	private readonly object m_textOrSql;
}
