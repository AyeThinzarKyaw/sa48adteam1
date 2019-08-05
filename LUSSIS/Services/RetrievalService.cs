using LUSSIS.Enums;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// written by Edwin
namespace LUSSIS.Services
{
    public class RetrievalService : IRetrievalService
    {
        private IStationeryRepo stationeryRepo;
        private ICategoryRepo categoryRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IDisbursementRepo disbursementRepo;
        private IEmployeeRepo employeeRepo;
        private IDepartmentRepo departmentRepo;
        private IPurchaseOrderRepo purchaseOrderRepo;
        private IPurchaseOrderDetailRepo purchaseOrderDetailRepo;
        private ICollectionPointRepo collectionPointRepo;
        private static RetrievalService instance = new RetrievalService();

        private RetrievalService()
        {
            stationeryRepo = StationeryRepo.Instance;
            categoryRepo = CategoryRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            disbursementRepo = DisbursementRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
            departmentRepo = DepartmentRepo.Instance;
            purchaseOrderRepo = PurchaseOrderRepo.Instance;
            collectionPointRepo = CollectionPointRepo.Instance;
            purchaseOrderDetailRepo = PurchaseOrderDetailRepo.Instance;
        }

        //returns single instance
        public static IRetrievalService Instance
        {
            get { return instance; }
        }

        // method to get Collection Point Ids that Clerk is in charge of
        public List<CollectionPoint> RetrieveAssignedCollectionPoints(int employeeId)
        {
            List<CollectionPoint> assignedCollectionPointList = (List<CollectionPoint>) collectionPointRepo.FindBy(x => x.EmployeeId == employeeId);

            return assignedCollectionPointList;
        }

        // method to get departments in collection point assigned
        public List<Department> RetrieveDepartmentsInCollectionPointList(List<CollectionPoint> collectionPointList)
        {
            List<Department> assignedDepartmentList = new List<Department>();

            foreach (CollectionPoint cp in collectionPointList)
            {
                List<Department> deptList = (List<Department>)departmentRepo.FindBy(x => x.CollectionPointId == cp.Id);
                foreach (Department dp in deptList)
                {
                    Department dept = dp;
                    assignedDepartmentList.Add(dept);
                }
            }

            return assignedDepartmentList;
        }

        // method to get all employees in Departments
        public List<Employee> RetrieveAllEmployeesInAssignedDepartmentList(List<Department> departmentList)
        {
            List<Employee> employeesInAssignedDepartmentList = new List<Employee>();

            foreach (Department dp in departmentList)
            {
                List<Employee> employeeList = (List<Employee>)employeeRepo.FindBy(x => x.DepartmentId == dp.Id);
                foreach (Employee empl in employeeList)
                {
                    Employee em = empl;
                    employeesInAssignedDepartmentList.Add(em);
                }
            }

            return employeesInAssignedDepartmentList;
        }

        // method to get all Status == Approved Requisitions from employees 
        public List<Requisition> RetrieveAllApprovedRequisitionsByEmployeeList(List<Employee> employeeList)
        {
            List<Requisition> approvedRequisitionList = new List<Requisition>();

            foreach (Employee em in employeeList)
            {
                List<Requisition> requisitionList = (List<Requisition>)requisitionRepo.FindBy(x => x.EmployeeId == em.Id && x.Status == "Approved");
                foreach (Requisition req in requisitionList)
                {
                    Requisition rq = req;
                    approvedRequisitionList.Add(rq);
                }
            }

            return approvedRequisitionList;
        }

        // method to get all Status == Preparing Requisition Details from Status == Approved Requisitions 
        public List<RequisitionDetail> RetrieveAllPreparingRequisitionDetailsByRequisitionList(List<Requisition> requisitionList)
        {
            List<RequisitionDetail> preparingRequisitionDetailList = new List<RequisitionDetail>();

            foreach (Requisition req in requisitionList)
            {
                List<RequisitionDetail> requisitionDetailList = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x => x.RequisitionId == req.Id && x.Status == "Preparing");
                foreach (RequisitionDetail reqDet in requisitionDetailList)
                {
                    RequisitionDetail rqDet = reqDet;
                    preparingRequisitionDetailList.Add(rqDet);
                }
            }

            return preparingRequisitionDetailList;
        }

