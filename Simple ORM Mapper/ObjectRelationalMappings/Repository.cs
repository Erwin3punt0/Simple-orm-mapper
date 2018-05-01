using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using ObjectRelationalMappings.Interfaces;

namespace ObjectRelationalMappings
{
    public abstract class Repository<T> where T : IDataStorageObject
    {
        private readonly string _connectionString;
        private readonly string _loadProcedure;
        private readonly string _saveProcedure;
        private readonly string _deleteProcedure;
        private readonly ILogger _logger;
        private readonly Mapper<T> _mapper;

        protected Repository(
            Mapper<T> mapper,
            string connectionString,
            string loadProcedure,
            string saveProcedure,
            string deleteProcedure,
            ILogger logger)
        {
            _mapper = mapper;
            _connectionString = connectionString;
            _loadProcedure = loadProcedure;
            _saveProcedure = saveProcedure;
            _deleteProcedure = deleteProcedure;
            _logger = logger;
        }

        protected bool Delete(ProcedureParameters parameters)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    DbCommand dbCommand = InitializeDbCommand(sqlConnection, _deleteProcedure, parameters);

                    sqlConnection.Open();

                    dbCommand.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Delete error", ex);
                throw;
            }
        }

        protected IList<T> Load(ProcedureParameters parameters)
        {
            List<T> list;

            DbDataReader rdr = null;

            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    DbCommand dbCommand = InitializeDbCommand(sqlConnection, _loadProcedure, parameters);

                    sqlConnection.Open();

                    rdr = dbCommand.ExecuteReader();
                    list = _mapper.CreatedDataStorageObjects(rdr);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Load list error", ex);
                throw;
            }
            finally
            {
                if (rdr != null && !rdr.IsClosed)
                {
                    rdr.Close();
                    rdr.Dispose();
                }
            }

            return list;
        }

        public bool Save(IList<T> dataStorageObjects)
        {
            DbCommand dbCommand = null;

            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();

                    foreach (var dataStorageObject in dataStorageObjects)
                    {
                        var parameters = _mapper.GetProcedureParameters(dataStorageObject);

                        dbCommand = InitializeDbCommand(sqlConnection, _saveProcedure, parameters);

                        dbCommand.Transaction = sqlConnection.BeginTransaction();
                        dbCommand.ExecuteNonQuery();
                        dbCommand.Transaction.Commit();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                dbCommand?.Transaction?.Rollback();
                _logger.Error("Save error", ex);
                throw;
            }
        }

        private static SqlCommand InitializeDbCommand(
            SqlConnection sqlConnection,
            string storedProcedure,
            ProcedureParameters parameters)
        {
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = storedProcedure;
            sqlCommand.CommandTimeout = 30;

            foreach (var parameter in parameters)
            {
                var dbParameter = sqlCommand.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value;
                sqlCommand.Parameters.Add(dbParameter);
            }

            return sqlCommand;
        }
    }
}
