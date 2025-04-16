namespace MuchAdo.Parameters;

internal sealed class DictionaryDbParameterSource<T>(IEnumerable<KeyValuePair<string, T>> pairs) : IDbParameterSource
{
	public void SubmitParameters(IDbParameterTarget target)
	{
		foreach (var pair in pairs)
			target.AcceptParameter(pair.Key, pair.Value);
	}
}
