using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using LUSSIS.Services.Interfaces;
using LUSSIS.Services;
using LUSSIS.Models.DTOs;
using iText.Layout;
using System.IO;
using iText.Kernel.Pdf;
using System.Text;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;

namespace LUSSIS.Controllers
{
    public class ChartController : Controller
    {
        PurchaseOrderService purchaseOrderService;
        // GET: Chart
        public ActionResult StationeryRequisitionTrend()
        {
            return View();
        }
        public ActionResult PurchaseOrderTrend()
        {
            return View();
        }

        public ActionResult InventoryHistoricalData()
        {
            purchaseOrderService = new PurchaseOrderService();
            SupplierChartFilteringDTO supplierChartFilterings = purchaseOrderService.FilteringByAttributes();
            SupplierChartFilteringDTO model = new SupplierChartFilteringDTO { SupplierForChartList = supplierChartFilterings.SupplierForChartList, StationeryForChartList = supplierChartFilterings.StationeryForChartList, CategoryForChartList = supplierChartFilterings.CategoryForChartList };

            //TempData["chartData"] = model;
            return View(model);
            //return RedirectToAction("ExportAsPDF");
        }


        public ActionResult ExportAsPDF()
        {
            SupplierChartFilteringDTO model = (SupplierChartFilteringDTO)TempData["chartData"];
            if (model != null)
            {
                MemoryStream workStream = new MemoryStream();
                PdfWriter writer = new PdfWriter(workStream);
                writer.SetCloseStream(false);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document pdfFile = CreatePDF(model, pdfDoc);
                pdfFile.Close();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                string strPDFFileName = string.Format("Report" + DateTime.Now.ToString("yyyyMMdd") + "-" + ".pdf");
                return File(workStream, "application/pdf", strPDFFileName);
            }
            //return RedirectToAction("InventoryHistoricalData");
            return null;
        }

        private Document CreatePDF(SupplierChartFilteringDTO model, PdfDocument pdfDoc)
        {
            Document doc = new Document(pdfDoc);

            List<int> SupplierIds = model.supplier;
            int CategoryId = model.category;
            int StationeryId = model.stationery;
            DateTime TheChosenDate = model.selectedDateTime;
            int trend = model.trend;

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
            int[] ArraySupplierIds = SupplierIds.ToArray();
            int[] AllSupplierIds = new int[] { 1, 2, 3, 4, 5, 6 };
            int[] ItemQtyArray = new int[3];
            string[] TwelveMonthRange = new string[3];
            decimal[] ItemPriceArray = new decimal[3];

            purchaseOrderService = new PurchaseOrderService();

            if (trend == 1)
            {
                if (ArraySupplierIds != null)
                {
                    foreach (int s in ArraySupplierIds)
                    {
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

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

                    };

                    Table t = GenerateTable(TwelveMonthRange, ItemQtyArray);

                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                            xValue: TwelveMonthRange,
                                                            yValues: ItemQtyArray).GetBytes("jpeg");
                    ImageData imgData = ImageDataFactory.Create(chartByteArr);
                    Image chartImage = new Image(imgData);

                    doc.Add(new Paragraph("Auto Generated Report").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                    doc.Add(chartImage);
                    doc.Add(t);
                    return doc;
                }
                else
                {
                    foreach (int s in AllSupplierIds)
                    {
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

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
                    };
                    Table t = GenerateTable(TwelveMonthRange, ItemQtyArray);

                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                            xValue: TwelveMonthRange,
                                                            yValues: ItemQtyArray).GetBytes("jpeg");
                    ImageData imgData = ImageDataFactory.Create(chartByteArr);
                    Image chartImage = new Image(imgData);

                    doc.Add(new Paragraph("Auto Generated Report").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                    doc.Add(chartImage);
                    doc.Add(t);
                    return doc;
                }
            }
            else
            {
                if (ArraySupplierIds != null)
                {
                    foreach (int s in ArraySupplierIds)
                    {
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

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
                    };
                    Table t = GenerateTable(TwelveMonthRange, ItemQtyArray);

                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                            xValue: TwelveMonthRange,
                                                            yValues: ItemPriceArray).GetBytes("jpeg");
                    ImageData imgData = ImageDataFactory.Create(chartByteArr);
                    Image chartImage = new Image(imgData);

                    doc.Add(new Paragraph("Auto Generated Report").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                    doc.Add(chartImage);
                    doc.Add(t);
                    return doc;
                }
                else
                {
                    foreach (int s in AllSupplierIds)
                    {
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

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
                    };
                    Table t = GenerateTable(TwelveMonthRange, ItemQtyArray);

                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                            xValue: TwelveMonthRange,
                                                            yValues: ItemPriceArray).GetBytes("jpeg");
                    ImageData imgData = ImageDataFactory.Create(chartByteArr);
                    Image chartImage = new Image(imgData);

                    doc.Add(new Paragraph("Auto Generated Report").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                    doc.Add(chartImage);
                    doc.Add(t);
                    return doc;
                }
            }
        }

