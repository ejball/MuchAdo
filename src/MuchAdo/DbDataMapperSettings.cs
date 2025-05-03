namespace MuchAdo;

/// <summary>
/// Settings when creating a <see cref="DefaultDbTypeMapperFactory"/>.
/// </summary>
public sealed class DbDataMapperSettings
{
	/// <summary>
	/// Empty settings.
	/// </summary>
	public static DbDataMapperSettings Empty { get; } = new();

	/// <summary>
	/// The type mapper factories used by this data mapper.
	/// </summary>
	public IReadOnlyList<DbTypeMapperFactory> TypeMapperFactories { get; private init; }

	/// <summary>
	/// Returns new settings with an additional type mapper factory.
	/// </summary>
	public DbDataMapperSettings WithTypeMapperFactory(DbTypeMapperFactory typeMapperFactory) =>
		ReplaceTypeMapperFactories([.. TypeMapperFactories, typeMapperFactory]);

	/// <summary>
	/// Returns new settings with the specified type mapper factories.
	/// </summary>
	public DbDataMapperSettings ReplaceTypeMapperFactories(IReadOnlyList<DbTypeMapperFactory> value) =>
		new(this) { TypeMapperFactories = value };

	/// <summary>
	/// True to allow strings to be mapped to enums.
	/// </summary>
	public bool AllowStringToEnum { get; private init; }

	/// <summary>
	/// Returns new settings with the specified value for <see cref="AllowStringToEnum"/>.
	/// </summary>
	public DbDataMapperSettings WithAllowStringToEnum(bool value = true) =>
		new(this) { AllowStringToEnum = value };

	/// <summary>
	/// True to ignore unused fields.
	/// </summary>
	public bool IgnoreUnusedFields { get; private init; }

	/// <summary>
	/// Returns new settings with the specified value for <see cref="IgnoreUnusedFields"/>.
	/// </summary>
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
