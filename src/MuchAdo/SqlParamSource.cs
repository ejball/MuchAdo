using System.Collections.ObjectModel;
using MuchAdo.Parameters;
using MuchAdo.SqlFormatting;

namespace MuchAdo;

public abstract class SqlParamSource : Sql
{
	/// <summary>
	/// An empty list of parameters.
	/// </summary>
	public new static readonly SqlParamSource Empty = new EmptySqlParamSource();

#if false
	/// <summary>
	/// Creates one parameter.
	/// </summary>
	public static NamedSqlParam<T> Create<T>(string name, T value) => new(name, value);

	/// <summary>
	/// Creates one parameter.
	/// </summary>
	public static SingleTypedSqlParam<T> Create<T>(string name, T value, SqlParamType? type) => new(name, value, type);

	/// <summary>
	/// Creates parameters from a sequence of parameters.
	/// </summary>
	public static SqlParamSource Create(params ReadOnlySpan<SqlParamSource> parameters) =>
		parameters.Length switch
		{
			0 => Empty,
			1 => parameters[0],
			_ => new SqlParamSources(parameters),
		};

	/// <summary>
	/// Creates parameters from a sequence of parameters.
	/// </summary>
	public static SqlParamSource Create(IEnumerable<SqlParamSource> parameters) =>
		new SqlParamSources(parameters ?? throw new ArgumentNullException(nameof(parameters)));

	/// <summary>
	/// Creates parameters from a sequence of parameters.
	/// </summary>
	public static SqlParamSource Create<T>(params ReadOnlySpan<(string Name, T Value)> parameters) =>
		parameters.Length switch
		{
			0 => Empty,
			1 => Create(parameters[0].Name, parameters[0].Value),
			_ => new TuplesSqlParamSource<T>(parameters.ToArray()),
		};

	/// <summary>
	/// Creates parameters from a sequence of name/value pairs.
	/// </summary>
	public static SqlParamSource Create<T>(IEnumerable<(string Name, T Value)> parameters) =>
		new TuplesSqlParamSource<T>(parameters ?? throw new ArgumentNullException(nameof(parameters)));

	/// <summary>
	/// Creates a list of parameters from the properties of a DTO.
	/// </summary>
	/// <remarks>The name of each parameter is the name of the corresponding DTO property.</remarks>
	public static SqlParamSource FromDto<T>(T dto)
	{
		if (dto is null)
			throw new ArgumentNullException(nameof(dto));
		return new DtoSqlParamSource<T>(dto);
	}
#endif

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
				Items.Add(type is null ? Param<object?>(value) : Param<object?>(value, type));
			else
				Items.Add(type is null ? NamedParam<object?>(name, value) : NamedParam<object?>(name, value, type));
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
		throw new NotSupportedException("SqlParamSource cannot be rendered directly.");
	}

	private sealed class EmptySqlParamSource : SqlParamSource
	{
		internal override void SubmitParameters(ISqlParamTarget target)
		{
		}
	}
}
