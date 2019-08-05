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
            var adjustmentvouchers = AdjustmentVoucherService.Instance.getAllAdjustmentVoucher().ToList();
            return View(adjustmentvouchers);
        }

        // GET: AdjustmentVoucherDetails
        //By NESS
        public ActionResult Detail(int adjId)
        {
            AdjustmentVoucher adjustmentVoucher = AdjustmentVoucherService.Instance.getAdjustmentVoucherById(adjId);
            return View(adjustmentVoucher);
        }

        //CRETE AdjustmentVoucher getMethod
        //By NESS
        public ActionResult Create()
        {
            AdjustmentVoucher adj = new AdjustmentVoucher();
            return View(adj);
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
    }
}