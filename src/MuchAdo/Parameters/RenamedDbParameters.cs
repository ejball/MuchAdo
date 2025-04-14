namespace MuchAdo.Parameters;

internal sealed class RenamedDbParameters(DbParameters source, Func<string, string> named) : DbParameters
{
	internal override void SubmitParametersCore(IDbParameterTarget target, Func<string, bool>? filterName, Func<string, string>? transformName) => source.SubmitParametersCore(target, FilterName(filterName), TransformName(transformName));

	private Func<string, bool>? FilterName(Func<string, bool>? filterName) =>
		filterName is null ? null : x => filterName(named(x));

	private Func<string, string> TransformName(Func<string, string>? transformName) =>
		transformName is null ? named : x => transformName(named(x));
}
