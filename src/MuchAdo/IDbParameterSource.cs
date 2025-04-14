namespace MuchAdo;

internal interface IDbParameterSource
{
	void SubmitParameters(IDbParameterTarget target);
}
