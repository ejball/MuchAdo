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

	public string Text => m_textBuilder?.ToString() ?? "";

	public int TextLength => m_textLength;

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
		if (m_prefixes is { Count: not 0 })
		{
			for (var index = 0; index < m_prefixes.Count; index++)
			{
				var prefix = m_prefixes[index];
				if (prefix is not null)
				{
					m_textBuilder?.Append(prefix);
					m_textLength += prefix.Length;
					m_prefixes[index] = null;
				}
			}
		}
	}

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
		{
			var suffix = m_suffixes![index]!;
			m_textBuilder?.Append(suffix);
			m_textLength += suffix.Length;
		}
		m_prefixes!.RemoveAt(index);
		m_suffixes!.RemoveAt(index);
	}

	private readonly StringBuilder? m_textBuilder;
	private readonly IDbParameterTarget? m_parameterTarget;
	private int m_textLength;
	private int m_parameterCount;
	private List<string?>? m_prefixes;
	private List<string?>? m_suffixes;
	private Dictionary<object, (string ParameterName, string SqlPlaceholder)>? m_parameterNames;
}
