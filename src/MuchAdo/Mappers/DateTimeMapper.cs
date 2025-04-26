using System.Data;

namespace MuchAdo.Mappers;

internal sealed class DateTimeMapper : SingleFieldValueMapper<DateTime>
{
	public override DateTime MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => record.GetDateTime(index);
}
