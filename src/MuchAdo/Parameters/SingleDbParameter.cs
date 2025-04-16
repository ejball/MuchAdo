namespace MuchAdo.Parameters;

internal sealed class SingleDbParameter<T>(string name, T value) : IDbParameterSource
{
	public void SubmitParameters(IDbParameterTarget target) => target.AcceptParameter(name, value);
}
