using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Services;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Filters;

namespace LUSSIS.Controllers
{
    public class AdjustmentVoucherController : Controller
    {

        // GET: AdjustmentVoucherList
        //By NESS
        [Authorizer]
        public ActionResult Index()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.StoreSupervisor && currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                List<AdjustmentVoucherDTO> adjustmentvouchers = AdjustmentVoucherService.Instance.getTotalAmountDTO();
                return View(adjustmentvouchers);
            }
            return RedirectToAction("Index", "Login");
        }

        // GET: AdjustmentVoucherDetails
        //By NESS
        [Authorizer]
        public ActionResult Detail(int adjId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.StoreSupervisor && currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                AdjustmentVoucherDTO adjDTO = new AdjustmentVoucherDTO();
                adjDTO.adjustmentVoucher = AdjustmentVoucherService.Instance.getAdjustmentVoucherById(adjId);

                return View(adjDTO);
            }
            return RedirectToAction("Index", "Login");
        }

        //CRETE AdjustmentVoucher getMethod
        //By NESS
        [Authorizer]
        public ActionResult Create()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                AdjustmentVoucherDTO adj = new AdjustmentVoucherDTO();
                adj.Stationeries = StationeryService.Instance.GetAllStationeries();
                return View(adj);
            }
            return RedirectToAction("Index", "Login");
        }

        //CRETE Adjustmentvoucher postMethod 
        //By NESS
        [Authorizer]
        [HttpPost]
        public ActionResult Create(AdjustmentVoucherDTO adjDTO)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                if (ModelState.IsValid)
                {
                    int clerkId = 1;
                    adjDTO.adjustmentVoucher = new AdjustmentVoucher();

                    AdjustmentVoucher existVoucher = AdjustmentVoucherService.Instance.getOpenAdjustmentVoucherByClerk(clerkId);
                    if (existVoucher != null)
                    {
                        adjDTO.adjustmentVoucher.Id = existVoucher.Id;
                    }
                    else
                    {
                        AdjustmentVoucher newVoucher = this.generateVoucher(adjDTO);
                        AdjustmentVoucherService.Instance.CreateAdjustmentVoucher(newVoucher);
                        adjDTO.adjustmentVoucher.Id = newVoucher.Id;
                    }
                    AdjustmentVoucherDetail newVoucherDetail = this.generateVoucherDetail(adjDTO);
                    AdjustmentVoucherService.Instance.CreateAdjustmentVoucherDetail(newVoucherDetail);

                    adjDTO.adjustmentVoucher = AdjustmentVoucherService.Instance.getAdjustmentVoucherById(adjDTO.adjustmentVoucher.Id);
                    //return RedirectToAction("Index");
                }

                adjDTO.Stationeries = StationeryService.Instance.GetAllStationeries();
                return View(adjDTO);
            }
            return RedirectToAction("Index", "Login");
        }

        //By NESS
        [Authorizer]
        private AdjustmentVoucher generateVoucher(AdjustmentVoucherDTO adjDTO)
        {
            AdjustmentVoucher newVoucher = new AdjustmentVoucher();
            newVoucher.Date = DateTime.Now;
            newVoucher.Status = Enum.GetName(typeof(Enums.AdjustmentVoucherStatus), Enums.AdjustmentVoucherStatus.Open);
            newVoucher.EmployeeId = 1;
            return newVoucher;
        }
        //By NESS
        [Authorizer]
        private AdjustmentVoucherDetail generateVoucherDetail(AdjustmentVoucherDTO adjdDTO)
        {
            AdjustmentVoucherDetail newVoucherDetail = new AdjustmentVoucherDetail();
            newVoucherDetail.AdjustmentVoucherId = adjdDTO.adjustmentVoucher.Id;
            newVoucherDetail.StationeryId = adjdDTO.StationeryId;
            newVoucherDetail.Quantity = adjdDTO.Quantity;
            newVoucherDetail.Reason = adjdDTO.Reason;
            newVoucherDetail.DateTime = DateTime.Now;
            return newVoucherDetail;
        }

        //change status of Adjustment Voucher
        //By NESS
        [Authorizer]
        public ActionResult ChangeStatus(int adjId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.StoreSupervisor && currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                AdjustmentVoucher adjustmentVoucher = AdjustmentVoucherService.Instance.getAdjustmentVoucherById(adjId);
                if (adjustmentVoucher.Status == Enum.GetName(typeof(Enums.AdjustmentVoucherStatus), Enums.AdjustmentVoucherStatus.Open))
                {
                    adjustmentVoucher.Status = Enum.GetName(typeof(Enums.AdjustmentVoucherStatus), Enums.AdjustmentVoucherStatus.Submitted);
                }
                else
                {
                    adjustmentVoucher.Status = Enum.GetName(typeof(Enums.AdjustmentVoucherStatus), Enums.AdjustmentVoucherStatus.Acknowledged);
                }
                AdjustmentVoucherService.Instance.UpdateAdjustmentVoucher(adjustmentVoucher);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Login");
        }


        [Authorizer]
        public JsonResult UpdateReceivedQty(int adjdId, int qty, int adjId)
        {
            AdjustmentVoucher adj = new AdjustmentVoucher();
            if (TempData["AdjustQty"] == null)
            {
                adj = AdjustmentVoucherService.Instance.getAdjustmentVoucherById(adjId);
            }
            else if (TempData["AdjustQty"] != null)
            {
                adj = (AdjustmentVoucher)TempData["AdjustQty"];
                TempData.Keep("AdjustQty");
            }
            if (adj.Id == adjId)
            {
                adj.AdjustmentVoucherDetails.Single(x => x.Id == adjdId).Quantity = qty;

                TempData["AdjustQty"] = adj;
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorizer]
        [HttpPost]
        public ActionResult SubmitVoucher(AdjustmentVoucherDTO adjDTO)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                if (ModelState.IsValid)
                {
                    AdjustmentVoucher updatedVoucher = AdjustmentVoucherService.Instance.getAdjustmentVoucherById(adjDTO.adjustmentVoucher.Id);
                    if (TempData["AdjustQty"] != null)
                    {
                        AdjustmentVoucher AdjustQtyDTO = (AdjustmentVoucher)TempData["AdjustQty"];
                        TempData.Keep("AdjustQty");

                        //update POdetails deliveredQty
                        foreach (var detail in AdjustQtyDTO.AdjustmentVoucherDetails)
                        {
                            AdjustmentVoucherDetail oldVoucherDetail = updatedVoucher.AdjustmentVoucherDetails.Single(x => x.Id == detail.Id);

                            oldVoucherDetail.Quantity = detail.Quantity;

                            //AdjustmentVoucherService.Instance.UpdateAdjustmentVoucherDetail(oldVoucherDetail);
                        }
                    }
                    updatedVoucher.Status = Enum.GetName(typeof(Enums.AdjustmentVoucherStatus), Enums.AdjustmentVoucherStatus.Submitted);
                    updatedVoucher.Date = DateTime.Now;
                    AdjustmentVoucherService.Instance.UpdateAdjustmentVoucher(updatedVoucher);
                    return RedirectToAction("Index");
                }
                return View(adjDTO);
            }
            return RedirectToAction("Index", "Login");
        }

    }

}