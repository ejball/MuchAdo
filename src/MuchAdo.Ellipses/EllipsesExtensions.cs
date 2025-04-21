using System.Collections;
using System.Text.RegularExpressions;

namespace MuchAdo.Ellipses;

/// <summary>
/// Methods for expanding ellipses in SQL.
/// </summary>
public static class EllipsesExtensions
{
	public static DbConnectorCommandBatch ExpandEllipses(this DbConnectorCommandBatch commandBatch)
	{
		for (var commandIndex = 0; commandIndex < commandBatch.CommandCount; commandIndex++)
		{
			var command = commandBatch.GetCommand(commandIndex);
			var commandText = command.Text ?? command.Sql!.ToString(commandBatch.Connector.SqlSyntax);
			var parameters = command.Parameters;

			if (commandText.ContainsOrdinal("..."))
			{
				var nameValuePairs = parameters.Enumerate().ToList();
				var parameterIndex = 0;
				while (parameterIndex < nameValuePairs.Count)
				{
					// look for @name... in SQL for collection parameters
					var (name, value) = nameValuePairs[parameterIndex];
					if (!string.IsNullOrEmpty(name) && value is not string && value is not byte[] && value is IEnumerable list)
					{
						var itemCount = -1;
						var replacements = new List<(string Name, object? Value)>();

						string Replacement(Match match)
						{
							if (itemCount == -1)
							{
								itemCount = 0;

								foreach (var item in list)
								{
									replacements.Add(($"{name}_{itemCount}", item));
									itemCount++;
								}

								if (itemCount == 0)
									throw new InvalidOperationException($"Collection parameter '{name}' must not be empty.");
							}

							return string.Join(",", Enumerable.Range(0, itemCount).Select(x => $"{match.Groups[1]}_{x}"));
						}

						commandText = Regex.Replace(commandText, $@"([?@:]{Regex.Escape(name)})\.\.\.",
							Replacement, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

						// if special syntax wasn't found, leave the parameter alone, for databases that support collections directly
						if (itemCount != -1)
						{
							parameters = DbParameterSource.Create(nameValuePairs.Take(parameterIndex).Concat(replacements).Concat(nameValuePairs.Skip(parameterIndex + 1)));
							parameterIndex += replacements.Count;
						}
						else
						{
							parameterIndex += 1;
						}
					}
					else
					{
						parameterIndex += 1;
					}
				}

				commandBatch.SetCommand(commandIndex, new(command.Type, commandText, parameters));
			}
		}

		return commandBatch;
	}
}
