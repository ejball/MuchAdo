using System.Data;

namespace MuchAdo.Mappers;

public abstract class SingleFieldMapper<T>(DbDataMapper dataMapper) : DbTypeMapper<T>
{
	public override int? FieldCount => 1;

	protected sealed override T MapCore(IDataRecord record, int index, int count, DbConnectorRecordState? state) =>
		(dataMapper.Settings.IgnoreUnusedFields ? count >= 1 : count == 1) ? MapField(record, index, state) : throw BadFieldCount(count);

	protected abstract T MapField(IDataRecord record, int index, DbConnectorRecordState? state);
}
