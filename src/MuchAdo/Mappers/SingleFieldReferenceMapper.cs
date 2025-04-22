using System.Data;

namespace MuchAdo.Mappers;

public abstract class SingleFieldReferenceMapper<T> : SingleFieldMapper<T?>
	where T : class
{
	protected sealed override T? MapField(IDataRecord record, int index) =>
		!record.IsDBNull(index) ? MapNotNullField(record, index) : null;

	public abstract T MapNotNullField(IDataRecord record, int index);
}
