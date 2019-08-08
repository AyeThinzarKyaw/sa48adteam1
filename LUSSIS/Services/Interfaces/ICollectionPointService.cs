using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface ICollectionPointService
    {

        Employee GetClerkByCollectionPointId(int cpId);

        IEnumerable<Employee> GetAllClerks();

        IEnumerable<CollectionPoint> GetAllCollectionPoints();

        CollectionPoint GetCollectionPointById(int id);

        void UpdateCollectionPoint(CollectionPoint collectionPoint);

        CollectionPoint GetDepartmentCollectionPointByEmployeeId(int employeeId);

        Department GetDepartmentByEmployeeId(int employeeId);

        void UpdateDepartmentCollectionPoint(Department department);
    }
}
