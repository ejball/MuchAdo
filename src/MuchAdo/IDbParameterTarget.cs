namespace MuchAdo;

public interface IDbParameterTarget
{
	void AcceptParameter<T>(string name, T value, IDbParameterType? type);
}
