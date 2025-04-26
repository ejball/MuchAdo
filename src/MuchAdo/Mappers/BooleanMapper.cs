using System.Data;

namespace MuchAdo.Mappers;

internal sealed class BooleanMapper : SingleFieldValueMapper<bool>
{
	public override bool MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => record.GetBoolean(index);
}
