using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class RequisitionManagementService: IRequisitionManagementService
    {
        //private IStationeryRepo stationeryRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IEmployeeRepo employeeRepo;
        private static RequisitionManagementService instance = new RequisitionManagementService();

        private RequisitionManagementService()
        {
            //stationeryRepo = StationeryRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
        }

        //returns single instance
        public static IRequisitionManagementService Instance
        {
            get { return instance; }
        }

        public List<Requisition> GetDepartmentRequisitions(int deptId)
        {
            return requisitionRepo.DepartmentRequisitionsEagerLoadEmployee(deptId);
        }
    }
}