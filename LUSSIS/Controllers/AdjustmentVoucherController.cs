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

        //CRETE AdjustmentVoucher getMethod
        //By NESS
        public ActionResult Create()
        {
            AdjustmentVoucher adj = new AdjustmentVoucher();
            return View(adj);
        }
    }
}