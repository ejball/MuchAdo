namespace MuchAdo;

internal sealed class ReusedSqlParam<T>(T value) : SqlParam<T>(value)
{
	internal override bool IsReused => true;
}
