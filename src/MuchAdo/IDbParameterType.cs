using System.Data;

namespace MuchAdo;

public interface IDbParameterType
{
	void ApplyToParameter(IDataParameter parameter);
}
