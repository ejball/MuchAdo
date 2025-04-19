namespace MuchAdo.Parameters;

public sealed class SingleDbParameter<T>(string name, T value, IDbParameterType? type) : IDbParameterSource
{
	public string Name { get; set; } = name;

	public T Value { get; set; } = value;

	public IDbParameterType? Type { get; set; } = type;

	public void SubmitParameters(IDbParameterTarget target) => target.AcceptParameter(Name, Value, Type);
}
