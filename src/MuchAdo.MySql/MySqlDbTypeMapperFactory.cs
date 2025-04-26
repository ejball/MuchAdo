using System.Data;
using MuchAdo.Mappers;
using MySqlConnector;

namespace MuchAdo.MySql;

internal sealed class MySqlDbTypeMapperFactory : DbTypeMapperFactory
{
	public override DbTypeMapper<T>? TryCreateTypeMapper<T>(DbDataMapper dataMapper)
	{
		if (typeof(T) == typeof(MySqlDateTime))
			return (DbTypeMapper<T>) (object) new MySqlDateTimeMapper();
		if (typeof(T) == typeof(MySqlDecimal))
			return (DbTypeMapper<T>) (object) new MySqlDecimalMapper();
		if (typeof(T) == typeof(MySqlGeometry))
			return (DbTypeMapper<T>) (object) new MySqlGeometryMapper();

		return null;
	}

	private sealed class MySqlDateTimeMapper : SingleFieldValueMapper<MySqlDateTime>
	{
		public override MySqlDateTime MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => ((MySqlDataReader) record).GetMySqlDateTime(index);
	}

	private sealed class MySqlDecimalMapper : SingleFieldValueMapper<MySqlDecimal>
	{
		public override MySqlDecimal MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => ((MySqlDataReader) record).GetMySqlDecimal(index);
	}

	private sealed class MySqlGeometryMapper : SingleFieldReferenceMapper<MySqlGeometry>
	{
		public override MySqlGeometry MapNotNullField(IDataRecord record, int index, DbConnectorRecordState? state) => ((MySqlDataReader) record).GetMySqlGeometry(index);
	}
}
