namespace MuchAdo.Parameters;

internal sealed class WhereDbParameters(DbParameters source, Func<string, bool> where) : DbParameters
{
	internal override void SubmitParametersCore(IDbParameterTarget target, Func<string, bool>? filterName, Func<string, string>? transformName) => source.SubmitParametersCore(target, FilterName(filterName, transformName), transformName);

	private Func<string, bool> FilterName(Func<string, bool>? filterName, Func<string, string>? transformName) =>
		filterName is null ? where : x => where(x) && filterName(transformName is null ? x : transformName(x));
}
