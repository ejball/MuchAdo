using System.Data;

namespace MuchAdo.Mappers;

internal sealed class GuidMapper : SingleFieldValueMapper<Guid>
{
	public override Guid MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => record.GetGuid(index);
}
