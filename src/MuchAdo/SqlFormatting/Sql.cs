using System.Diagnostics.CodeAnalysis;
using MuchAdo.Parameters;

namespace MuchAdo.SqlFormatting;

/// <summary>
/// Encapsulates parameterized SQL.
/// </summary>
[SuppressMessage("Naming", "CA1724", Justification = "Conflicts with rarely-used System.Data.Sql namespace.")]
public abstract class Sql
{
	/// <summary>
	/// An empty SQL string.
	/// </summary>
	public static readonly Sql Empty = Raw("");

	/// <summary>
	/// Joins the specified SQL fragments with the AND operator.
	/// </summary>
	public static Sql And(params IEnumerable<Sql> sqls) => new AndOperatorSql(sqls.AsReadOnlyList());

	/// <summary>
	/// Joins the specified SQL fragments with newlines.
	/// </summary>
	public static Sql Clauses(params IEnumerable<Sql> sqls) => Join("\n", sqls);

	/// <summary>
	/// Returns a comma-delimited list of column names for a DTO of the specified type.
	/// </summary>
	public static ColumnNamesSql<T> ColumnNames<T>() => new();

	/// <summary>
	/// Returns a comma-delimited list of column names for a DTO of the specified type.
	/// </summary>
	public static ColumnNamesSql<T> ColumnNames<T>(T dto) => new();

	/// <summary>
	/// Returns a comma-delimited list of arbitrarily-named parameters for the column values of the specified DTO.
	/// </summary>
	public static ColumnParamsSql<T> ColumnParams<T>(T dto) => new(dto ?? throw new ArgumentNullException(nameof(dto)));

	/// <summary>
	/// Concatenates SQL fragments.
	/// </summary>
	public static Sql Concat(params IEnumerable<Sql> sqls) =>
		new ConcatSql((sqls ?? throw new ArgumentNullException(nameof(sqls))).AsReadOnlyList());

	/// <summary>
	/// Returns a comma-delimited list of named parameters for the properties of the specified DTO.
	/// </summary>
	public static DtoParamNamesSql<T> DtoParamNames<T>() => new();

	/// <summary>
	/// Returns a comma-delimited list of named parameters for the properties of the specified DTO.
	/// </summary>
	public static DtoParamNamesSql<T> DtoParamNames<T>(T dto) => new();

	public static SqlParamSource NamedDtoParams<T>(T dto) => new DtoSqlParamSource<T>(dto);

	/// <summary>
	/// Creates SQL from a formatted string.
	/// </summary>
	public static Sql Format(SqlFormatStringHandler stringHandler) => stringHandler.ToSql();

	/// <summary>
	/// Creates SQL for a GROUP BY clause. If the SQLs are empty, the GROUP BY clause is omitted.
	/// </summary>
	public static Sql GroupBy(params IEnumerable<Sql> sqls) => new GroupByClauseSql(Join(", ", sqls));

	/// <summary>
	/// Creates SQL for a HAVING clause. If the SQL is empty, the HAVING clause is omitted.
	/// </summary>
	public static Sql Having(Sql sql) => new HavingClauseSql(sql);

	/// <summary>
	/// Joins SQL fragments with the specified separator.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored.</remarks>
	public static Sql Join(string separator, params IEnumerable<Sql> sqls) =>
		new JoinSql(separator ?? throw new ArgumentNullException(nameof(separator)), (sqls ?? throw new ArgumentNullException(nameof(sqls))).AsReadOnlyList());

	/// <summary>
	/// Creates SQL for an arbitrarily-named parameter with the specified fragment of a LIKE pattern followed by a trailing <c>%</c>.
	/// </summary>
	/// <remarks>This SQL fragment escapes <c>%</c> and <c>_</c> in the prefix with <c>\</c>. Depending on the database
	/// and its settings, <c>escape '\'</c> may be needed after the parameter.</remarks>
	public static Sql LikeParamStartsWith(string prefix) => new LikeParamStartsWithSql(prefix ?? throw new ArgumentNullException(nameof(prefix)));

	/// <summary>
	/// Creates SQL for a comma-delimited list of SQL fragments.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored. Since it would otherwise result in a confusing SQL syntax error, an <see cref="InvalidOperationException" />
	/// is thrown if the SQL fragments are missing or all empty. Use <c>Sql.Join(", ", sqls)</c> to permit an empty SQL fragment.</remarks>
	public static Sql List(params IEnumerable<Sql> sqls) => JoinOrThrow(", ", sqls, "Sql.List was empty.");

	/// <summary>
	/// Creates SQL for a quoted identifier.
	/// </summary>
	public static Sql Name(string identifier) => new NameSql(identifier ?? throw new ArgumentNullException(nameof(identifier)));

