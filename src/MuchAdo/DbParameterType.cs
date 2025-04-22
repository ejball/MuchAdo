using System.Data;
using System.Data.Common;

namespace MuchAdo;

public static class DbParameterType
{
	public static IDbParameterType Create(Action<DbParameter> action) => new ActionDbParameterType(action);

	private sealed class ActionDbParameterType(Action<DbParameter> action) : IDbParameterType
	{
		public void ApplyToParameter(IDataParameter parameter) => action((DbParameter) parameter);
	}
}
