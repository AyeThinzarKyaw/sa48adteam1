using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class ClerkCollectionPointDTO
    {
        public CollectionPoint CollectionPoint1 { get; set; }

        public CollectionPoint CollectionPoint2 { get; set; }

        public CollectionPoint CollectionPoint3 { get; set; }

        public CollectionPoint CollectionPoint4 { get; set; }

        public CollectionPoint CollectionPoint5 { get; set; }

        public CollectionPoint CollectionPoint6 { get; set; }

        public int Employee1 { get; set; }

        public int Employee2 { get; set; }

        public int Employee3 { get; set; }

        public int Employee4 { get; set; }

        public int Employee5 { get; set; }

        public int Employee6 { get; set; }

        public IEnumerable<Employee> Clerks { get; set; }

        public IEnumerable<CollectionPoint> CollectionPoints { get; set; }
    }
}