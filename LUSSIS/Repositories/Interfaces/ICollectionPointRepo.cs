using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface ICollectionPointRepo : IGenericRepo<CollectionPoint, int>
    {
        CollectionPoint GetDepartmentCollectionPointByEmployeeId(int id);
    }
}