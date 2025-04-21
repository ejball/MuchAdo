using System.Data;
using System.Data.Common;

namespace MuchAdo.Mappers;

internal sealed class FieldValueStructMapper<T> : NonNullableValueMapper<T>
	where T : struct
{
	public override T MapNotNullField(IDataRecord record, int index) =>
		(record as DbDataReader ?? throw new InvalidOperationException("Record must be a DbDataRecord.")).GetFieldValue<T>(index);
}
