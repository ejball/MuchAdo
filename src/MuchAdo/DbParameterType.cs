using System.Data;

namespace MuchAdo;

public static class DbParameterType
{
	public static IDbParameterType Create<T>(Action<T> action)
		where T : IDataParameter => new ActionDbParameterType<T>(action);

	private sealed class ActionDbParameterType<T>(Action<T> action) : IDbParameterType
		where T : IDataParameter
	{
		public void ApplyToParameter(IDataParameter parameter) => action((T) parameter);
	}
}
