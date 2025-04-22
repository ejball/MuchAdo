using System.Data;

namespace MuchAdo.Mappers;

internal sealed class DateTimeMapper : SingleFieldValueMapper<DateTime>
{
	public override DateTime MapNotNullField(IDataRecord record, int index) => record.GetDateTime(index);
}
