using System.Data;
using System.Data.Common;

namespace MuchAdo;

public abstract class SqlParamType
{
	public static SqlParamType Create(Action<DbParameter> action) => new ActionSqlParamType(action);

	private sealed class ActionSqlParamType(Action<DbParameter> action) : SqlParamType
	{
		public override void Apply(IDataParameter parameter) => action((DbParameter) parameter);
	}

	public abstract void Apply(IDataParameter parameter);
}
