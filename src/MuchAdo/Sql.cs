using System.Diagnostics.CodeAnalysis;
using MuchAdo.Sources;

namespace MuchAdo;

/// <summary>
/// Encapsulates parameterized SQL.
/// </summary>
[SuppressMessage("Naming", "CA1724", Justification = "Conflicts with rarely-used System.Data.Sql namespace.")]
public static class Sql
{
	/// <summary>
	/// Empty SQL/parameters.
	/// </summary>
	public static readonly SqlParamSource Empty = new EmptySqlParamSource();

	/// <summary>
	/// Joins the specified SQL fragments with the AND operator.
	/// </summary>
	public static SqlSource And(params IEnumerable<SqlSource> sqls) => new AndOperatorSqlSource(sqls.Memoize());

	/// <summary>
	/// Joins the specified SQL fragments with newlines.
	/// </summary>
	public static SqlSource Clauses(params IEnumerable<SqlSource> sqls) => new ClausesSqlSource(sqls.Memoize());

	/// <summary>
	/// Returns a comma-separated list of column names for a DTO of the specified type.
	/// </summary>
	public static ColumnNamesSqlSource<T> ColumnNames<T>() => new();

	/// <summary>
	/// Returns a comma-separated list of column names for a DTO of the specified type.
	/// </summary>
	public static ColumnNamesSqlSource<T> ColumnNames<T>(T dto) => new();

	/// <summary>
	/// Returns a comma-separated list of unnamed parameters for the column values of the specified DTO.
	/// </summary>
	public static ColumnParamsSqlSource<T> ColumnParams<T>(T dto) => new(dto ?? throw new ArgumentNullException(nameof(dto)));

	/// <summary>
	/// Concatenates SQL fragments.
	/// </summary>
	public static SqlSource Concat(params IEnumerable<SqlSource> sqls) =>
		new ConcatSqlSource(sqls.Memoize());

	public static SqlParamSource DtoNamedParams<T>(T dto) => new DtoNamedSqlParamSource<T>(dto);

	/// <summary>
	/// Returns a comma-separated list of named parameters for the properties of the specified DTO.
	/// </summary>
	public static DtoParamNamesSqlSource<T> DtoParamNames<T>() => new();

	/// <summary>
	/// Returns a comma-separated list of named parameters for the properties of the specified DTO.
	/// </summary>
	public static DtoParamNamesSqlSource<T> DtoParamNames<T>(T dto) => new();

	/// <summary>
	/// Creates SQL from a formatted string.
	/// </summary>
	public static SqlSource Format(SqlFormatStringHandler stringHandler) => stringHandler.ToSqlSource();

	/// <summary>
	/// Creates SQL for a GROUP BY clause. If the SQLs are empty, the GROUP BY clause is omitted.
	/// </summary>
	public static SqlSource GroupBy(params IEnumerable<SqlSource> sqls) => new GroupByClauseSqlSource(List(sqls));

	/// <summary>
	/// Creates SQL for a HAVING clause. If the SQL is empty, the HAVING clause is omitted.
	/// </summary>
	public static SqlSource Having(SqlSource sql) => new HavingClauseSqlSource(sql);

	/// <summary>
	/// Joins SQL fragments with the specified separator.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored.</remarks>
	public static SqlSource Join(string separator, params IEnumerable<SqlSource> sqls) =>
		new JoinSqlSource(separator ?? throw new ArgumentNullException(nameof(separator)), sqls.Memoize());

	/// <summary>
	/// Creates SQL for an unnamed parameter with the specified fragment of a LIKE pattern followed by a trailing <c>%</c>.
	/// </summary>
	/// <remarks>This SQL fragment escapes <c>%</c> and <c>_</c> in the prefix with <c>\</c>. Depending on the database
	/// and its settings, <c>escape '\'</c> may be needed after the parameter.</remarks>
	public static SqlSource LikeParamStartsWith(string prefix) => new LikeParamStartsWithSqlSource(prefix ?? throw new ArgumentNullException(nameof(prefix)));

	/// <summary>
	/// Creates SQL for a comma-separated list of SQL fragments.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored.</remarks>
	public static SqlSource List(params IEnumerable<SqlSource> sqls) => new ListSqlSource(sqls.Memoize());

	/// <summary>
	/// Creates SQL for a quoted identifier.
	/// </summary>
	public static SqlSource Name(string identifier) => new NameSqlSource(identifier ?? throw new ArgumentNullException(nameof(identifier)));

	/// <summary>
	/// Creates SQL for a named parameter with the specified value.
	/// </summary>
	public static SqlParam<T> NamedParam<T>(string name, T value) => value is not SqlSource ? new NamedSqlParam<T>(name, value) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates SQL for a named parameter with the specified value.
	/// </summary>
	public static SqlParam<T> NamedParam<T>(string name, T value, SqlParamType? type) => value is not SqlSource ? new NamedTypedSqlParam<T>(name, value, type) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates parameters from tuples.
	/// </summary>
	public static SqlParamSource NamedParams<T>(params IEnumerable<(string Name, T Value)> parameters) =>
		new TuplesSqlParamSource<T>(parameters.Memoize());

	/// <summary>
	/// Creates parameters from a dictionary.
	/// </summary>
	public static SqlParamSource NamedParams<T>(IEnumerable<KeyValuePair<string, T>> parameters) =>
		new DictionarySqlParamSource<T>(parameters.Memoize());

	/// <summary>
	/// Joins the specified SQL fragments with the OR operator.
	/// </summary>
	public static SqlSource Or(params IEnumerable<SqlSource> sqls) => new OrOperatorSqlSource(sqls.Memoize());

	/// <summary>
	/// Creates SQL for an ORDER BY clause. If the SQLs are empty, the ORDER BY clause is omitted.
	/// </summary>
	public static SqlSource OrderBy(params IEnumerable<SqlSource> sqls) => new OrderByClauseSqlSource(List(sqls));

	/// <summary>
	/// Creates SQL for an unnamed parameter with the specified value.
	/// </summary>
	public static SqlParam<T> Param<T>(T value) => value is not SqlSource ? new SqlParam<T>(value) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates SQL for an unnamed parameter with the specified value.
	/// </summary>
	public static SqlParam<T> Param<T>(T value, SqlParamType? type) => value is not SqlSource ? new TypedSqlParam<T>(value, type) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates SQL for a comma-separated list of unnamed parameters with the specified values.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored.</remarks>
	public static SqlParamSource Params<T>(T value1, T value2, params ReadOnlySpan<T> values) => new ParamsSqlParamSource<T>([value1, value2, .. values]);

	/// <summary>
	/// Creates SQL for a comma-separated list of unnamed parameters with the specified values.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored.</remarks>
	public static SqlParamSource Params<T>(IEnumerable<T> values) => new ParamsSqlParamSource<T>(values.Memoize());

	/// <summary>
	/// Creates SQL from a raw string.
	/// </summary>
	public static SqlSource Raw(string text) => new RawSqlSource(text ?? throw new ArgumentNullException(nameof(text)));

	/// <summary>
	/// Creates SQL for a comma-separated list of SQL fragments, surrounded by parentheses.
	/// </summary>
	public static SqlSource Tuple(params IEnumerable<SqlSource> sqls) => Format($"({List(sqls)})");

	/// <summary>
	/// Creates SQL for a WHERE clause. If the SQL is empty, the WHERE clause is omitted.
	/// </summary>
	public static SqlSource Where(SqlSource sql) => new WhereClauseSqlSource(sql);

	private const string c_paramIsSqlMessage = "Parameters may not be created from Sql instances.";
}
