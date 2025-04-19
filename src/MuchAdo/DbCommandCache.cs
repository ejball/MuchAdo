namespace MuchAdo;

internal sealed class DbCommandCache
{
	public object? GetValueOrDefault(object key) => m_dictionary.TryGetValue(key, out var value) ? value : null;

	public void AddValue(object key, object value) => m_dictionary.Add(key, value);

	public IReadOnlyCollection<object> GetValues() => m_dictionary.Values;

	private readonly Dictionary<object, object> m_dictionary = new();
}
