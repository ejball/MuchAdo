using System.Data;

namespace MuchAdo.Mappers;

internal sealed class ByteMapper : SingleFieldValueMapper<byte>
{
	public override byte MapNotNullField(IDataRecord record, int index) => record.GetByte(index);
}
