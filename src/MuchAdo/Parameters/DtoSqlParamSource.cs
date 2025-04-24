namespace MuchAdo.Parameters;

internal sealed class DtoSqlParamSource<T>(T dto) : SqlParamSource
{
	internal override void Submit(ISqlParamTarget target)
	{
		foreach (var property in DbDtoInfo.GetInfo<T>().Properties)
			property.SubmitParameter(target, property.Name, dto, type: null);
	}
}
