namespace MuchAdo;

internal sealed class TypedSqlParam<T>(T value, SqlParamType? type) : SqlParam<T>(value)
{
	public override SqlParamType? Type => type;
}