	/// <summary>
	/// Joins the specified SQL fragments with the OR operator.
	/// </summary>
	public static Sql Or(params IEnumerable<Sql> sqls) => new OrOperatorSql(sqls.AsReadOnlyList());

	/// <summary>
	/// Creates SQL for an ORDER BY clause. If the SQLs are empty, the ORDER BY clause is omitted.
	/// </summary>
	public static Sql OrderBy(params IEnumerable<Sql> sqls) => new OrderByClauseSql(Join(", ", sqls));

	/// <summary>
	/// Creates SQL for an arbitrarily-named parameter with the specified value.
	/// </summary>
	public static SqlParam<T> Param<T>(T value) => value is not Sql ? new SqlParam<T>(value) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates SQL for an arbitrarily-named parameter with the specified value.
	/// </summary>
	public static SqlParam<T> Param<T>(T value, SqlParamType? type) => value is not Sql ? new TypedSqlParam<T>(value, type) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates SQL for a named parameter with the specified value.
	/// </summary>
	public static SqlParam<T> NamedParam<T>(string name, T value) => value is not Sql ? new NamedSqlParam<T>(name, value) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates SQL for a named parameter with the specified value.
	/// </summary>
	public static SqlParam<T> NamedParam<T>(string name, T value, SqlParamType? type) => value is not Sql ? new NamedTypedSqlParam<T>(name, value, type) : throw new ArgumentException(c_paramIsSqlMessage, nameof(value));

	/// <summary>
	/// Creates SQL for a comma-delimted list of arbitrarily-named parameters with the specified values.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored. Since it would otherwise result in a confusing SQL syntax error, an <see cref="InvalidOperationException" />
	/// is thrown if the collection of values is empty. Use <c>Sql.Join(", ", values.Select(Sql.Param))")</c> to allow an empty collection.</remarks>
	public static Sql ParamList<T>(IEnumerable<T> values) => JoinOrThrow(", ", values.Select(Param), "Sql.ParamList was empty.");

	/// <summary>
	/// Creates SQL for a comma-delimted list of arbitrarily-named parameters with the specified values, surrounded by parentheses.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored. Since it would otherwise result in a confusing SQL syntax error, an <see cref="InvalidOperationException" />
	/// is thrown if the collection of values is empty. Use <c>Sql.Format($"({Sql.Join(", ", values.Select(Sql.Param))})")</c> to permit an empty tuple.</remarks>
	public static Sql ParamTuple<T>(IEnumerable<T> values) => Format($"({JoinOrThrow(", ", values.Select(Param), "Sql.ParamTuple was empty.")})");

	/// <summary>
	/// Creates SQL from a raw string.
	/// </summary>
	public static Sql Raw(string text) => new RawSql(text ?? throw new ArgumentNullException(nameof(text)));

	/// <summary>
	/// Creates SQL for a comma-delimited list of SQL fragments, surrounded by parentheses.
	/// </summary>
	/// <remarks>Empty SQL fragments are ignored. Since it would otherwise result in a confusing SQL syntax error, an <see cref="InvalidOperationException" />
	/// is thrown if the SQL fragments are missing or all empty. Use <c>Sql.Format($"({Sql.Join(", ", sqls)})")</c> to permit an empty tuple.</remarks>
	public static Sql Tuple(params IEnumerable<Sql> sqls) => Format($"({JoinOrThrow(", ", sqls, "Sql.Tuple was empty.")})");

	/// <summary>
	/// Creates SQL for a WHERE clause. If the SQL is empty, the WHERE clause is omitted.
	/// </summary>
	public static Sql Where(Sql sql) => new WhereClauseSql(sql);

	/// <summary>
	/// Concatenates two SQL fragments.
	/// </summary>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use Concat.")]
	public static Sql operator +(Sql a, Sql b) => new AddSql(a ?? throw new ArgumentNullException(nameof(a)), b ?? throw new ArgumentNullException(nameof(b)));

	/// <inheritdoc />
	public override string ToString() => ToString(SqlSyntax.Ansi);

	public string ToString(SqlSyntax syntax)
	{
		var commandBuilder = new DbConnectorCommandBuilder(syntax, buildText: true, parameterTarget: null);
		Render(commandBuilder);
		return commandBuilder.GetText();
	}

	internal abstract void Render(DbConnectorCommandBuilder builder);

	private static JoinSql JoinOrThrow(string separator, IEnumerable<Sql> sqls, string throwMessageIfEmpty) =>
		new(separator ?? throw new ArgumentNullException(nameof(separator)), (sqls ?? throw new ArgumentNullException(nameof(sqls))).AsReadOnlyList(), throwMessageIfEmpty);

	private const string c_paramIsSqlMessage = "Parameters may not be created from Sql instances.";
}
