namespace MuchAdo;

public interface IDbParameterSource
{
	void SubmitParameters(IDbParameterTarget target);
}
