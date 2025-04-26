namespace MuchAdo.Sources;

public sealed class DtoParamsSqlSource<T> : SqlSource
{
	public DtoParamsSqlSource<T> Where(Func<string, bool> filter) =>
		new(m_dto, m_filter is null ? filter : x => m_filter(x) && filter(x));

	internal DtoParamsSqlSource(T dto, Func<string, bool>? filter = null)
	{
		m_dto = dto;
		m_filter = filter;
	}

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		var properties = DbDtoInfo.GetInfo<T>().Properties;
		if (properties.Count == 0)
			throw new InvalidOperationException($"The specified type has no columns: {typeof(T).FullName}");

		var oldTextLength = builder.TextLength;

		foreach (var property in properties)
		{
			if (m_filter is null || m_filter(property.Name))
			{
				if (builder.TextLength != oldTextLength)
					builder.AppendText(", ");
				builder.AppendParameterValue(null, m_dto, property);
			}
		}

		if (builder.TextLength == oldTextLength)
			throw new InvalidOperationException($"The specified type has no remaining columns: {typeof(T).FullName}");
	}

	private readonly T m_dto;
	private readonly Func<string, bool>? m_filter;
}
