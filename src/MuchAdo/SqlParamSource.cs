using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using MuchAdo.Sources;

namespace MuchAdo;

public abstract class SqlParamSource : SqlSource
{
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use Sql.Param.")]
	public static implicit operator SqlParamSource(DbParameter parameter) => Sql.Param(parameter);

	public IEnumerable<SqlParam<object?>> Enumerate()
	{
		var target = new EnumerateParameterTarget();
		SubmitParameters(target);
		return target.Items;
	}

	private sealed class EnumerateParameterTarget : ISqlParamTarget
	{
		public Collection<SqlParam<object?>> Items { get; } = new();

		public void AcceptParameter<T>(string name, T value, SqlParamType? type)
		{
			if (string.IsNullOrEmpty(name))
				Items.Add(type is null ? Sql.Param<object?>(value) : Sql.Param<object?>(value, type));
			else
				Items.Add(type is null ? Sql.NamedParam<object?>(name, value) : Sql.NamedParam<object?>(name, value, type));
		}
	}

	/// <summary>
	/// Filters the parameters by name.
	/// </summary>
	public SqlParamSource Where(Func<string, bool> nameMatches)
	{
		if (nameMatches is null)
			throw new ArgumentNullException(nameof(nameMatches));
		return new FilteredSqlParamSource(this, nameMatches);
	}

	/// <summary>
	/// Transforms the parameter names using the specified function.
	/// </summary>
	public SqlParamSource Renamed(Func<string, string> transform)
	{
		if (transform is null)
			throw new ArgumentNullException(nameof(transform));
		return new RenamedSqlParamSource(this, transform);
	}

	internal abstract void SubmitParameters(ISqlParamTarget target);

	internal override void Render(DbConnectorCommandBuilder builder)
	{
		var target = new ParamTarget(builder);
		SubmitParameters(target);
	}

	private readonly struct ParamTarget(DbConnectorCommandBuilder builder) : ISqlParamTarget
	{
		public void AcceptParameter<T>(string name, T value, SqlParamType? type)
		{
			if (m_originalTextLength != builder.TextLength)
				builder.AppendText(", ");

			if (string.IsNullOrEmpty(name))
			{
				builder.AppendParameterValue(identity: null, value, type);
			}
			else
			{
				builder.AppendText(builder.Syntax.NamedParameterPrefix);
				builder.AppendText(name);
				builder.SubmitParameters(Sql.NamedParam(name, value, type));
			}
		}

		private readonly int m_originalTextLength = builder.TextLength;
	}
}