        private Table GenerateTable(string[] TwelveMonthRange, int[] ItemQtyArray)
        {
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            var monthHeading = new Paragraph("Month").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            var qtyHeading = new Paragraph("Item Quantity").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            table.AddCell(monthHeading);
            table.AddCell(qtyHeading);
            for (int i = 0; i < 12; i++)
            {
                table.AddCell(TwelveMonthRange[i]);
                table.AddCell(ItemQtyArray[i].ToString());
            }

            return table;
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

            TempData["chartData"] = model;
            ViewBag.chart = "bar";
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
                    int[] ItemQtyArray = new int[3];
                    string[] TwelveMonthRange = new string[3];

                    foreach (int s in ArraySupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

                        

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
                        //chart.AddSeries(
                        //    chartType: "column",
                        //    xValue: TwelveMonthRange,
                        //    yValues: ItemQtyArray);


                    };
                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                            xValue: TwelveMonthRange,
                                                            yValues: ItemQtyArray).GetBytes("jpeg");
                    var path = Path.Combine(Server.MapPath("~/Images/Chart"), "chart.png");
                    System.IO.File.WriteAllBytes(path, chartByteArr);

                }
                else
                {
                    int[] ItemQtyArray = new int[3];
                    string[] TwelveMonthRange = new string[3];

                    foreach (int s in AllSupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);                      

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
                        //chart.AddSeries(
                        //    chartType: "column",
                        //    xValue: TwelveMonthRange,
                        //    yValues: ItemQtyArray);


                    };
                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                           xValue: TwelveMonthRange,
                                                           yValues: ItemQtyArray).GetBytes("jpeg");
                    var path = Path.Combine(Server.MapPath("~/Images/Chart"), "chart.png");
                    System.IO.File.WriteAllBytes(path, chartByteArr);
                }
            }
            else
            {
                if (ArraySupplierIds != null)
                {
                    int[] ItemQtyArray = new int[3];
                    string[] TwelveMonthRange = new string[3];
                    foreach (int s in ArraySupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

                        
                        decimal[] ItemPriceArray = new decimal[3];
                      

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
                        //chart.AddSeries(
                        //    chartType: "column",
                        //    xValue: TwelveMonthRange,
                        //    yValues: ItemPriceArray);


                    };
                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                           xValue: TwelveMonthRange,
                                                           yValues: ItemQtyArray).GetBytes("jpeg");
                    var path = Path.Combine(Server.MapPath("~/Images/Chart"), "chart.png");
                    System.IO.File.WriteAllBytes(path, chartByteArr);
                }
                else
                {
                    int[] ItemQtyArray = new int[3];
                    string[] TwelveMonthRange = new string[3];

                    foreach (int s in AllSupplierIds)
                    {

                        purchaseOrderService = new PurchaseOrderService();
                        List<SupplierChartDTO> PieChartDTOs = purchaseOrderService.TrendChartInfo(s, CategoryId, StationeryId);

                        
                        decimal[] ItemPriceArray = new decimal[3];
                        

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
                        //chart.AddSeries(
                        //    chartType: "column",
                        //    xValue: TwelveMonthRange,
                        //    yValues: ItemPriceArray);


                    };
                    byte[] chartByteArr = chart.AddSeries(chartType: "column",
                                                           xValue: TwelveMonthRange,
                                                           yValues: ItemQtyArray).GetBytes("jpeg");
                    var path = Path.Combine(Server.MapPath("~/Images/Chart"), "chart.png");
                    System.IO.File.WriteAllBytes(path, chartByteArr);
                }
            }
            //chart.Write("png");

            //var BarChartImage = chart.GetBytes();
            //return File(BarChartImage, "image/bytes");
            //return null;
            //chart.Write("png");

            //return null;
        }
    }
}