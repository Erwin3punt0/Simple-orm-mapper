using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ExampleUsage.Dto;
using ObjectRelationalMappings;
using ObjectRelationalMappings.Interfaces;

namespace ExampleUsage
{
    public class PersonRepository : Repository<Person>
    {
        public IList<Person> Find(string name)
        {
            return Load(new ProcedureParameters { { "@name", name } });
        }

        public Person Get(Guid id)
        {
            return Load(new ProcedureParameters { { "@id", id } }).FirstOrDefault();
        }

        public PersonRepository(
            ILogger logger) 
            : base(
                new PersonMapper(),
                ConfigurationManager.ConnectionStrings["PersonDB"].ConnectionString,
                "up_Load_Person",
                "up_Save_Person",
                "up_Delete_Person",
                logger)
        {
        }
    }
}
