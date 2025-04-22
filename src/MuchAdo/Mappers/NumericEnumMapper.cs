using System.Data;

namespace MuchAdo.Mappers;

internal sealed class NumericEnumMapper<TEnum, TUnderlyingType>(DbTypeMapper<TUnderlyingType> underlyingTypeMapper) : SingleFieldValueMapper<TEnum>
	where TEnum : struct
{
	public override TEnum MapNotNullField(IDataRecord record, int index) =>
		(TEnum) (object) underlyingTypeMapper.Map(record, index, state: null)!;
}
