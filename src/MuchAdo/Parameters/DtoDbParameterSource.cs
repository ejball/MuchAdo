namespace MuchAdo.Parameters;

internal sealed class DtoDbParameterSource<T>(T dto) : IDbParameterSource
{
	public void SubmitParameters(IDbParameterTarget target)
	{
		foreach (var property in DbDtoInfo.GetInfo<T>().Properties)
			property.SubmitParameter(target, property.Name, dto, type: null);
	}
}
