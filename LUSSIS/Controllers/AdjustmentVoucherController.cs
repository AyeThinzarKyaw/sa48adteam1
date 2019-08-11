using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Services;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;

namespace LUSSIS.Controllers
{
    public class AdjustmentVoucherController : Controller
    {

        // GET: AdjustmentVoucherList
        //By NESS
        public ActionResult Index()
        {
            List<AdjustmentVoucherDTO> adjustmentvouchers = AdjustmentVoucherService.Instance.getTotalAmountDTO();
            return View(adjustmentvouchers);
        }

        // GET: AdjustmentVoucherDetails
        //By NESS
        public ActionResult Detail(int adjId)
        {
            AdjustmentVoucherDTO adjDTO = new AdjustmentVoucherDTO();
            adjDTO.adjustmentVoucher = AdjustmentVoucherService.Instance.getAdjustmentVoucherById(adjId);
            
            return View(adjDTO);
        }

        //CRETE AdjustmentVoucher getMethod
        //By NESS
        public ActionResult Create()
        {
            AdjustmentVoucherDTO adj = new AdjustmentVoucherDTO();
            adj.Stationeries = StationeryService.Instance.GetAllStationeries();
            return View(adj);
        }

        //CRETE Adjustmentvoucher postMethod 
        //By NESS
        [HttpPost]
        public ActionResult Create(AdjustmentVoucherDTO adjDTO)
        {
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

        //By NESS
        private AdjustmentVoucher generateVoucher(AdjustmentVoucherDTO adjDTO)
        {
            AdjustmentVoucher newVoucher = new AdjustmentVoucher();
            newVoucher.Date = DateTime.Now;
            newVoucher.Status = Enum.GetName(typeof(Enums.AdjustmentVoucherStatus), Enums.AdjustmentVoucherStatus.Open);
            newVoucher.EmployeeId = 1;
            return newVoucher;
        }
        //By NESS
        private AdjustmentVoucherDetail generateVoucherDetail(AdjustmentVoucherDTO adjdDTO)
        {
            AdjustmentVoucherDetail newVoucherDetail = new AdjustmentVoucherDetail();
            newVoucherDetail.AdjustmentVoucherId =adjdDTO.adjustmentVoucher.Id;
            newVoucherDetail.StationeryId = adjdDTO.StationeryId;
            newVoucherDetail.Quantity = adjdDTO.Quantity;
            newVoucherDetail.Reason = adjdDTO.Reason;
            newVoucherDetail.DateTime = DateTime.Now;
            return newVoucherDetail;
        }

        //change status of Adjustment Voucher
        //By NESS
        public ActionResult ChangeStatus(int adjId)
        {
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

        [HttpPost]
        public ActionResult SubmitVoucher(AdjustmentVoucherDTO adjDTO)
        {
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

    }

}