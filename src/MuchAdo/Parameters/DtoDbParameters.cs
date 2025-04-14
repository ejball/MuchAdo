namespace MuchAdo.Parameters;

internal sealed class DtoDbParameters<T>(T dto) : DbParameters
{
	internal override void SubmitParametersCore(IDbParameterTarget target, Func<string, bool>? filterName, Func<string, string>? transformName)
	{
		foreach (var property in DbDtoInfo.GetInfo<T>().Properties)
		{
			var name = property.Name;
			if (filterName is null || filterName(name))
			{
				var transformedName = transformName is null ? name : transformName(name);
				property.SubmitParameter(target, transformedName, dto);
			}
		}
	}
}
