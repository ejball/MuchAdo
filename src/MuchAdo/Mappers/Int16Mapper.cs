using System.Data;

namespace MuchAdo.Mappers;

internal sealed class Int16Mapper : SingleFieldValueMapper<short>
{
	public override short MapNotNullField(IDataRecord record, int index) => record.GetInt16(index);
}
