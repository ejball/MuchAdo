namespace MuchAdo.Parameters;

internal sealed class RenamedDbParameterSource(IDbParameterSource source, Func<string, string> rename) : IDbParameterSource
{
	public void SubmitParameters(IDbParameterTarget target) => source.SubmitParameters(new RenamedDbParameterTarget(target, rename));

	private sealed class RenamedDbParameterTarget(IDbParameterTarget target, Func<string, string> rename) : IDbParameterTarget
	{
		public void AcceptParameter<T>(string name, T value, IDbParameterType? type) => target.AcceptParameter(rename(name), value, type);
	}
}
