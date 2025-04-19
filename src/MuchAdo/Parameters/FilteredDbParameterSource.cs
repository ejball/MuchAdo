namespace MuchAdo.Parameters;

internal sealed class FilteredDbParameterSource(IDbParameterSource source, Func<string, bool> nameMatches) : IDbParameterSource
{
	public void SubmitParameters(IDbParameterTarget target) => source.SubmitParameters(new FilteredDbParameterTarget(target, nameMatches));

	private sealed class FilteredDbParameterTarget(IDbParameterTarget target, Func<string, bool> where) : IDbParameterTarget
	{
		public void AcceptParameter<T>(string name, T value, IDbParameterType? type)
		{
			if (where(name))
				target.AcceptParameter(name, value, type);
		}
	}
}
