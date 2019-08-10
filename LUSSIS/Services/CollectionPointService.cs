using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class CollectionPointService : ICollectionPointService
    {
        private CollectionPointService() { }

        private static CollectionPointService instance = new CollectionPointService();
        public static ICollectionPointService Instance
        {
            get { return instance; }
        }

        public Employee GetClerkByCollectionPointId(int cpId)
        {
            return EmployeeRepo.Instance.GetClerkByCollectionPointId(cpId);
        }

        public IEnumerable<Employee> GetAllClerks()
        {
            return EmployeeRepo.Instance.GetAllClerks();
        }

        public IEnumerable<CollectionPoint> GetAllCollectionPoints()
        {
            return CollectionPointRepo.Instance.FindAll();
        }

        public CollectionPoint GetCollectionPointById(int id)
        {
            return CollectionPointRepo.Instance.FindById(id);
        }

        public void UpdateCollectionPoint(CollectionPoint collectionPoint)
        {

            CollectionPointRepo.Instance.Update(collectionPoint);
        }

        public void UpdateDepartmentCollectionPoint(Department department)
        {

            DepartmentRepo.Instance.Update(department);
        }

        public CollectionPoint GetDepartmentCollectionPointByEmployeeId(int employeeId)
        {
            return CollectionPointRepo.Instance.GetDepartmentCollectionPointByEmployeeId(employeeId);
        }


        public Department GetDepartmentByEmployeeId(int employeeId)
        {
            return DepartmentRepo.Instance.GetDepartmentByEmployeeId(employeeId);
        }
    }
}