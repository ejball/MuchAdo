using System.Data;

namespace MuchAdo.Mappers;

internal sealed class FloatMapper : SingleFieldValueMapper<float>
{
	public override float MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => record.GetFloat(index);
}
