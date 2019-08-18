using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class CollectionPointRepo : GenericRepo<CollectionPoint, int> , ICollectionPointRepo
    {
        private CollectionPointRepo() { }

        private static CollectionPointRepo instance = new CollectionPointRepo();

        public static ICollectionPointRepo Instance
        {
            get { return instance; }
        }

        public CollectionPoint GetDepartmentCollectionPointByEmployeeId(int id)
        {
            var result = from cp in Context.CollectionPoints
                         join d in Context.Departments on cp.Id equals d.CollectionPointId
                         join e in Context.Employees on d.Id equals e.DepartmentId
                         where e.Id == id
                         select cp;
            return result.SingleOrDefault();
        }
    }
}