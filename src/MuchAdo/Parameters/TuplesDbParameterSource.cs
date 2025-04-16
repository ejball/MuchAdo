namespace MuchAdo.Parameters;

internal sealed class TuplesDbParameterSource<T>(IEnumerable<(string Name, T Value)> tuples) : IDbParameterSource
{
	public void SubmitParameters(IDbParameterTarget target)
	{
		foreach (var tuple in tuples)
			target.AcceptParameter(tuple.Name, tuple.Value);
	}
}
