using System.Runtime.CompilerServices;

namespace MuchAdo.SqlFormatting;

[InterpolatedStringHandler]
public readonly ref struct SqlFormatStringHandler
{
	public SqlFormatStringHandler(int literalLength, int formattedCount)
	{
		m_parts = new(capacity: formattedCount * 2 + 1);
	}

	public void AppendLiteral(string s) => m_parts.Add(s);

	public void AppendFormatted<T>(T t) => m_parts.Add(t as SqlSource ?? new FormatParamSql<T>(t));

	public void AppendFormatted<T>(T t, string? format)
	{
		if (format == "raw" && t is string { } text)
			m_parts.Add(text);
		else
			throw new NotSupportedException($"Format '{format}' not supported for {typeof(T).FullName}.");
	}

	internal SqlSource ToSql() => new FormatSql(m_parts);

	private readonly List<object> m_parts;
}
