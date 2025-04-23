using System.Collections.ObjectModel;
using MuchAdo.Parameters;

namespace MuchAdo;

public static class DbParameterSource
{
	/// <summary>
	/// An empty list of parameters.
	/// </summary>
	public static readonly IDbParameterSource Empty = new EmptyDbParameterSource();

	/// <summary>
	/// Creates one parameter.
	/// </summary>
	public static SingleDbParameter<T> Create<T>(string name, T value) => new(name, value);

	/// <summary>
	/// Creates one parameter.
	/// </summary>
	public static SingleTypedDbParameter<T> Create<T>(string name, T value, IDbParameterType? type) => new(name, value, type);

	/// <summary>
	/// Creates parameters from a sequence of parameters.
	/// </summary>
	public static IDbParameterSource Create(params ReadOnlySpan<IDbParameterSource> parameters) =>
		parameters.Length switch
		{
			0 => Empty,
			1 => parameters[0],
			_ => new DbParameterSources(parameters),
		};

	/// <summary>
	/// Creates parameters from a sequence of parameters.
	/// </summary>
	public static IDbParameterSource Create(IEnumerable<IDbParameterSource> parameters) =>
		new DbParameterSources(parameters ?? throw new ArgumentNullException(nameof(parameters)));

	/// <summary>
	/// Creates parameters from a sequence of parameters.
	/// </summary>
	public static IDbParameterSource Create<T>(params ReadOnlySpan<(string Name, T Value)> parameters) =>
		parameters.Length switch
		{
			0 => Empty,
			1 => Create(parameters[0].Name, parameters[0].Value),
			_ => new TuplesDbParameterSource<T>(parameters.ToArray()),
		};

	/// <summary>
	/// Creates parameters from a sequence of name/value pairs.
	/// </summary>
	public static IDbParameterSource Create<T>(IEnumerable<(string Name, T Value)> parameters) =>
		new TuplesDbParameterSource<T>(parameters ?? throw new ArgumentNullException(nameof(parameters)));

	/// <summary>
	/// Creates parameters from a dictionary.
	/// </summary>
	public static IDbParameterSource Create<T>(IEnumerable<KeyValuePair<string, T>> parameters) =>
		new DictionaryDbParameterSource<T>(parameters ?? throw new ArgumentNullException(nameof(parameters)));

	/// <summary>
	/// Creates a list of parameters from the properties of a DTO.
	/// </summary>
	/// <remarks>The name of each parameter is the name of the corresponding DTO property.</remarks>
	public static IDbParameterSource FromDto<T>(T dto)
	{
		if (dto is null)
			throw new ArgumentNullException(nameof(dto));
		return new DtoDbParameterSource<T>(dto);
	}

	public static int Count(this IDbParameterSource source)
	{
		var target = new CountParameterTarget();
		source.SubmitParameters(target);
		return target.Count;
	}

	private sealed class CountParameterTarget : IDbParameterTarget
	{
		public int Count { get; private set; }

		public void AcceptParameter<T>(string name, T value, IDbParameterType? type) => Count++;
	}

	public static IEnumerable<(string Name, object? Value)> Enumerate(this IDbParameterSource source)
	{
		var target = new EnumerateParameterTarget();
		source.SubmitParameters(target);
		return target.Items;
	}

	private sealed class EnumerateParameterTarget : IDbParameterTarget
	{
		public Collection<(string Name, object? Value)> Items { get; } = new();

		public void AcceptParameter<T>(string name, T value, IDbParameterType? type) => Items.Add((name, value));
	}

	/// <summary>
	/// Filters the parameters by name.
	/// </summary>
	public static IDbParameterSource Where(this IDbParameterSource source, Func<string, bool> nameMatches)
	{
		if (nameMatches is null)
			throw new ArgumentNullException(nameof(nameMatches));
		return new FilteredDbParameterSource(source, nameMatches);
	}

	/// <summary>
	/// Transforms the parameter names using the specified function.
	/// </summary>
	public static IDbParameterSource Renamed(this IDbParameterSource source, Func<string, string> transform)
	{
		if (transform is null)
			throw new ArgumentNullException(nameof(transform));
		return new RenamedDbParameterSource(source, transform);
	}

	private sealed class EmptyDbParameterSource : IDbParameterSource
	{
		public void SubmitParameters(IDbParameterTarget target)
		{
		}
	}
}
