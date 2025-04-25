using System.Data;
using System.Dynamic;

namespace MuchAdo.Mappers;

internal sealed class ObjectMapper(DbDataMapper dataMapper) : DbTypeMapper<object>
{
	public override int? FieldCount => null;

	protected override object MapCore(IDataRecord record, int index, int count, DbConnectorRecordState? state)
	{
		if (count == 1)
		{
			var value = record.GetValue(index);
			return value == DBNull.Value ? null! : value;
		}

		IDictionary<string, object?> obj = new ExpandoObject();
		var notNull = false;
		for (var i = index; i < index + count; i++)
		{
			var name = record.GetName(i);
			if (!record.IsDBNull(i))
			{
				obj[name] = dataMapper.GetTypeMapper<object>().Map(record, i, state);
				notNull = true;
			}
			else
			{
				obj[name] = null;
			}
		}
		return notNull ? obj : null!;
	}
}
