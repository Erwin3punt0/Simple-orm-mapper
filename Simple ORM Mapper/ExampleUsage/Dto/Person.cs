
using System;
using ObjectRelationalMappings.Interfaces;

namespace ExampleUsage.Dto
{
    public class Person : IDataStorageObject
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }
}