        // method to get all stationery Details from Status == Preparing Requisition Details
        public List<int> RetrieveStationeryDetailsByRequisitionDetailsList(List<RequisitionDetail> requisitionDetailList)
        {
            List<int> stationeryIdList = new List<int>();
            List<Stationery> stationeryDetailList = new List<Stationery>();

            foreach (RequisitionDetail reqDet in requisitionDetailList.GroupBy(x => x.StationeryId).Select(g => g.First()).Distinct().ToList())
            {
                Stationery st = (Stationery) stationeryRepo.FindById(reqDet.StationeryId);
                stationeryIdList.Add(st.Id);

            }

            return stationeryIdList;
        }

        public RetrievalDTO constructRetrievalDTO(LoginDTO loginDTO)
        {
            List<RetrievalPrepItemDTO> retrievalPrepList = new List<RetrievalPrepItemDTO>();
            List<RetrievalItemDTO> retrievalList = new List<RetrievalItemDTO>();

            List<CollectionPoint> assignedCollectionPoint = RetrieveAssignedCollectionPoints(loginDTO.EmployeeId);
            List<Department> assignedDepartment = RetrieveDepartmentsInCollectionPointList(assignedCollectionPoint);
            List<Employee> employeesInAssignedDepartments = RetrieveAllEmployeesInAssignedDepartmentList(assignedDepartment);
            List<Requisition> approvedRequisitionsFromEmployeesInAssignedDepartments = RetrieveAllApprovedRequisitionsByEmployeeList(employeesInAssignedDepartments);
            List<RequisitionDetail> preparingRequisitionDetailsFromApprovedRequisitions = RetrieveAllPreparingRequisitionDetailsByRequisitionList(approvedRequisitionsFromEmployeesInAssignedDepartments);
            List<int> stationeriesInPreparingRequisitionDetails = RetrieveStationeryDetailsByRequisitionDetailsList(preparingRequisitionDetailsFromApprovedRequisitions);

            // .GroupBy(x => x.Id).Select(g => g.First()).ToList()
            // create retrievalItemDTO
            foreach (int s in stationeriesInPreparingRequisitionDetails)
            {
                RetrievalItemDTO rID = new RetrievalItemDTO();

                rID.StationeryId = s;
                rID.Description = stationeryRepo.FindOneBy(x => x.Id == s).Description;
                rID.Location = stationeryRepo.FindOneBy(x => x.Id == s).Bin;

                List<RetrievalPrepItemDTO> tempRPIDTO = new List<RetrievalPrepItemDTO>();

                List<RequisitionDetail> reqDetailList = (List<RequisitionDetail>)preparingRequisitionDetailsFromApprovedRequisitions.FindAll(x => x.StationeryId == rID.StationeryId);
                foreach (RequisitionDetail reDList in reqDetailList)
                {
                    RetrievalPrepItemDTO rPID = new RetrievalPrepItemDTO();
                    rPID.ReqStationery = stationeryRepo.FindOneBy(x => x.Id == reDList.StationeryId);
                    rPID.ReqDetail = reDList;
                    rPID.Req = approvedRequisitionsFromEmployeesInAssignedDepartments.Find(x => x.Id == rPID.ReqDetail.RequisitionId);
                    rPID.ReqOwner = employeesInAssignedDepartments.Find(x => x.Id == rPID.Req.EmployeeId);
                    rPID.ReqDepartmentRep = employeesInAssignedDepartments.Find(x => x.RoleId == 3);
                    rPID.ReqDepartment = assignedDepartment.Find(x => x.Id == rPID.ReqDepartmentRep.DepartmentId);
                    rPID.ReqCollectionPoint = assignedCollectionPoint.Find(x => x.Id == rPID.ReqDepartment.CollectionPointId);

                    tempRPIDTO.Add(rPID);
                }

                rID.RetrievalPrepItemList = tempRPIDTO;

                int count = 0;

                foreach (RetrievalPrepItemDTO retriPrepItem in tempRPIDTO)
                {
                    RequisitionDetail x = retriPrepItem.ReqDetail;
                    int y = x.QuantityOrdered;
                    count = count + y;
                }

                rID.NeededQuantity = count;
                rID.RetrievedQty = count;


                retrievalList.Add(rID);
            }

            RetrievalDTO retrieval = new RetrievalDTO();

            retrieval.RetrievalDate = System.DateTime.Now.ToString("MM/dd/yyyy");
            Employee clerk = (Employee) employeeRepo.FindOneBy(x => x.Id == loginDTO.EmployeeId);

            if (clerk.Name == null)
            {
                retrieval.GeneratedBy = null;
            }
            else
            {
                retrieval.GeneratedBy = clerk.Name;
            }

            retrieval.RetrievalItem = retrievalList;

            return retrieval;
        }


        }
}