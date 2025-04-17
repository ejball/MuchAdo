namespace MuchAdo.SqlFormatting;

public sealed class SqlPositionalParameterStrategy
{
	public static SqlPositionalParameterStrategy Named(string namePrefix) =>
		new() { NamedParameterNamePrefix = namePrefix };

	public static SqlPositionalParameterStrategy Numbered(string placeholderPrefix) =>
		new() { NumberedParameterPlaceholderPrefix = placeholderPrefix };

	public static SqlPositionalParameterStrategy Unnumbered(string placeholder) =>
		new() { UnnumberedParameterPlaceholder = placeholder };

	internal string? NamedParameterNamePrefix { get; init; }

	internal string? NumberedParameterPlaceholderPrefix { get; init; }

	internal string? UnnumberedParameterPlaceholder { get; init; }

	private SqlPositionalParameterStrategy()
	{
	}
}
