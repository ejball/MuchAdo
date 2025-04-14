using System.Linq.Expressions;
using System.Reflection;

namespace MuchAdo;

internal sealed class DbDtoProperty<T>
{
	public DbDtoProperty(PropertyInfo propertyInfo, string? columnName)
	{
		MemberInfo = propertyInfo;
		Name = propertyInfo.Name;
		ValueType = propertyInfo.PropertyType;
		IsReadOnly = propertyInfo.SetMethod?.IsPublic is not true;
		ColumnName = columnName;
		m_lazySubmitParameter = new(CreateSubmitParameter);
	}

	public DbDtoProperty(FieldInfo fieldInfo, string? columnName)
	{
		MemberInfo = fieldInfo;
		Name = fieldInfo.Name;
		ValueType = fieldInfo.FieldType;
		IsReadOnly = fieldInfo.IsInitOnly;
		ColumnName = columnName;
		m_lazySubmitParameter = new(CreateSubmitParameter);
	}

	public MemberInfo MemberInfo { get; }

	public string Name { get; }

	public Type ValueType { get; }

	public bool IsReadOnly { get; }

	public string? ColumnName { get; }

	public void SubmitParameter(IDbParameterTarget target, string name, T source) => m_lazySubmitParameter.Value(target, name, source);

	private Action<IDbParameterTarget, string, T> CreateSubmitParameter()
	{
		var targetParam = Expression.Parameter(typeof(IDbParameterTarget), "target");
		var nameParam = Expression.Parameter(typeof(string), "name");
		var sourceParam = Expression.Parameter(typeof(T), "source");

		var getValue = MemberInfo is PropertyInfo propertyInfo
			? Expression.Property(sourceParam, propertyInfo)
			: Expression.Field(sourceParam, (FieldInfo) MemberInfo);

		var acceptMethod = typeof(IDbParameterTarget)
			.GetMethods(BindingFlags.Public | BindingFlags.Instance)
			.Single(x => x is { Name: "AcceptParameter", IsGenericMethod: true } &&
				x.GetGenericArguments().Length == 1 &&
				x.GetParameters() is [var p0, var p1] &&
				p0.ParameterType == typeof(string) &&
				p1.ParameterType.IsGenericParameter).MakeGenericMethod(ValueType);

		return Expression.Lambda<Action<IDbParameterTarget, string, T>>(
			Expression.Call(targetParam, acceptMethod, nameParam, getValue), targetParam, nameParam, sourceParam).Compile();
	}

	private readonly Lazy<Action<IDbParameterTarget, string, T>> m_lazySubmitParameter;
}
