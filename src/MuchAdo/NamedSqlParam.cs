namespace MuchAdo;

internal sealed class NamedSqlParam<T>(string name, T value) : SqlParam<T>(value)
{
	public override string Name => name;
}
