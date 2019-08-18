using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class DepartmentCollectionPointDTO
    {
        public CollectionPoint DepartmentCollectionPoint { get; set; }

        public CollectionPoint CollectionPoint1 { get; set; }

        public CollectionPoint CollectionPoint2 { get; set; }

        public CollectionPoint CollectionPoint3 { get; set; }

        public CollectionPoint CollectionPoint4 { get; set; }

        public CollectionPoint CollectionPoint5 { get; set; }

        public CollectionPoint CollectionPoint6 { get; set; }

        public int DepartmentCollectionPointId { get; set; }

        public IEnumerable<CollectionPoint> CollectionPoints { get; set; }
    }
}