namespace MuchAdo;

internal sealed class DbCommandCache
{
	public object? TryRemoveCommand(object key)
	{
		if (!m_dictionary.TryGetValue(key, out var command))
			return null;

		m_dictionary.Remove(key);
		return command;
	}

	public void AddCommand(object key, object command) => m_dictionary.Add(key, command);

	public IReadOnlyCollection<object> GetCommandCollection() => m_dictionary.Values;

	private readonly Dictionary<object, object> m_dictionary = new(KeyComparer.Instance);

	private sealed class KeyComparer : IEqualityComparer<object>
	{
		public static readonly KeyComparer Instance = new();

		bool IEqualityComparer<object>.Equals(object? x, object? y)
		{
			if (x is IEnumerable<string> xs && y is IEnumerable<string> ys)
				return xs.SequenceEqual(ys, StringComparer.Ordinal);

			return EqualityComparer<object>.Default.Equals(x!, y!);
		}

		int IEqualityComparer<object>.GetHashCode(object obj)
		{
			if (obj is IEnumerable<string> texts)
			{
				var hash = 0;
				foreach (var text in texts)
					hash = PortableUtility.CombineHashCodes(hash, text.GetHashCodeOrdinal());
				return hash;
			}

			return EqualityComparer<object>.Default.GetHashCode(obj);
		}
	}
}
