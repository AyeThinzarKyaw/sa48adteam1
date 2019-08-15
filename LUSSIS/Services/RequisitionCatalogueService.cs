using LUSSIS.Enums;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using LUSSIS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class RequisitionCatalogueService : IRequisitionCatalogueService
    {
        private ICartDetailRepo cartDetailRepo;
        private IStationeryRepo stationeryRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private IEmployeeRepo employeeRepo;
        private IPurchaseOrderDetailRepo purchaseOrderDetailRepo;
        private IDisbursementRepo disbursementRepo;
        private IEmailNotificationService emailNotificationService;

        private static RequisitionCatalogueService instance = new RequisitionCatalogueService();

        private RequisitionCatalogueService()
        {
            cartDetailRepo = CartDetailRepo.Instance;
            stationeryRepo = StationeryRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
            purchaseOrderDetailRepo = PurchaseOrderDetailRepo.Instance;
            emailNotificationService = EmailNotificationService.Instance;
            disbursementRepo = DisbursementRepo.Instance;

        }

        //returns single instance
        public static IRequisitionCatalogueService Instance
        {
            get { return instance; }
        }

        public List<CatalogueItemDTO> GetCatalogueItems(int employeeId)
        {
            List<CartDetail> cartDetails = GetAnyExistingCartDetails(employeeId);
            List<Stationery> stationeries = (List<Stationery>)stationeryRepo.FindAll();
            List<CatalogueItemDTO> catalogueItems = new List<CatalogueItemDTO>();

            //for each stationery in stationeries create a catalogueItemDTO
            foreach(Stationery s in stationeries)
            {
                CatalogueItemDTO catalogueItemDTO = new CatalogueItemDTO()
                {
                    Item = s.Description,
                    UnitOfMeasure = s.UnitOfMeasure,
                    StationeryId = s.Id
                };
                getCatalogueItemAvailability(catalogueItemDTO, s);

                //CartDetail cartDetail=s.CartDetails.SingleOrDefault(x => x.EmployeeId == employeeId);
                //if (cartDetail!=null)
                //{
                //    catalogueItemDTO.OrderQtyInput = cartDetail.Quantity;
                //    catalogueItemDTO.ReservedCount = getReservedBalanceForExistingCartItem(cartDetail);
                //    catalogueItemDTO.WaitlistCount = cartDetail.Quantity - catalogueItemDTO.ReservedCount;
                //}
                
                catalogueItems.Add(catalogueItemDTO);
            }

            if (cartDetails != null)
            {
                foreach (CartDetail cd in cartDetails)
                {
                    CatalogueItemDTO catItemDTO = catalogueItems.Find(x => x.StationeryId == cd.StationeryId);
                    catItemDTO.OrderQtyInput = cd.Quantity;
                    catItemDTO.ReservedCount = getReservedBalanceForExistingCartItem(cd);
                    catItemDTO.WaitlistCount = cd.Quantity - catItemDTO.ReservedCount;
                    catItemDTO.Confirmation = true;
                }
            }
            return catalogueItems;
        }

        private StockAvailabilityEnum getStockAvailabilityStatus(int currBalance, int? reorderLevel)
        {
            if (currBalance <= 0)
            {
                return StockAvailabilityEnum.OutOfStock;
            }
            else if (reorderLevel != null && currBalance < reorderLevel)
            {
                return StockAvailabilityEnum.LowStock;
                
            }
            else 
            {
                return StockAvailabilityEnum.InStock;
            }
        }

        private int getReservedBalanceForExistingCartItem(CartDetail cd)
        {
            //Q - requisition reserved - adjustment open - total in front of queue
            int reservedCount = requisitionDetailRepo.GetReservedCountForStationery(cd.StationeryId);
            int foqCartCount = cartDetailRepo.GetFrontOfQueueCartCountForStationery(cd.StationeryId, cd.DateTime);
            int openAdjustmentCount = adjustmentVoucherRepo.GetOpenAdjustmentVoucherCountForStationery(cd.StationeryId);
            int totalCount = stationeryRepo.FindById(cd.StationeryId).Quantity;


            //int reservedCount = (from rd in cd.Stationery.RequisitionDetails
            //                     where (rd.Status.Equals("RESERVED_PENDING") || rd.Status.Equals("PREPARING") || rd.Status.Equals("PENDING_COLLECTION"))
            //                     select (int?)rd.QuantityOrdered).Sum() ?? 0;
            //int foqCartCount =  (int)(from c in cd.Stationery.CartDetails
            //                         where c.DateTime < cd.DateTime
            //                         select c.Quantity).Sum();
            //int openAdjustmentCount = (from av in cd.Stationery.AdjustmentVoucherDetails
            //                           where av.AdjustmentVoucher.Status.Equals("Open") || av.AdjustmentVoucher.Status.Equals("Pending")
            //                           select (int?)av.Quantity).Sum() ?? 0;
            //int totalCount = cd.Stationery.Quantity;
            int netCount = totalCount - reservedCount - foqCartCount + openAdjustmentCount;

            if (netCount <= 0)
            {
                return 0;
            }
            else if (netCount >= cd.Quantity)
            {
                return cd.Quantity;
            }
            else
            {
                return netCount;
            }
        }

        private int getCurrentBalance(Stationery s)
        {
            int reservedCount = requisitionDetailRepo.GetReservedCountForStationery(s.Id);
            int cartCount = cartDetailRepo.GetCountOnHoldForStationery(s.Id); //could be 0
            int openAdjustmentCount = adjustmentVoucherRepo.GetOpenAdjustmentVoucherCountForStationery(s.Id);
            int totalCount = stationeryRepo.FindById(s.Id).Quantity;
            //int reservedCount = (from rd in s.RequisitionDetails
            //                     where (rd.Status.Equals("RESERVED_PENDING") || rd.Status.Equals("PREPARING") || rd.Status.Equals("PENDING_COLLECTION"))
            //                     select (int?)rd.QuantityOrdered).Sum() ?? 0;
            //int cartCount = (int)(from cd in s.CartDetails
            //                      select cd.Quantity).Sum();
            //int openAdjustmentCount = (from av in s.AdjustmentVoucherDetails
            //                           where av.AdjustmentVoucher.Status.Equals("Open") || av.AdjustmentVoucher.Status.Equals("Pending")
            //                           select (int?)av.Quantity).Sum() ?? 0;
            //int totalCount = s.Quantity;
            int netCount = totalCount - reservedCount - cartCount + openAdjustmentCount;

            if(netCount <= 0)
            {
                return 0;
            }
            else
            {
                return netCount;
            }
          
        }


        //returns null if user's cart is empty
        private List<CartDetail> GetAnyExistingCartDetails(int employeeId)
        {
            
            List<CartDetail> cartDetails = (List<CartDetail>)cartDetailRepo.FindBy(x => x.Employee.Id == employeeId);
            if (cartDetails.Count == 0)
            {
                return null;
            }
            else
            {
                return cartDetails;
            }
            
        }     

        public CatalogueItemDTO AddCartDetail(int employeeId, int stationeryId, int inputQty)
        {
            Stationery s = stationeryRepo.FindById(stationeryId);
            int currBalance = getCurrentBalance(s);
            int reserved = 0;
            int waitlist = 0;
            if (inputQty > currBalance)
            {
                reserved = currBalance;
                waitlist = inputQty - currBalance;
            }
            else{
                reserved = inputQty;
            }
            CartDetail cd = new CartDetail { DateTime = DateTime.Now, EmployeeId = employeeId, Quantity = inputQty, StationeryId = stationeryId };
            cartDetailRepo.Create(cd); //persist new cart detail

            //can abstract into a method and share with above bbut must instantiate a new DTO first
            CatalogueItemDTO catalogueItemDTO = new CatalogueItemDTO() { ReservedCount = reserved, WaitlistCount = waitlist };
            getCatalogueItemAvailability(catalogueItemDTO, s);

            return catalogueItemDTO;

        }

        public CatalogueItemDTO RemoveCartDetail(int employeeId, int stationeryId)
        {
            Stationery s = stationeryRepo.FindById(stationeryId);
            //remove cd from db first
            CartDetail cd = cartDetailRepo.FindOneBy(x => x.EmployeeId == employeeId && x.StationeryId == stationeryId);
            if(cd != null)
            {
                cartDetailRepo.Delete(cd);
                CatalogueItemDTO catalogueItemDTO = new CatalogueItemDTO();
                getCatalogueItemAvailability(catalogueItemDTO, s);
                return catalogueItemDTO;
            }

            return null;
        }

        private void getCatalogueItemAvailability(CatalogueItemDTO catalogueItemDTO, Stationery s)
        {
            int currBalance = getCurrentBalance(s);
            int? lowStockCount = null;
            StockAvailabilityEnum stockAvailEnum = getStockAvailabilityStatus(currBalance, s.ReorderLevel);
            if (stockAvailEnum == StockAvailabilityEnum.LowStock)
            {
                lowStockCount = currBalance;
            }

            catalogueItemDTO.StockAvailability = stockAvailEnum;
            catalogueItemDTO.LowStockAvailability = lowStockCount;
        }

        public CatalogueItemDTO UpdateCartDetail(int employeeId, int stationeryId, int inputQty)
        {
            throw new NotImplementedException();
        }

        public List<Requisition> GetPersonalRequisitionHistory(int employeeId)
        {
            return (List<Requisition>)requisitionRepo.FindBy(x=> x.EmployeeId == employeeId);  
        }


        public Requisition ConvertCartDetailsToRequisitionDetails(int employeeId)
        {
            //erase records from cartDetails
            List<CartDetail> cartDetails = (List<CartDetail>)cartDetailRepo.FindBy(x => x.EmployeeId == employeeId);
            Requisition newRequisition = new Requisition() {DateTime = DateTime.Now, EmployeeId = employeeId,
                Status = RequisitionStatusEnum.PENDING.ToString() };
            newRequisition = requisitionRepo.Create(newRequisition);

            List<RequisitionDetail> requisitionDetails = new List<RequisitionDetail>();

            foreach(CartDetail cd in cartDetails)
            {
                cartDetailRepo.Delete(cd);

                int reservedCount = getReservedBalanceForExistingCartItem(cd);
                if(reservedCount < cd.Quantity)
                {
                    int waitlistCount = cd.Quantity - reservedCount;
                    //has waitlist, create 2 requisition details
                    createNewRequisitionDetail(waitlistCount, newRequisition.Id, cd.StationeryId, RequisitionDetailStatusEnum.WAITLIST_PENDING);
                    if(reservedCount > 0)
                    {
                        createNewRequisitionDetail(reservedCount, newRequisition.Id, cd.StationeryId, RequisitionDetailStatusEnum.RESERVED_PENDING);
                    }                    
                }
                else
                {
                    createNewRequisitionDetail(cd.Quantity, newRequisition.Id, cd.StationeryId, RequisitionDetailStatusEnum.RESERVED_PENDING);
                }

            }
            return newRequisition;
        }

        public void createNewRequisitionDetail(int qty, int requisitionId, int stationeryId, RequisitionDetailStatusEnum status)
        {
            RequisitionDetail reservedRequisitionDetail = new RequisitionDetail()
            {
                QuantityOrdered = qty,
                RequisitionId = requisitionId,
                Status = status.ToString(),
                StationeryId = stationeryId
            };
            //persist new RD record
            requisitionDetailRepo.Create(reservedRequisitionDetail);
        }

        private void createReservedPendingRequisitionDetail(int reservedCount, int requisitionId, int stationeryId)
        {
            RequisitionDetail reservedRequisitionDetail = new RequisitionDetail()
            {
                QuantityOrdered = reservedCount,
                RequisitionId = requisitionId,
                Status = RequisitionDetailStatusEnum.RESERVED_PENDING.ToString(),
                StationeryId = stationeryId
            };
            requisitionDetailRepo.Create(reservedRequisitionDetail);
        }

        RequisitionDetailsDTO IRequisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(int requisitionId, int employeeId)
        {
            string employeeName = employeeRepo.FindById(employeeId).Name;
            Requisition requisition = requisitionRepo.FindById(requisitionId);
            //switch to eager loading method
            //List<RequisitionDetail> requisitionDetails = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x=> x.RequisitionId == requisitionId);
            List<RequisitionDetail> requisitionDetails = requisitionDetailRepo.RequisitionDetailsEagerLoadStationery(requisitionId);
            return new RequisitionDetailsDTO() { EmployeeName = employeeName,
                RequestedDate = requisition.DateTime.ToString("yyyy-MM-dd"),
                RequisitionFormId = requisition.Id,
                RequisitionStatus = requisition.Status,
                Remarks = requisition.Remarks,
                RequisitionDetails = requisitionDetails};
        }

        public bool CancelPendingRequisition(int requisitionId,int cancelledBy)
        {
            Requisition r = requisitionRepo.FindById(requisitionId);
            if (r.EmployeeId!=cancelledBy)
            {
                return false;
            }
            r.Status = RequisitionStatusEnum.CANCELLED.ToString();
            requisitionRepo.Update(r);
            CascadeToRequisitionDetails("cancel", requisitionId);
            return true;
        }

        private void CascadeToRequisitionDetails(string action, int requisitionId)
        {
            //get all requisitiondetails belonging to this requisition
            if (action.Equals("cancel"))
            {
                List<RequisitionDetail> rds = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x=>x.RequisitionId == requisitionId);
                foreach(RequisitionDetail rd in rds)
                {
                    rd.Status = RequisitionDetailStatusEnum.CANCELLED.ToString();
                    requisitionDetailRepo.Update(rd);
                }
            }
        }

        public void CancelWaitlistedRequisitionDetail(int requisitionDetailId, int cancelledBy)
        {
            RequisitionDetail rd = requisitionDetailRepo.FindById(requisitionDetailId);
            if (rd.Requisition.EmployeeId==cancelledBy)
            {
                rd.Status = RequisitionDetailStatusEnum.CANCELLED.ToString();
                requisitionDetailRepo.Update(rd);
            }
            
        }

        public List<Requisition> GetSchoolRequisitionsWithEmployeeAndDept()
        {
            //call repo to eager fetch employee include dept
            List<Requisition> requisitions = requisitionRepo.SchoolRequisitionsEagerLoadEmployeeIncDepartment();

            return requisitions;
        }

        public RequisitionDetailsDTO GetRequisitionDetailsForClerk(int requisitionId)
        {
            Requisition r = requisitionRepo.OneRequisitionEagerLoadEmployeeIncDepartment(requisitionId);
            List<RequisitionDetail> rds = requisitionDetailRepo.RequisitionDetailsEagerLoadStationeryIncCategory(requisitionId);

            return new RequisitionDetailsDTO() { Requisition = r, RequisitionDetails = rds};
        }

        public void UpdateRequisitionDetailsAfterRetrieval(int qtyRetrieved, List<int> requisitionDetailIds)
        {
            List<RequisitionDetail> requisitionDetails = GetSortedRequisitionDetailsListFromListOfIds(requisitionDetailIds);

            int qtyToDisburse = qtyRetrieved;
            foreach (RequisitionDetail rd in requisitionDetails)
            {
                if (qtyToDisburse >= rd.QuantityOrdered)
                {
                    //can give all
                    rd.QuantityDelivered = rd.QuantityOrdered;
                    qtyToDisburse = qtyToDisburse - rd.QuantityOrdered;
                }
                else //qtyToDisburse < ordered
                {
                    //can give some
                    rd.QuantityDelivered = qtyToDisburse;
                    qtyToDisburse = 0;
                }
                //update status to PENDING COLLECTION and update db
                rd.Status = RequisitionDetailStatusEnum.PENDING_COLLECTION.ToString();
                requisitionDetailRepo.Update(rd);
            }
        }

        private List<RequisitionDetail> GetSortedRequisitionDetailsListFromListOfIds(List<int> requisitionDetailIds)
        {
            List<RequisitionDetail> requisitionDetails = new List<RequisitionDetail>();
            foreach (int id in requisitionDetailIds)
            {
                //Get RD
                RequisitionDetail rd = requisitionDetailRepo.FindById(id);
                requisitionDetails.Add(rd);
            }
            requisitionDetails.Sort(new RequisitionDetailComparer());

            return requisitionDetails;
        }

        //only call AFTER GENERATING NEW RD FOR UNFULFILLED ITEMS
        public void CheckRequisitionCompletenessAfterDisbursement(int disbursementId, Models.MobileDTOs.DisbursementDTO dDto)
        {
            List<Requisition> uniqueReqs = requisitionDetailRepo.GetUniqueRequisitionsForDisbursement(disbursementId);

            foreach(Requisition r in uniqueReqs)
            {
                int rowsReqDets = requisitionDetailRepo.FindBy(x=>x.RequisitionId == r.Id).Count();
                int rowsFulfilled = requisitionDetailRepo.FindBy(x=>x.RequisitionId == r.Id 
                && x.Status.Equals("COLLECTED")).Count();
                if (rowsReqDets == rowsFulfilled)
                {
                    //update r to completed
                    Requisition req = requisitionRepo.FindById(r.Id);
                    req.Status = RequisitionStatusEnum.COMPLETED.ToString();
                    requisitionRepo.Update(req);
                    emailNotificationService.NotifyEmployeeCompletedRequisition(r, r.Employee);
                }
            }
            Disbursement d = disbursementRepo.FindById(disbursementId);
            d.DeliveryDateTime = DateTime.Now;
            byte[] bytes = Convert.FromBase64String(dDto.Signature);
            d.Signature = bytes;
            d.OnRoute = false;
            disbursementRepo.Update(d);
            foreach(RequisitionDetail rd in d.RequisitionDetails)
            {
                Stationery s = stationeryRepo.FindById(rd.Stationery.Id);
                s.Quantity -= (int) rd.QuantityDelivered;
                stationeryRepo.Update(s);
            }
                
        }

        public void UpdateRequisitionDetailsAfterDisbursement(int qtyCollected, List<int> requisitionDetailIds)
        {
            List<RequisitionDetail> requisitionDetails = new List<RequisitionDetail>();

            List<RequisitionDetail> unfulfilledRds = new List<RequisitionDetail>();

            foreach (int id in requisitionDetailIds)
            {
                //Get RD
                RequisitionDetail rd = requisitionDetailRepo.FindById(id);
                requisitionDetails.Add(rd);
            }

            requisitionDetails.Sort(new RequisitionDetailComparer());

            int qtyDisbursed = qtyCollected;
            foreach(RequisitionDetail rd in requisitionDetails)
            {
                if (qtyDisbursed >= rd.QuantityOrdered)
                {
                    //can give all
                    rd.QuantityDelivered = rd.QuantityOrdered;
                    qtyDisbursed = qtyDisbursed - rd.QuantityOrdered;
                }
                else if (qtyDisbursed >0 && qtyDisbursed < rd.QuantityOrdered)
                {
                    //can give some
                    rd.QuantityDelivered = qtyDisbursed;
                    qtyDisbursed = 0;
                    unfulfilledRds.Add(rd);
                }
                else
                {
                    //can give 0
                    rd.QuantityDelivered = 0;
                    unfulfilledRds.Add(rd);
                }
                //update status to collected and update db
                rd.Status = RequisitionDetailStatusEnum.COLLECTED.ToString();
                requisitionDetailRepo.Update(rd);
            }

            //generate new rd for unfulfilled items
            GenerateNewRequisitionDetailsForUnfulfilledRds(unfulfilledRds);
        }

        public void GenerateNewRequisitionDetailsForUnfulfilledRds(List<RequisitionDetail> unfulfilledRds)
        {
            foreach (RequisitionDetail rd in unfulfilledRds)
            {
                int diff = rd.QuantityOrdered - (int)rd.QuantityDelivered;
                int availStockForUnfulfilled = GetAvailStockForUnfulfilledRd(rd.StationeryId, rd.Id);

                if(availStockForUnfulfilled < diff) //insufficient stock
                {
                    int waitlistCount = diff - availStockForUnfulfilled;
                    createNewRequisitionDetail(waitlistCount, rd.RequisitionId, rd.StationeryId, RequisitionDetailStatusEnum.WAITLIST_APPROVED);                   

                    if (availStockForUnfulfilled > 0)
                    {
                        createNewRequisitionDetail(availStockForUnfulfilled, rd.RequisitionId, rd.StationeryId, RequisitionDetailStatusEnum.PREPARING);
                    }
                }
                else
                {
                    createNewRequisitionDetail(diff, rd.RequisitionId, rd.StationeryId, RequisitionDetailStatusEnum.PREPARING);
                }

            }
        }

        public int GetAvailStockForUnfulfilledRd(int stationeryId, int reqDetId)
        {
            int reqInTransitCount = requisitionDetailRepo.GetReservedCountForStationery(stationeryId);

            int openAdjustmentCount = adjustmentVoucherRepo.GetOpenAdjustmentVoucherCountForStationery(stationeryId);

            int totalCount = stationeryRepo.FindById(stationeryId).Quantity;

            return totalCount + openAdjustmentCount - reqInTransitCount;
        }

        public void CheckStockAndUpdateStatusForWaitlistApprovedRequisitionDetails(int purchaseOrderId)
        {
            List<PurchaseOrderDetail> poDetails = (List<PurchaseOrderDetail>)purchaseOrderDetailRepo.FindBy(x=>x.PurchaseOrderId == purchaseOrderId);
            //get each stationery from each podetail
            foreach (PurchaseOrderDetail pod in poDetails)
            {
                int availstock =  getCurrentBalance(pod.Stationery);

                List<RequisitionDetail> requisitionDetails = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x=>x.StationeryId == pod.StationeryId && x.Status.Equals("WAITLIST_APPROVED"));
                requisitionDetails.Sort(new RequisitionDetailComparer());

                DistributeIncomingGoodsAmongWaitlistApprovedRds(availstock, requisitionDetails);
            }
        }

        private void DistributeIncomingGoodsAmongWaitlistApprovedRds(int qty, List<RequisitionDetail> requisitionDetails)
        {
            int qtyToAllocate = qty;

            int rdIndex = 0;

            while(qtyToAllocate != 0 && rdIndex < requisitionDetails.Count())
            {
                RequisitionDetail rd = requisitionDetails[rdIndex];

                if (qtyToAllocate >= rd.QuantityOrdered) //enough for this record
                {
                    qtyToAllocate = qtyToAllocate - rd.QuantityOrdered;
                    rd.Status = RequisitionDetailStatusEnum.PREPARING.ToString();
                }
                else
                {
                    int diff = rd.QuantityOrdered - qtyToAllocate;
                    rd.QuantityOrdered = qtyToAllocate; //update qtyOrdered column
                    qtyToAllocate = 0; //after this item exits the loop
                    rd.Status = RequisitionDetailStatusEnum.PREPARING.ToString();

                    createNewRequisitionDetail(diff, rd.RequisitionId, rd.StationeryId, RequisitionDetailStatusEnum.WAITLIST_APPROVED);
                }
                requisitionDetailRepo.Update(rd); //persist chnages in db

                rdIndex++;
            }

        }

        public List<DeptOwedItemDTO> GetListOfDeptOwedItems()
        {
            List<DeptOwedItemDTO> deptOwedItems = new List<DeptOwedItemDTO>();
            List<IGrouping<int, RequisitionDetail>> groups = requisitionDetailRepo.GetUnfulfilledRequisitionDetailsGroupedByDept();

            foreach(var group in groups)
            {              
                //get the department
                List<RequisitionDetail> rds = group.ToList();
                Department d = rds.First().Requisition.Employee.Department;

                //Create a list of owedItemDTOs
                List<OwedItemDTO> stationeryGroups = GetListOfOwedItemDTOs(rds);
                deptOwedItems.Add(new DeptOwedItemDTO {Department = d, OwedItems = stationeryGroups});
            }

            return deptOwedItems;
        }

        private List<OwedItemDTO> GetListOfOwedItemDTOs(List<RequisitionDetail> requisitionDetails)
        {
            List<OwedItemDTO> owedItems = new List<OwedItemDTO>();
            List<IGrouping<int, RequisitionDetail>> groups = requisitionDetails.GroupBy(x => x.StationeryId).ToList();
            foreach(var group in groups)
            {
                int sum = 0;
                List<RequisitionDetail> rds = group.ToList();
                foreach(RequisitionDetail rd in rds)
                {
                    int diff = rd.QuantityOrdered - (int)rd.QuantityDelivered;
                    sum += diff;
                }
                //get stationery
                Stationery s = rds.First().Stationery;
                owedItems.Add(new OwedItemDTO {Stationery = s, QtyOwed = sum });
            }
            return owedItems;
        }

        public bool HasItemInCart(int employeeId)
        {
            return cartDetailRepo.AnyItemInCartByEmployeeId(employeeId);
        }
    }
}