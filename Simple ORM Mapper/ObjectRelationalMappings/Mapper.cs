using System;
using System.Collections.Generic;
using System.Data.Common;
using ObjectRelationalMappings.Interfaces;

namespace ObjectRelationalMappings
{
    public abstract class Mapper<T> where T : IDataStorageObject
    {
        public abstract List<T> CreatedDataStorageObjects(DbDataReader rdr);
        public abstract ProcedureParameters GetProcedureParameters(T dataStorageObject);
    }
}
