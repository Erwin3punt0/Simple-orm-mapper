using System.Collections.Generic;

namespace ObjectRelationalMappings
{
    public class ProcedureParameters : Dictionary<string, object>
    {
        public ProcedureParameters(params object[] id)
        {
            for (var i = id.GetLowerBound(0); i <= id.GetUpperBound(0); ++i)
                Add($"ID{i}", id[i]);
        }
    }
}
