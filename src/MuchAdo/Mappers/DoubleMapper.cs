using System.Data;

namespace MuchAdo.Mappers;

internal sealed class DoubleMapper : SingleFieldValueMapper<double>
{
	public override double MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => record.GetDouble(index);
}
