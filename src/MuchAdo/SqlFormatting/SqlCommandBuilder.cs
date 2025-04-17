using System.Text;
using MuchAdo.Parameters;
using static System.FormattableString;

namespace MuchAdo.SqlFormatting;

internal sealed class SqlCommandBuilder
{
	public SqlCommandBuilder(SqlSyntax syntax)
	{
		Syntax = syntax;
		m_textBuilder = new StringBuilder();
		m_parameterSources = new DbParameterSources();
	}

	public SqlSyntax Syntax { get; }

	public string Text => m_textBuilder.ToString();

	public int TextLength => m_textBuilder.Length;

	public IDbParameterSource Parameters => m_parameterSources;

	public void AppendText(string text)
	{
		if (text.Length != 0)
		{
			ApplyPrefixes();
			m_textBuilder.Append(text);
		}
	}

	public void AppendText(char ch)
	{
		ApplyPrefixes();
		m_textBuilder.Append(ch);
	}

	public void AppendParameterValue<T>(object? key, T value)
	{
		DoAppendParameter(key, out var needsParameterNamed);
		if (needsParameterNamed is not null)
			m_parameterSources.Add(DbParameterSource.Create(needsParameterNamed, value));
	}

	public void AppendParameterValue<T>(object? key, T valueSource, DbDtoProperty<T> valueProperty)
	{
		DoAppendParameter(key, out var needsParameterNamed);
		if (needsParameterNamed is not null)
			m_parameterSources.Add(new PropertyDbParameter<T>(needsParameterNamed, valueSource, valueProperty));
	}

	private void DoAppendParameter(object? key, out string? needsParameterNamed)
	{
		ApplyPrefixes();

		if (key is null || m_parameterNames is null || !m_parameterNames.TryGetValue(key, out var tuple))
		{
			if (Syntax.PositionalParameterStrategy.NamedParameterNamePrefix is { } namedPrefix)
			{
				tuple.ParameterName = Invariant($"{namedPrefix}{++m_parameterCount}");
				tuple.SqlPlaceholder = Invariant($"{Syntax.NamedParameterChar}{tuple.ParameterName}");
				if (key is not null)
					(m_parameterNames ??= new()).Add(key, tuple);
			}
			else if (Syntax.PositionalParameterStrategy.NumberedParameterPlaceholderPrefix is { } numberedPrefix)
			{
				tuple.ParameterName = "";
				tuple.SqlPlaceholder = Invariant($"{numberedPrefix}{++m_parameterCount}");
				if (key is not null)
					(m_parameterNames ??= new()).Add(key, tuple);
			}
			else if (Syntax.PositionalParameterStrategy.UnnumberedParameterPlaceholder is { } unnumberedPlaceholder)
			{
				tuple.ParameterName = "";
				tuple.SqlPlaceholder = unnumberedPlaceholder;
			}
			else
			{
				throw new InvalidOperationException($"Unexpected {nameof(Syntax.PositionalParameterStrategy)}.");
			}

			needsParameterNamed = tuple.ParameterName;
		}
		else
		{
			needsParameterNamed = null;
		}

		m_textBuilder.Append(tuple.SqlPlaceholder);
	}

	private void ApplyPrefixes()
	{
		if (m_prefixes is { Count: not 0 })
		{
			for (var index = 0; index < m_prefixes.Count; index++)
			{
				var prefix = m_prefixes[index];
				if (prefix is not null)
				{
					m_textBuilder.Append(prefix);
					m_prefixes[index] = null;
				}
			}
		}
	}

	public void AddParameters(IDbParameterSource parameters) => m_parameterSources.Add(parameters);

	public DbConnectorBracketScope Prefix(string prefix) => Bracket(prefix, "");

	public DbConnectorBracketScope Bracket(string prefix, string suffix)
	{
		(m_prefixes ??= new()).Add(prefix);
		(m_suffixes ??= new()).Add(suffix);
		return new(this);
	}

	internal void EndBracket()
	{
		var index = m_prefixes!.Count - 1;
		if (m_prefixes![index] is null)
			m_textBuilder.Append(m_suffixes![index]);
		m_prefixes!.RemoveAt(index);
		m_suffixes!.RemoveAt(index);
	}

	public (string Text, IDbParameterSource Parameters) Build() => (m_textBuilder.ToString(), m_parameterSources);

	private readonly StringBuilder m_textBuilder;
	private readonly DbParameterSources m_parameterSources;
	private int m_parameterCount;
	private List<string?>? m_prefixes;
	private List<string?>? m_suffixes;
	private Dictionary<object, (string ParameterName, string SqlPlaceholder)>? m_parameterNames;
}
