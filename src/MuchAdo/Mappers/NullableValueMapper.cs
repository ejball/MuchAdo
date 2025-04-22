using System.Data;

namespace MuchAdo.Mappers;

internal sealed class NullableValueMapper<T>(SingleFieldValueMapper<T> mapper) : SingleFieldMapper<T?>
	where T : struct
{
	protected override T? MapField(IDataRecord record, int index) =>
		!record.IsDBNull(index) ? mapper.MapNotNullField(record, index) : null;
}
