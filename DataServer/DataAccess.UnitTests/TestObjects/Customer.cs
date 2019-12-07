
using System;

namespace DataAccess.UnitTests.TestObjects
{
    public class Customer : ILiteDbRow
    {
        public string Name { get; set; }
        public Guid CustomerId { get; set; }
        public int Count { get; set; }
        public int Id { get; set; }
    }
}
