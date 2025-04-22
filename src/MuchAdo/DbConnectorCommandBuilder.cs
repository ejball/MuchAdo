using System.Text;
using MuchAdo.SqlFormatting;
using static System.FormattableString;

namespace MuchAdo;

internal sealed class DbConnectorCommandBuilder
{
	public DbConnectorCommandBuilder(SqlSyntax syntax, bool buildText, IDbParameterTarget? parameterTarget)
	{
		Syntax = syntax;
		m_textBuilder = buildText ? new StringBuilder(capacity: 128) : null;
		m_parameterTarget = parameterTarget;
	}

	public SqlSyntax Syntax { get; }

	public int TextLength => m_textLength;

	public string GetText() => m_textBuilder?.ToString() ?? "";

	public void AppendText(string text)
	{
		if (text.Length != 0)
		{
			ApplyPrefixes();
			m_textBuilder?.Append(text);
			m_textLength += text.Length;
		}
	}

	public void AddParameters(IDbParameterSource parameters)
	{
		if (m_parameterTarget is not null)
			parameters.SubmitParameters(m_parameterTarget);
	}

	public void AppendParameterValue<T>(object? key, T value, IDbParameterType? type)
	{
		DoAppendParameter(key, out var needsParameterNamed);
		if (m_parameterTarget is not null && needsParameterNamed is not null)
			m_parameterTarget.AcceptParameter(needsParameterNamed, value, type);
	}

	public void AppendParameterValue<T>(object? key, T valueSource, DbDtoProperty<T> valueProperty, IDbParameterType? type)
	{
		DoAppendParameter(key, out var needsParameterNamed);
		if (m_parameterTarget is not null && needsParameterNamed is not null)
			valueProperty.SubmitParameter(m_parameterTarget, needsParameterNamed, valueSource, type);
	}

	private void DoAppendParameter(object? key, out string? needsParameterNamed)
	{
		ApplyPrefixes();

		if (key is null || m_parameterNames is null || !m_parameterNames.TryGetValue(key, out var tuple))
		{
			if (Syntax.PositionalParameterStrategy.NamedParameterNamePrefix is { } namedPrefix)
			{
				tuple.ParameterName = Invariant($"{namedPrefix}{++m_parameterCount}");
				tuple.SqlPlaceholder = Invariant($"{Syntax.NamedParameterPrefix}{tuple.ParameterName}");
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

		m_textBuilder?.Append(tuple.SqlPlaceholder);
		m_textLength += tuple.SqlPlaceholder.Length;
	}

	private void ApplyPrefixes()
	{
		if (m_brackets is { Count: not 0 })
		{
			for (var index = 0; index < m_brackets.Count; index++)
			{
				var (prefix, suffix) = m_brackets[index];
				if (prefix is not null)
				{
					m_textBuilder?.Append(prefix);
					m_textLength += prefix.Length;
					m_brackets[index] = (null, suffix);
				}
			}
		}
	}

	public DbConnectorBracketScope Prefix(string prefix) => Bracket(prefix, null);

	public DbConnectorBracketScope Bracket(string prefix, string? suffix)
	{
		(m_brackets ??= new()).Add((prefix, suffix));
		return new(this);
	}

	internal void EndBracket()
	{
		var index = m_brackets!.Count - 1;
		var (prefix, suffix) = m_brackets[index];
		if (prefix is null && suffix is not null)
		{
			m_textBuilder?.Append(suffix);
			m_textLength += suffix.Length;
		}
		m_brackets!.RemoveAt(index);
	}

	private readonly StringBuilder? m_textBuilder;
	private readonly IDbParameterTarget? m_parameterTarget;
	private int m_textLength;
	private int m_parameterCount;
	private List<(string? Prefix, string? Suffix)>? m_brackets;
	private Dictionary<object, (string ParameterName, string SqlPlaceholder)>? m_parameterNames;
}
