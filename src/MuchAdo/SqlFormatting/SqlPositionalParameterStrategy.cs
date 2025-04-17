namespace MuchAdo.SqlFormatting;

public sealed class SqlPositionalParameterStrategy
{
	public static SqlPositionalParameterStrategy Named(string namePrefix)
	{
		if (string.IsNullOrEmpty(namePrefix))
			throw new ArgumentException("The name prefix cannot be null or empty.", nameof(namePrefix));
		return new() { NamedPositionalParameterNamePrefix = namePrefix };
	}

	internal string? NamedPositionalParameterNamePrefix { get; init; }

	private SqlPositionalParameterStrategy()
	{
	}
}
