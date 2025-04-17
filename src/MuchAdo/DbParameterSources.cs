using System.Collections;

namespace MuchAdo;

/// <summary>
/// A list of sets of parameters.
/// </summary>
public sealed class DbParameterSources : IList<IDbParameterSource>, IReadOnlyList<IDbParameterSource>, IDbParameterSource
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

	/// <inheritdoc />
	public void Add(IDbParameterSource source) => m_sources.Add(source);

	/// <inheritdoc />
	public void Clear() => m_sources.Clear();

	/// <inheritdoc />
	public bool Contains(IDbParameterSource item) => m_sources.Contains(item);

	/// <inheritdoc />
	public void CopyTo(IDbParameterSource[] array, int arrayIndex) => m_sources.CopyTo(array, arrayIndex);

	/// <inheritdoc />
	public bool Remove(IDbParameterSource item) => m_sources.Remove(item);

	public int Count => m_sources.Count;

	/// <inheritdoc />
	public IEnumerator<IDbParameterSource> GetEnumerator() => m_sources.GetEnumerator();

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator() => m_sources.GetEnumerator();

	/// <inheritdoc />
	public int IndexOf(IDbParameterSource item) => m_sources.IndexOf(item);

	/// <inheritdoc />
	public void Insert(int index, IDbParameterSource item) => m_sources.Insert(index, item);

	/// <inheritdoc />
	public void RemoveAt(int index) => m_sources.RemoveAt(index);

	public IDbParameterSource this[int index]
	{
		get => m_sources[index];
		set => m_sources[index] = value;
	}

	public void SubmitParameters(IDbParameterTarget target)
	{
		foreach (var source in m_sources)
			source.SubmitParameters(target);
	}

	/// <inheritdoc />
	bool ICollection<IDbParameterSource>.IsReadOnly => false;

	private readonly List<IDbParameterSource> m_sources;
}
