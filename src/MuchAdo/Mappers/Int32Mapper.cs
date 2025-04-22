using System.Data;

namespace MuchAdo.Mappers;

internal sealed class Int32Mapper : SingleFieldValueMapper<int>
{
	public override int MapNotNullField(IDataRecord record, int index) => record.GetInt32(index);
}
