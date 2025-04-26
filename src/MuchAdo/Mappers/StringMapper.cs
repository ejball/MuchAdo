using System.Data;

namespace MuchAdo.Mappers;

internal sealed class StringMapper : SingleFieldReferenceMapper<string>
{
	public override string MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => record.GetString(index);
}
