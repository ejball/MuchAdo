using System.Diagnostics.CodeAnalysis;

namespace MuchAdo.SqlFormatting;

/// <summary>
/// Encapsulates parameterized SQL.
/// </summary>
public abstract class SqlSource
{
	/// <summary>
	/// Concatenates two SQL fragments.
	/// </summary>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use Concat.")]
	public static SqlSource operator +(SqlSource a, SqlSource b) => new AddSql(a ?? throw new ArgumentNullException(nameof(a)), b ?? throw new ArgumentNullException(nameof(b)));

	public override string ToString() => ToString(SqlSyntax.Ansi);

	public string ToString(SqlSyntax syntax)
	{
		var commandBuilder = new DbConnectorCommandBuilder(syntax, buildText: true, parameterTarget: null);
		Render(commandBuilder);
		return commandBuilder.GetText();
	}

	internal abstract void Render(DbConnectorCommandBuilder builder);
}
