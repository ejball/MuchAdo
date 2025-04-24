namespace MuchAdo.Parameters;

internal sealed class NamedFromDtoSqlParamSource<T>(T dto) : SqlParamSource
{
	internal override void SubmitParameters(ISqlParamTarget target)
	{
		foreach (var property in DbDtoInfo.GetInfo<T>().Properties)
			property.SubmitParameter(target, property.Name, dto, type: null);
	}
}
