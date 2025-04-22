using System.Data;

namespace MuchAdo.Mappers;

internal sealed class DoubleMapper : SingleFieldValueMapper<double>
{
	public override double MapNotNullField(IDataRecord record, int index) => record.GetDouble(index);
}
