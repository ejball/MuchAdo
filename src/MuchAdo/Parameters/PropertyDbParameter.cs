namespace MuchAdo.Parameters;

internal sealed class PropertyDbParameter<T>(string name, T valueSource, DbDtoProperty<T> valueProperty) : IDbParameterSource
{
	public void SubmitParameters(IDbParameterTarget target) => valueProperty.SubmitParameter(target, name, valueSource);
}
