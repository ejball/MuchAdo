namespace MuchAdo;

internal interface IDbParameterTarget
{
	void AcceptParameter<T>(string name, T value);
}
