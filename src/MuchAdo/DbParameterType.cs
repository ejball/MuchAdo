using System.Data;

namespace MuchAdo;

public static class DbParameterType
{
	public static IDbParameterType Default { get; } = new DefaultDbParameterType();

	public static IDbParameterType FromAction(Action<IDataParameter> action) => new ActionDbParameterType(action);

	public static IDbParameterType FromDbType(DbType dbType) => new DbTypeDbParameterType(dbType);

	private sealed class DefaultDbParameterType : IDbParameterType
	{
		public void ApplyToParameter(IDataParameter parameter)
		{
		}
	}

	private sealed class ActionDbParameterType(Action<IDataParameter> action) : IDbParameterType
	{
		public void ApplyToParameter(IDataParameter parameter) => action(parameter);
	}

	private sealed class DbTypeDbParameterType(DbType dbType) : IDbParameterType
	{
		public void ApplyToParameter(IDataParameter parameter) => parameter.DbType = dbType;
	}
}
