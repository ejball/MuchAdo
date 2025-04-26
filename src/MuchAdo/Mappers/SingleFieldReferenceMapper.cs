using System.Data;

namespace MuchAdo.Mappers;

public abstract class SingleFieldReferenceMapper<T> : SingleFieldMapper<T?>
	where T : class
{
	protected sealed override T? MapField(IDataRecord record, int index, DbConnectorRecordState? state) =>
		!record.IsDBNull(index) ? MapNotNullField(record, index, state) : null;

	public abstract T MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state);
}
