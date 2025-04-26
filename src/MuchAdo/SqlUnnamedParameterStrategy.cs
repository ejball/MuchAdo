namespace MuchAdo;

public sealed class SqlUnnamedParameterStrategy
{
	public static SqlUnnamedParameterStrategy Named(string namePrefix) =>
		new() { NamedParameterNamePrefix = namePrefix };

	public static SqlUnnamedParameterStrategy Numbered(string placeholderPrefix) =>
		new() { NumberedParameterPlaceholderPrefix = placeholderPrefix };

	public static SqlUnnamedParameterStrategy Unnumbered(string placeholder) =>
		new() { UnnumberedParameterPlaceholder = placeholder };

	internal string? NamedParameterNamePrefix { get; init; }

	internal string? NumberedParameterPlaceholderPrefix { get; init; }

	internal string? UnnumberedParameterPlaceholder { get; init; }

	private SqlUnnamedParameterStrategy()
	{
	}
}
