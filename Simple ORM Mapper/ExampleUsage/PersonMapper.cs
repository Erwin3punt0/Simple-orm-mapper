using System;
using System.Collections.Generic;
using System.Data.Common;
using ExampleUsage.Dto;
using ObjectRelationalMappings;

namespace ExampleUsage
{
    public class PersonMapper : Mapper<Person>
    {
        public override List<Person> CreatedDataStorageObjects(DbDataReader rdr)
        {
            var persons = new List<Person>();

            while (rdr.Read())
            {
                persons.Add(new Person
                {
                    Id = rdr.GetGuid(rdr.GetOrdinal("Id")),
                    Name = rdr.IsDBNull(rdr.GetOrdinal("Name")) ?
                        null :
                        rdr.GetString(rdr.GetOrdinal("Name"))
                });
            }

            return persons;
        }

        public override ProcedureParameters GetProcedureParameters(Person person)
        {
            var personId = person.Id ?? Guid.NewGuid();

            return new ProcedureParameters
            {
                {"@id", personId },
                {"@name", person.Name },
            };
        }
    }
}
