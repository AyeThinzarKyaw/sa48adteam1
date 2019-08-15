using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using LUSSIS.Services.Interfaces;
using LUSSIS.Services;
using LUSSIS.Models.DTOs;



namespace LUSSIS.Controllers
{
    public class ChartController : Controller
    {
        PurchaseOrderService purchaseOrderService;
        // GET: Chart
        public ActionResult InventoryHistoricalData()
        {
            purchaseOrderService = new PurchaseOrderService();
            SupplierChartFilteringDTO supplierChartFilterings = purchaseOrderService.FilteringByAttributes();
            SupplierChartFilteringDTO model = new SupplierChartFilteringDTO { SupplierForChartList = supplierChartFilterings.SupplierForChartList, StationeryForChartList = supplierChartFilterings.StationeryForChartList, CategoryForChartList = supplierChartFilterings.CategoryForChartList };


            return View(model);
        }
        [HttpPost]
        public ActionResult InventoryHistoricalData(SupplierChartFilteringDTO model)
        {
            if (model != null)
            {
                BarChart(model.supplier, model.category, model.stationery, model.selectedDateTime, model.trend);
            }
            purchaseOrderService = new PurchaseOrderService();
            SupplierChartFilteringDTO supplierChartFilterings = purchaseOrderService.FilteringByAttributes();
            model.SupplierForChartList = supplierChartFilterings.SupplierForChartList;
            model.StationeryForChartList = supplierChartFilterings.StationeryForChartList;
            model.CategoryForChartList = supplierChartFilterings.CategoryForChartList;

            return View(model);
        }
        public void BarChart(List<int> SupplierIds, int CategoryId, int StationeryId, DateTime TheChosenDate, int trend)
        {
            string themeChart = @"<Chart>
                      <ChartAreas>
                        <ChartArea Name=""Default"" _Template_=""All"">
                          <AxisY>
                            <LabelStyle Font=""Verdana, 12px"" />
                          </AxisY>
                          <AxisX LineColor=""64, 64, 64, 64"" Interval=""1"">
                            <LabelStyle Font=""Verdana, 12px"" />
                          </AxisX>
                        </ChartArea>
                      </ChartAreas>                        
                    </Chart>";
            Chart chart = new Chart(width: 1000, height: 200, theme: themeChart);


            //SupplierIds = new List<int>() { 1, 4 };
            int[] ArraySupplierIds = SupplierIds.ToArray();
            int[] AllSupplierIds = new int[] { 1, 2, 3, 4, 5, 6 };
            //int CategoryId = 1;
            //int StationeryId = 1;
            //DateTime TheChosenDate = new DateTime(2019, 2, 1);
            if (trend == 1)
            {
                if (ArraySupplierIds != null)
                {
                    foreach (int s in ArraySupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

                        int[] ItemQtyArray = new int[3];

                        string[] TwelveMonthRange = new string[3];

                        for (int i = 2; i > -1; i--)
                        {
                            TwelveMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                            int MonthlyQty = 0;
                            foreach (SupplierChartDTO pc in PieChartDTOs)
                            {
                                if (pc.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                {
                                    MonthlyQty += pc.QuantityOrdered;
                                    ItemQtyArray[2 - i] = MonthlyQty;
                                }
                            }
                        }
                        chart.AddSeries(
                            chartType: "column",
                            xValue: TwelveMonthRange,
                            yValues: ItemQtyArray);


                    };
                }
                else
                {
                    foreach (int s in AllSupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

                        int[] ItemQtyArray = new int[3];

                        string[] TwelveMonthRange = new string[3];

                        for (int i = 2; i > -1; i--)
                        {
                            TwelveMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                            int MonthlyQty = 0;
                            foreach (SupplierChartDTO pc in PieChartDTOs)
                            {
                                if (pc.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                {
                                    MonthlyQty += pc.QuantityOrdered;
                                    ItemQtyArray[2 - i] = MonthlyQty;
                                }
                            }
                        }
                        chart.AddSeries(
                            chartType: "column",
                            xValue: TwelveMonthRange,
                            yValues: ItemQtyArray);


                    };
                }
            }
            else
            {
                if (ArraySupplierIds != null)
                {
                    foreach (int s in ArraySupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

                        int[] ItemQtyArray = new int[3];
                        decimal[] ItemPriceArray = new decimal[3];
                        string[] TwelveMonthRange = new string[3];

                        for (int i = 2; i > -1; i--)
                        {
                            TwelveMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                            int MonthlyQty = 0;
                            decimal MonthlyPrice = 0;
                            foreach (SupplierChartDTO pc in PieChartDTOs)
                            {
                                if (pc.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                {
                                    MonthlyQty += pc.QuantityOrdered;
                                    ItemQtyArray[2 - i] = MonthlyQty;
                                    MonthlyPrice += MonthlyQty * pc.ItemUnitPrice;
                                    ItemPriceArray[2 - i] = MonthlyPrice;
                                }
                            }
                        }
                        chart.AddSeries(
                            chartType: "column",
                            xValue: TwelveMonthRange,
                            yValues: ItemPriceArray);


                    };
                }
                else
                {
                    foreach (int s in AllSupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

                        int[] ItemQtyArray = new int[3];
                        decimal[] ItemPriceArray = new decimal[3];
                        string[] TwelveMonthRange = new string[3];

                        for (int i = 2; i > -1; i--)
                        {
                            TwelveMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                            int MonthlyQty = 0;
                            decimal MonthlyPrice = 0;
                            foreach (SupplierChartDTO pc in PieChartDTOs)
                            {
                                if (pc.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                {
                                    MonthlyQty += pc.QuantityOrdered;
                                    ItemQtyArray[2 - i] = MonthlyQty;
                                    MonthlyPrice += MonthlyQty * pc.ItemUnitPrice;
                                    ItemPriceArray[2 - i] = MonthlyPrice;
                                }
                            }
                        }
                        chart.AddSeries(
                            chartType: "column",
                            xValue: TwelveMonthRange,
                            yValues: ItemPriceArray);


                    };
                }
            }
            chart.Write("png");

            //var BarChartImage = chart.GetBytes();
            //return File(BarChartImage, "image/bytes");
            //return null;
            //chart.Write("png");

            //return null;
        }
    }
}