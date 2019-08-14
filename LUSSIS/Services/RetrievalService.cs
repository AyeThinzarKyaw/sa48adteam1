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
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private IAdjustmentVoucherDetailRepo adjustmentVoucherDetailRepo;
        private IRequisitionCatalogueService requisitionCatalogueService;
        private IPublicHolidayRepo publicHolidayRepo;
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
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
            adjustmentVoucherDetailRepo = AdjustmentVoucherDetailRepo.Instance;
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
            purchaseOrderDetailRepo = PurchaseOrderDetailRepo.Instance;
            publicHolidayRepo = PublicHolidayRepo.Instance;
        }

        //returns single instance
        public static IRetrievalService Instance
        {
            get { return instance; }
        }

        // method to get Collection Point Ids that Clerk is in charge of
        public List<CollectionPoint> RetrieveAssignedCollectionPoints(int employeeId)
        {
            List<CollectionPoint> assignedCollectionPointList = (List<CollectionPoint>)collectionPointRepo.FindBy(x => x.EmployeeId == employeeId);

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
                Stationery st = (Stationery)stationeryRepo.FindById(reqDet.StationeryId);
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
                    rPID.ReqDepartmentRep = employeesInAssignedDepartments.Find(x => x.RoleId == (int)Enums.Roles.DepartmentRepresentative);
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

            retrieval.RetrievalDate = System.DateTime.Now.ToString("dd/MM/yyyy");
            Employee clerk = (Employee)employeeRepo.FindOneBy(x => x.Id == loginDTO.EmployeeId);

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


        public void completeRetrievalProcess(RetrievalDTO retrieval,int employeeId)
        {
            int deliveredEmployeeId = employeeId;

            List<RetrievalItemDTO> retrievalItems = retrieval.RetrievalItem;

            foreach (RetrievalItemDTO rtItem in retrievalItems)
            {
                if (rtItem.RetrievedQty != rtItem.NeededQuantity)
                {
                    int qtyDifference = rtItem.NeededQuantity - rtItem.RetrievedQty;

                    AdjustmentVoucher targetAdjustmentVoucher = retrieveNewOrAvailableAdjustmentVoucherForClerk(deliveredEmployeeId);
                    createNewAdjustmentVoucherDetail(targetAdjustmentVoucher, rtItem.StationeryId, qtyDifference);

                    // List<RetrievalPrepItemDTO> orderedRPItemListByDate = (List<RetrievalPrepItemDTO>)rtItem.RetrievalPrepItemList.OrderBy(x => x.Req.DateTime);
                    List<RequisitionDetail> requisitionDetailsList = (List<RequisitionDetail>)rtItem.RetrievalPrepItemList.Select(x => x.ReqDetail).ToList();
                    List<int> requisitionDetailsIdList = (List<int>)requisitionDetailsList.Select(x => x.Id).ToList();

                    int availableBalance = rtItem.RetrievedQty;

                    //updates status of affected requisition details and chain creates new one if partially retrieved --> written by shona
                    requisitionCatalogueService.UpdateRequisitionDetailsAfterRetrieval(availableBalance, requisitionDetailsIdList);

                    List<RequisitionDetail> pendingCollectionList = getRequisitionDetailListByPendingCollectionAndStationeryId(rtItem.StationeryId);

                    createNewDisbursementsFromUpdatedRequisitionDetails(pendingCollectionList, deliveredEmployeeId);

                }
                else
                {
                    // List<RetrievalPrepItemDTO> orderedRPItemListByDate = (List<RetrievalPrepItemDTO>)rtItem.RetrievalPrepItemList.OrderBy(x => x.Req.DateTime);
                    List<RequisitionDetail> requisitionDetailsList = (List<RequisitionDetail>)rtItem.RetrievalPrepItemList.Select(x => x.ReqDetail).ToList();
                    List<int> requisitionDetailsIdList = (List<int>)requisitionDetailsList.Select(x => x.Id).ToList();

                    int availableBalance = rtItem.RetrievedQty;

                    //updates status of affected requisition details and chain creates new one if partially retrieved --> written by shona
                    requisitionCatalogueService.UpdateRequisitionDetailsAfterRetrieval(availableBalance, requisitionDetailsIdList);

                    List<RequisitionDetail> pendingCollectionList = getRequisitionDetailListByPendingCollectionAndStationeryId(rtItem.StationeryId);

                    createNewDisbursementsFromUpdatedRequisitionDetails(pendingCollectionList, deliveredEmployeeId);
                }

            }

        }


        public void createNewDisbursementsFromUpdatedRequisitionDetails(List<RequisitionDetail> reqDetailList, int deliveredEmployeeId)
        {
            foreach (RequisitionDetail entry in reqDetailList)
            {
                Requisition affectedReq = (Requisition)requisitionRepo.FindOneBy(x => x.Id == entry.RequisitionId);
                int targetDeptId = (int)employeeRepo.FindOneBy(x => x.Id == affectedReq.EmployeeId).DepartmentId;
                int receivedEmployeeId = (int)employeeRepo.FindOneBy(x => x.DepartmentId == targetDeptId && x.RoleId ==(int)Enums.Roles.DepartmentRepresentative).Id;

                RequisitionDetail target = entry;
                int targetDisId = retrieveNewOrAvailableDisbursementIdForDept(deliveredEmployeeId, receivedEmployeeId);

                target.DisbursementId = targetDisId;
                requisitionDetailRepo.Update(target);
            }
        }


        public List<RequisitionDetail> getRequisitionDetailListByPendingCollectionAndStationeryId(int stationeryId)
        {
            List<RequisitionDetail> returnReqList = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x => x.StationeryId == stationeryId && x.Status == "Pending_Collection");

            return returnReqList;

        }


        // create new adjustmentvoucher detail
        public void createNewAdjustmentVoucherDetail(AdjustmentVoucher adjustmentVoucher, int stationeryId, int quantity)
        {
            AdjustmentVoucherDetail aVD = new AdjustmentVoucherDetail();

            aVD.AdjustmentVoucherId = adjustmentVoucher.Id;
            aVD.StationeryId = stationeryId;
            aVD.Quantity = quantity;
            aVD.DateTime = System.DateTime.Now;

            aVD = adjustmentVoucherDetailRepo.Create(aVD);
        }


        // retrieve open Adj Voucher
        public AdjustmentVoucher retrieveNewOrAvailableAdjustmentVoucherForClerk(int deliveredEmployeeId)
        {
            AdjustmentVoucher targetAdjustmentVoucher = (AdjustmentVoucher)adjustmentVoucherRepo.FindOneBy(x => x.EmployeeId == deliveredEmployeeId && x.Status == "Open");

            AdjustmentVoucher returnTargetAdjustmentVoucherId;

            if (targetAdjustmentVoucher == null)
            {
                AdjustmentVoucher newAdjustmentVoucher = new AdjustmentVoucher() { EmployeeId = deliveredEmployeeId, Status = "Open" };
                newAdjustmentVoucher = adjustmentVoucherRepo.Create(newAdjustmentVoucher);

                returnTargetAdjustmentVoucherId = adjustmentVoucherRepo.FindOneBy(x => x.EmployeeId == deliveredEmployeeId && x.Status == "Open");

            }
            else
            {
                returnTargetAdjustmentVoucherId = targetAdjustmentVoucher;
            }

            return returnTargetAdjustmentVoucherId;
        }




        public int retrieveNewOrAvailableDisbursementIdForDept(int deliveredEmployeeId, int receivedEmployeeId)
        {
            Disbursement availableDisbursement = disbursementRepo.FindOneBy(x => x.DeliveredEmployeeId == deliveredEmployeeId && x.ReceivedEmployeeId == receivedEmployeeId && x.Signature == null);

            int targetDisbursementId;

            if (availableDisbursement == null)
            {
                Disbursement newDisbursement = new Disbursement();

                newDisbursement.DeliveredEmployeeId = deliveredEmployeeId;
                newDisbursement.ReceivedEmployeeId = receivedEmployeeId;
                newDisbursement.AdHoc = false;

                int deptId = (int)employeeRepo.FindOneBy(x => x.Id == receivedEmployeeId).DepartmentId;
                int cPId = (int)departmentRepo.FindOneBy(x => x.Id == deptId).CollectionPointId;
                newDisbursement.CollectionPoint = collectionPointRepo.FindById(cPId).NameTime;

                DateTime tomorrow = DateTime.Today.AddDays(1);
                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)tomorrow.DayOfWeek + 7) % 7;
                DateTime nextMonday = tomorrow.AddDays(daysUntilMonday);

                PublicHoliday holidayConflict = publicHolidayRepo.FindOneBy(x => x.Date == nextMonday);

                while (holidayConflict != null)
                {
                    daysUntilMonday = daysUntilMonday + 7;
                    nextMonday = tomorrow.AddDays(daysUntilMonday);

                    holidayConflict = publicHolidayRepo.FindOneBy(x => x.Date == nextMonday);

                }

                newDisbursement.DeliveryDateTime = nextMonday;

                newDisbursement = disbursementRepo.Create(newDisbursement);

                targetDisbursementId = disbursementRepo.FindOneBy(x => x.DeliveredEmployeeId == deliveredEmployeeId && x.ReceivedEmployeeId == receivedEmployeeId && x.Signature == null).Id;
            }
            else
            {
                targetDisbursementId = availableDisbursement.Id;
            }

            return targetDisbursementId;
        }


        public List<Department> retrieveAllDepartmentsWithApprovedRequisitions()
        {
            List<Department> departmentList = (List<Department>) departmentRepo.FindAll();
            List<Employee> employeesAllDepartments = RetrieveAllEmployeesInAssignedDepartmentList(departmentList);
            List<Requisition> approvedRequisitionsFromEmployeesInAssignedDepartments = RetrieveAllApprovedRequisitionsByEmployeeList(employeesAllDepartments);

            List<Employee> distinctEmployeeId = new List<Employee>();

            foreach (Requisition req in approvedRequisitionsFromEmployeesInAssignedDepartments.GroupBy(x => x.EmployeeId).Select(g => g.First()).Distinct().ToList())
            {
                Employee outputEmployee = employeeRepo.FindById(req.EmployeeId);
                distinctEmployeeId.Add(outputEmployee);
            }

            List<Department> distinctDepartmentList = new List<Department>();

            foreach (Employee x in distinctEmployeeId.GroupBy(x => x.DepartmentId).Select(g => g.First()).Distinct().ToList())
            {
                Department outputDepartment = departmentRepo.FindById(x.DepartmentId);
                distinctDepartmentList.Add(outputDepartment);
            }

            return distinctDepartmentList;
        }


        public List<Requisition> retrieveAllApprovedRequisitionIdsByDepartmentName(string departmentName)
        {
            Department department = departmentRepo.FindOneBy(x => x.DepartmentName == departmentName);
            List<Employee> employeesInAssignedDepartmentList = (List<Employee>)employeeRepo.FindBy(x => x.DepartmentId == department.Id).ToList();

            List<Requisition> approvedRequisitionsFromEmployeesInAssignedDepartments = RetrieveAllApprovedRequisitionsByEmployeeList(employeesInAssignedDepartmentList);

            return approvedRequisitionsFromEmployeesInAssignedDepartments;
        }



        public AdHocRetrievalMenuDTO generateAdHocRetrievalMenuDTO()
        {
            AdHocRetrievalMenuDTO output = new AdHocRetrievalMenuDTO();

            List<Department> departmentList = retrieveAllDepartmentsWithApprovedRequisitions();

            List<AdHocDeptAndRetrievalDTO> adDRList = new List<AdHocDeptAndRetrievalDTO>();


            foreach (Department dept in departmentList)
            {
                
                List<Requisition> rt =  retrieveAllApprovedRequisitionIdsByDepartmentName(dept.DepartmentName).ToList();
                AdHocDeptAndRetrievalDTO adDR = new AdHocDeptAndRetrievalDTO() { department = dept, requisitions = rt };

                adDRList.Add(adDR);
            }

            output.departmentAndRetrieval = adDRList;

            AdHocDeptAndRetrievalDTO firstAdHocAndRetrieval = adDRList.FirstOrDefault();
            

            return output;
        }


        // PENDING MODIFICATION TO METHOD
        public RetrievalDTO constructAdHocRetrievalDTO(LoginDTO loginDTO, int requisitionId)
        {
            List<RetrievalPrepItemDTO> retrievalPrepList = new List<RetrievalPrepItemDTO>();
            List<RetrievalItemDTO> retrievalList = new List<RetrievalItemDTO>();

            List<RequisitionDetail> preparingRequisitionDetailsFromSelectedRequisition = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x => x.RequisitionId == requisitionId).ToList();
            List<int> stationeriesInPreparingRequisitionDetails = RetrieveStationeryDetailsByRequisitionDetailsList(preparingRequisitionDetailsFromSelectedRequisition).ToList();

            foreach (int s in stationeriesInPreparingRequisitionDetails)
            {
                RetrievalItemDTO rID = new RetrievalItemDTO();

                rID.StationeryId = s;
                rID.Description = stationeryRepo.FindOneBy(x => x.Id == s).Description;
                rID.Location = stationeryRepo.FindOneBy(x => x.Id == s).Bin;

                List<RetrievalPrepItemDTO> tempRPIDTO = new List<RetrievalPrepItemDTO>();

                List<RequisitionDetail> reqDetailList = (List<RequisitionDetail>)preparingRequisitionDetailsFromSelectedRequisition.FindAll(x => x.StationeryId == rID.StationeryId).ToList();
                foreach (RequisitionDetail reDList in reqDetailList)
                {
                    RetrievalPrepItemDTO rPID = new RetrievalPrepItemDTO();
                    rPID.ReqStationery = stationeryRepo.FindOneBy(x => x.Id == reDList.StationeryId);
                    rPID.ReqDetail = reDList;
                    rPID.Req = requisitionRepo.FindOneBy(x => x.Id == requisitionId);
                    rPID.ReqOwner = employeeRepo.FindOneBy(x => x.Id == rPID.Req.EmployeeId);

                    int deptId = rPID.ReqOwner.DepartmentId;

                    rPID.ReqDepartmentRep = employeeRepo.FindOneBy(x => x.RoleId == (int)Enums.Roles.DepartmentRepresentative && x.DepartmentId == deptId);

                    rPID.ReqDepartment = departmentRepo.FindOneBy(x => x.Id == deptId);
                    rPID.ReqCollectionPoint = collectionPointRepo.FindOneBy(x => x.Id == rPID.ReqDepartment.CollectionPointId);

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

            retrieval.RetrievalDate = System.DateTime.Now.ToString("dd/MM/yyyy");
            Employee clerk = (Employee)employeeRepo.FindOneBy(x => x.Id == loginDTO.EmployeeId);

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