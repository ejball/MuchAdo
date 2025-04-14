namespace MuchAdo.Parameters;

internal sealed class PropertyDbParameter<T>(string name, T valueSource, DbDtoProperty<T> valueProperty) : DbParameters
{
	internal override void SubmitParametersCore(IDbParameterTarget target, Func<string, bool>? filterName, Func<string, string>? transformName)
	{
		if (filterName is null || filterName(name))
		{
			var transformedName = transformName is null ? name : transformName(name);
			valueProperty.SubmitParameter(target, transformedName, valueSource);
		}
	}
}
