namespace MuchAdo;

/// <summary>
/// Settings when creating a <see cref="DefaultDbTypeMapperFactory"/>.
/// </summary>
public sealed class DbDataMapperSettings
{
	public static DbDataMapperSettings Default { get; } = new() { TypeMapperFactories = [new DefaultDbTypeMapperFactory()] };

	/// <summary>
	/// The type mapper factories used by this data mapper.
	/// </summary>
	public IReadOnlyList<DbTypeMapperFactory> TypeMapperFactories { get; private init; }

	public DbDataMapperSettings WithTypeMapperFactory(DbTypeMapperFactory typeMapperFactory) =>
		ReplaceTypeMapperFactories([.. TypeMapperFactories, typeMapperFactory]);

	public DbDataMapperSettings ReplaceTypeMapperFactories(IReadOnlyList<DbTypeMapperFactory> value) =>
		new(this) { TypeMapperFactories = value };

	/// <summary>
	/// True to allow strings to be mapped to enums.
	/// </summary>
	public bool AllowStringToEnum { get; private init; }

	public DbDataMapperSettings WithAllowStringToEnum(bool value = true) =>
		new(this) { AllowStringToEnum = value };

	/// <summary>
	/// True to ignore unused fields.
	/// </summary>
	public bool IgnoreUnusedFields { get; private init; }

	public DbDataMapperSettings WithIgnoreUnusedFields(bool value = true) =>
		new(this) { IgnoreUnusedFields = value };

	private DbDataMapperSettings()
	{
		TypeMapperFactories = [];
		AllowStringToEnum = false;
		IgnoreUnusedFields = false;
	}

	private DbDataMapperSettings(DbDataMapperSettings source)
	{
		TypeMapperFactories = source.TypeMapperFactories;
		AllowStringToEnum = source.AllowStringToEnum;
		IgnoreUnusedFields = source.IgnoreUnusedFields;
	}
}
