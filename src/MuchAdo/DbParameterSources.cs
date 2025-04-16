namespace MuchAdo;

/// <summary>
/// A list of sets of parameters.
/// </summary>
public sealed class DbParameterSources : IDbParameterSource
{
	/// <summary>
	/// Creates an empty list.
	/// </summary>
	public DbParameterSources() => m_sources = [];

	/// <summary>
	/// Creates a list from the specified sets of parameters.
	/// </summary>
	public DbParameterSources(params ReadOnlySpan<IDbParameterSource> items) => m_sources = [.. items];

	/// <summary>
	/// Creates a list from the specified sets of parameters.
	/// </summary>
	public DbParameterSources(IEnumerable<IDbParameterSource> items) => m_sources = [.. items];

	public void Add(IDbParameterSource source) => m_sources.Add(source);

	public void SubmitParameters(IDbParameterTarget target)
	{
		foreach (var source in m_sources)
			source.SubmitParameters(target);
	}

	private readonly List<IDbParameterSource> m_sources;
}
