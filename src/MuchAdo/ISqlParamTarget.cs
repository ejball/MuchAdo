namespace MuchAdo;

public interface ISqlParamTarget
{
	void AcceptParameter<T>(string name, T value, SqlParamType? type);
}
