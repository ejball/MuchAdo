using System.Data;

namespace MuchAdo.Mappers;

internal sealed class DecimalMapper : SingleFieldValueMapper<decimal>
{
	public override decimal MapNotNullField(IDataRecord record, int index) => record.GetDecimal(index);
}
