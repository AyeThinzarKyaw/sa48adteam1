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
using LUSSIS.Filters;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Repositories;
using LUSSIS.Models;

namespace LUSSIS.Controllers
{
    public class ChartController : Controller
    {

        ChartService chartService;
        ISupplierRepo supplierRepo;
        IDepartmentRepo departmentRepo;
        ICategoryRepo categoryRepo;
        IStationeryRepo stationeryRepo;

        public ChartController()
        {
            supplierRepo = SupplierRepo.Instance;
            departmentRepo = DepartmentRepo.Instance;
            categoryRepo = CategoryRepo.Instance;
            stationeryRepo = StationeryRepo.Instance;
        }

        // GET: Chart
        [Authorizer]
        public ActionResult InventoryHistoricalData()
        {

            chartService = new ChartService();
            ChartFilteringDTO supplierChartFilterings = chartService.FilteringByAttributes();
            ChartFilteringDTO model = new ChartFilteringDTO { SupplierForChartList = supplierChartFilterings.SupplierForChartList, StationeryForChartList = supplierChartFilterings.StationeryForChartList, CategoryForChartList = supplierChartFilterings.CategoryForChartList, DepartmentForChartList = supplierChartFilterings.DepartmentForChartList };


            return View(model);
        }

        [Authorizer]
        public JsonResult FilterItem(int id)
        {           

            var aa = StationeryService.Instance.GetStationeriesByCategory(id);

            var bb =
                from c in aa
                orderby c.Description
                select new
                {
                    Id = c.Id,
                    Description = c.Description
                };

            return Json(bb,JsonRequestBehavior.AllowGet);
        }

        [Authorizer]
        #region ExportAsPDF
        public ActionResult ExportAsPDF()
        {
            if (TempData["FilterModel"] != null)
            {
                ChartFilteringDTO model = (ChartFilteringDTO)TempData["FilterModel"];
                TempData.Keep("FilterModel");
                //ChartFilteringDTO model = (ChartFilteringDTO)TempData["chartData"];
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
                else   //return RedirectToAction("InventoryHistoricalData");
                    return null;
            }
            return RedirectToAction("InventoryHistoricalData");
        }
        #endregion

        [Authorizer]
        #region CreatePDF
        private Document CreatePDF(ChartFilteringDTO model, PdfDocument pdfDoc)
        {
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Auto Generated Report").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));

            List<int> SupplierIds = model.supplier;
            int CategoryId = model.category;
            int StationeryId = model.stationery;
            DateTime TheChosenDate = model.selectedDateTime;
            int trend = model.trend;

            List<int> DepartmentIdsForDep = model.department;
            int CategoryIdForDep = model.categoryDep;
            int StationeryIdForDep = model.stationeryDep;
            DateTime TheChosenDateForDep = model.selectedDateTimeDep;
            int trendForDep = model.trendDep;



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
            if (SupplierIds != null)
            {
                Category cat = categoryRepo.FindById(CategoryId);
                Stationery stat = stationeryRepo.FindById(StationeryId);

                doc.Add(new Paragraph("Item Category: " + cat.Type));
                if(stat != null)
                {
                    doc.Add(new Paragraph("Item: " + stat.Description));
                }
                else
                {
                    doc.Add(new Paragraph("Item: ALL"));
                }
                

                int[] AllSupplierIds = new int[] { 1, 2, 3, 4, 5, 6 }; //total 6 suppliers
                int[] AllStationeryIds = new int[90]; //all stationery items


                if (SupplierIds != null)
                {
                    int[] ArraySupplierIds = SupplierIds.ToArray(); //convert input parameter from list to array
                    if (trend == 1)
                    {
                        if (StationeryId != 0) // if there is an item selected
                        {
                            foreach (int s in ArraySupplierIds)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId); //calling service to pass the list of ChartDTO
                                int[] ItemQtyArray = new int[3]; // for 3 months comparison
                                string[] ThreeMonthRange = new string[3]; // for 3 months comparison


                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"); //convert date into MMMM-yyyy
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        } //sum up the quantity within the month

                                    }
                                }
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: BarChartInfo.FirstOrDefault().SupplierName);
                                }//add series to chart
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else if (StationeryId == 0) // if no item selected
                        {
                            foreach (int s in ArraySupplierIds)
                            {
                                int[] ItemQtyArray = new int[3];

                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                    if (BarChartInfo.Any(x => x.SupplierId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.QuantityOrdered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else { return null; }
                    }
                    else
                    {
                        if (StationeryId != 0)
                        {
                            foreach (int s in ArraySupplierIds)
                            {


                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId);

                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];

                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: BarChartInfo.FirstOrDefault().SupplierName);
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));

                            };
                            chart.AddTitle("Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else if (StationeryId == 0)
                        {
                            foreach (int s in ArraySupplierIds)
                            {
                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                    if (BarChartInfo.Any(x => x.SupplierId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        decimal MonthlyPrice = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.QuantityOrdered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                                MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                                ItemPriceArray[2 - i] = MonthlyPrice;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else { return null; }
                    }
                }
                else
                {
                    if (trend == 1)
                    {
                        if (StationeryId != 0)
                        {
                            foreach (int s in AllSupplierIds)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId);
                                int[] ItemQtyArray = new int[3];
                                string[] ThreeMonthRange = new string[3];

                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        }
                                    }
                                }
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: BarChartInfo.FirstOrDefault().SupplierName);
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else if (StationeryId == 0)
                        {
                            foreach (int s in AllSupplierIds)
                            {
                                int[] ItemQtyArray = new int[3];

                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                    if (BarChartInfo.Any(x => x.SupplierId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.QuantityOrdered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));

                            };
                            chart.AddTitle("Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else { return null; }
                    }
                    else
                    {
                        if (StationeryId != 0)
                        {
                            foreach (int s in AllSupplierIds)
                            {


                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId);

                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];

                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: BarChartInfo.FirstOrDefault().SupplierName);
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));

                            };
                            chart.AddTitle("Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else if (StationeryId == 0)
                        {
                            foreach (int s in AllSupplierIds)
                            {
                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                    if (BarChartInfo.Any(x => x.SupplierId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        decimal MonthlyPrice = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.QuantityOrdered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                                MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                                ItemPriceArray[2 - i] = MonthlyPrice;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Supplier supplier = supplierRepo.FindById(s);
                                doc.Add(new Paragraph(supplier.Name).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);
                            return doc;
                        }
                        else { return null; }
                    }
                }
            }
            else
            {
                Category cat = categoryRepo.FindById(CategoryIdForDep);
                Stationery stat = stationeryRepo.FindById(StationeryIdForDep);

                doc.Add(new Paragraph("Item Category: " + cat.Type));
                if (stat != null)
                {
                    doc.Add(new Paragraph("Item: " + stat.Description));
                }
                else
                {
                    doc.Add(new Paragraph("Item: ALL"));
                }

                int[] AllDepartmentIds = new int[] { 1, 2, 3, 4, 5, 6, 7 }; //total 7 Departments
                int[] AllStationeryIds = new int[90]; //all stationery items
                if (DepartmentIdsForDep != null)
                {
                    int[] ArrayDepartmentIds = DepartmentIdsForDep.ToArray(); //convert input parameter from list to array
                    if (trendForDep == 1)
                    {
                        if (StationeryIdForDep != 0) // if there is an item selected
                        {
                            foreach (int s in ArrayDepartmentIds)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep); //calling service to pass the list of ChartDTO
                                int[] ItemQtyArray = new int[3]; // for 3 months comparison
                                string[] ThreeMonthRange = new string[3]; // for 3 months comparison


                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"); //convert date into MMMM-yyyy
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        } //sum up the quantity within the month

                                    }
                                }
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                                }//add series to chart
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Department Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else if (StationeryIdForDep == 0) // if no item selected
                        {
                            foreach (int s in ArrayDepartmentIds)
                            {
                                int[] ItemQtyArray = new int[3];

                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                    if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.RequisitionQuantityDelivered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Department Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else { return null; }
                    }
                    else
                    {
                        if (StationeryIdForDep != 0)
                        {
                            foreach (int s in ArrayDepartmentIds)
                            {


                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep);

                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];

                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));

                            };
                            chart.AddTitle("Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);
                            return doc;
                        }
                        else if (StationeryIdForDep == 0)
                        {
                            foreach (int s in ArrayDepartmentIds)
                            {
                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                    if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        decimal MonthlyPrice = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.RequisitionQuantityDelivered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                                MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                                ItemPriceArray[2 - i] = MonthlyPrice;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Department Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);
                            return doc;
                        }
                        else { return null; }
                    }
                }
                else
                {
                    if (trendForDep == 1)
                    {
                        if (StationeryIdForDep != 0)
                        {
                            foreach (int s in AllDepartmentIds)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep);
                                int[] ItemQtyArray = new int[3];
                                string[] ThreeMonthRange = new string[3];

                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        }
                                    }
                                }
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Department Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else if (StationeryIdForDep == 0)
                        {
                            foreach (int s in AllDepartmentIds)
                            {
                                int[] ItemQtyArray = new int[3];

                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                    if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.RequisitionQuantityDelivered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemQtyArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));

                            };
                            chart.AddTitle("Department Monthly Volume Comparison");
                            chart.SetYAxis("Volume");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);

                            return doc;
                        }
                        else { return null; }

                    }
                    else
                    {
                        if (StationeryIdForDep != 0)
                        {
                            foreach (int s in AllDepartmentIds)
                            {


                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep);

                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];

                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));

                            };
                            chart.AddTitle("Department Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);
                            return doc;
                        }
                        else if (StationeryIdForDep == 0)
                        {
                            foreach (int s in AllDepartmentIds)
                            {
                                int[] ItemQtyArray = new int[3];
                                decimal[] ItemPriceArray = new decimal[3];
                                string[] ThreeMonthRange = new string[3];
                                List<string> SuNames = new List<string>();
                                for (int j = 0; j < AllStationeryIds.Length; j++)
                                {
                                    chartService = new ChartService();
                                    List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                    if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                    { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                    for (int i = 2; i > -1; i--)
                                    {
                                        ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                        int MonthlyQty = 0;
                                        decimal MonthlyPrice = 0;
                                        foreach (ChartDTO bci in BarChartInfo)
                                        {
                                            if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                            {
                                                MonthlyQty += bci.RequisitionQuantityDelivered;
                                                ItemQtyArray[2 - i] = MonthlyQty;
                                                MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                                ItemPriceArray[2 - i] = MonthlyPrice;
                                            }
                                        }
                                    }
                                }
                                if (SuNames.Count > 0)
                                {
                                    chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemPriceArray,
                                    name: SuNames.First());
                                }
                                Table t = GenerateVolumeTable(ThreeMonthRange, ItemPriceArray);
                                Department dep = departmentRepo.FindById(s);
                                doc.Add(new Paragraph(dep.DepartmentName + " Department").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                doc.Add(t);
                                doc.Add(new Paragraph("\n"));
                            };
                            chart.AddTitle("Department Monthly Price Comparison");
                            chart.SetYAxis("Price");
                            chart.AddLegend();
                            chart.SetXAxis("Time");
                            byte[] chartByteArr = chart.GetBytes("jpeg");
                            ImageData imgData = ImageDataFactory.Create(chartByteArr);
                            Image chartImage = new Image(imgData);

                            doc.Add(chartImage);
                            return doc;
                        }
                        else { return null; }
                    }

                }
            }
        }

        private Table GenerateVolumeTable(string[] ThreeMonthRange, decimal[] ItemPriceArray)
        {
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            var monthHeading = new Paragraph("Month").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            var qtyHeading = new Paragraph("Item Quantity").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            table.AddCell(monthHeading);
            table.AddCell(qtyHeading);
            for (int i = 0; i < 3; i++)
            {
                table.AddCell(ThreeMonthRange[i]);
                table.AddCell(ItemPriceArray[i].ToString());
            }

            return table;
        }
        #endregion
    
        [Authorizer]
        #region VolumeTable
        private Table GenerateVolumeTable(string[] TwelveMonthRange, int[] ItemQtyArray)
        {
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            var monthHeading = new Paragraph("Month").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            var qtyHeading = new Paragraph("Item Quantity").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            table.AddCell(monthHeading);
            table.AddCell(qtyHeading);
            for (int i = 0; i < 3; i++)
            {
                table.AddCell(TwelveMonthRange[i]);
                table.AddCell(ItemQtyArray[i].ToString());
            }

            return table;
        }
        #endregion

        [Authorizer]
        #region PriceTable
        private Table GeneratePriceTable(string[] TwelveMonthRange, decimal[] ItemPriceArray)
        {
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            var monthHeading = new Paragraph("Month").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            var priceHeading = new Paragraph("Item Quantity").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            table.AddCell(monthHeading);
            table.AddCell(priceHeading);
            for (int i = 0; i < 3; i++)
            {
                table.AddCell(TwelveMonthRange[i]);
                table.AddCell(ItemPriceArray[i].ToString());
            }

            return table;
        }
        #endregion

        #region InventoryHistoryicalData(HttpPost)
        [HttpPost]
        [Authorizer]
        public ActionResult InventoryHistoricalDataDep(ChartFilteringDTO model)
        {
            if (model != null)
            {

                BarChartDepartment(model.department, model.categoryDep, model.stationeryDep, model.selectedDateTimeDep, model.trendDep);
                ViewBag.chart = "dept";


            }

            chartService = new ChartService();
            ChartFilteringDTO supplierChartFilterings = chartService.FilteringByAttributes();
            model.SupplierForChartList = supplierChartFilterings.SupplierForChartList;
            model.StationeryForChartList = supplierChartFilterings.StationeryForChartList;
            model.CategoryForChartList = supplierChartFilterings.CategoryForChartList;
            model.DepartmentForChartList = supplierChartFilterings.DepartmentForChartList;
            TempData["FilterModel"] = model;
            return View("InventoryHistoricalData", "_Layout", model);
        }
        #endregion
        [Authorizer]
        #region InventoryHistoryicalData(HttpPost)
        [HttpPost]
        public ActionResult InventoryHistoricalDataSup(ChartFilteringDTO model)
        {
            if (model != null)
            {

                BarChartSupplier(model.supplier, model.category, model.stationery, model.selectedDateTime, model.trend);
                ViewBag.chart = "supp";


            }

            chartService = new ChartService();
            ChartFilteringDTO supplierChartFilterings = chartService.FilteringByAttributes();
            model.SupplierForChartList = supplierChartFilterings.SupplierForChartList;
            model.StationeryForChartList = supplierChartFilterings.StationeryForChartList;
            model.CategoryForChartList = supplierChartFilterings.CategoryForChartList;
            model.DepartmentForChartList = supplierChartFilterings.DepartmentForChartList;
            TempData["FilterModel"] = model;
            return View("InventoryHistoricalData", "_Layout", model);
        }
        #endregion

        [Authorizer]
        #region BarChartSupplierMethod
        public void BarChartSupplier(List<int> SupplierIds, int CategoryId, int StationeryId, DateTime TheChosenDate, int trend)
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

            int[] AllSupplierIds = new int[] { 1, 2, 3, 4, 5, 6 }; //total 6 suppliers
            int[] AllStationeryIds = new int[90]; //all stationery items


            if (SupplierIds != null)
            {
                int[] ArraySupplierIds = SupplierIds.ToArray(); //convert input parameter from list to array
                if (trend == 1)
                {
                    if (StationeryId != 0) // if there is an item selected
                    {
                        foreach (int s in ArraySupplierIds)
                        {
                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId); //calling service to pass the list of ChartDTO
                            int[] ItemQtyArray = new int[3]; // for 3 months comparison
                            string[] ThreeMonthRange = new string[3]; // for 3 months comparison


                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"); //convert date into MMMM-yyyy
                                int MonthlyQty = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.QuantityOrdered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                    } //sum up the quantity within the month

                                }
                            }
                            if (BarChartInfo.Any(x => x.SupplierId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemQtyArray,
                                name: BarChartInfo.FirstOrDefault().SupplierName);
                            }//add series to chart

                        };
                    }
                    else if (StationeryId == 0) // if no item selected
                    {
                        foreach (int s in ArraySupplierIds)
                        {
                            int[] ItemQtyArray = new int[3];

                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemQtyArray,
                                name: SuNames.First());
                            }

                        };
                    }
                    chart.AddTitle("Monthly Volume Comparison");
                    chart.SetYAxis("Volume");
                }
                else
                {
                    if (StationeryId != 0)
                    {
                        foreach (int s in ArraySupplierIds)
                        {


                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId);

                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];

                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                int MonthlyQty = 0;
                                decimal MonthlyPrice = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.QuantityOrdered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                        MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                        ItemPriceArray[2 - i] = MonthlyPrice;
                                    }
                                }
                            }
                            if (BarChartInfo.Any(x => x.SupplierId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: BarChartInfo.FirstOrDefault().SupplierName);
                            }

                        };
                    }
                    else if (StationeryId == 0)
                    {
                        foreach (int s in ArraySupplierIds)
                        {
                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: SuNames.First());
                            }
                        };
                    }
                    chart.AddTitle("Monthly Price Comparison");
                    chart.SetYAxis("Price");
                }
            }
            else
            {
                if (trend == 1)
                {
                    if (StationeryId != 0)
                    {
                        foreach (int s in AllSupplierIds)
                        {
                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId);
                            int[] ItemQtyArray = new int[3];
                            string[] ThreeMonthRange = new string[3];

                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                int MonthlyQty = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.QuantityOrdered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                    }
                                }
                            }
                            if (BarChartInfo.Any(x => x.SupplierId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemQtyArray,
                                name: BarChartInfo.FirstOrDefault().SupplierName);
                            }
                        };
                    }
                    else if (StationeryId == 0)
                    {
                        foreach (int s in AllSupplierIds)
                        {
                            int[] ItemQtyArray = new int[3];

                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemQtyArray,
                                name: SuNames.First());
                            }


                        };
                    }
                    chart.AddTitle("Monthly Volume Comparison");
                    chart.SetYAxis("Volume");
                }
                else
                {
                    if (StationeryId != 0)
                    {
                        foreach (int s in AllSupplierIds)
                        {


                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, StationeryId);

                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];

                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                int MonthlyQty = 0;
                                decimal MonthlyPrice = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.QuantityOrdered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                        MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                        ItemPriceArray[2 - i] = MonthlyPrice;
                                    }
                                }
                            }
                            if (BarChartInfo.Any(x => x.SupplierId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: BarChartInfo.FirstOrDefault().SupplierName);
                            }

                        };
                    }
                    else if (StationeryId == 0)
                    {
                        foreach (int s in AllSupplierIds)
                        {
                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForSupplier(s, CategoryId, j + 1);
                                if (BarChartInfo.Any(x => x.SupplierId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().SupplierName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDate.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.OrderDateTime.ToString("MMMM yyyy") == TheChosenDate.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.QuantityOrdered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.ItemUnitPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: SuNames.First());
                            }
                        };
                    }
                    chart.AddTitle("Monthly Price Comparison");
                    chart.SetYAxis("Price");
                }

            }
            chart.AddLegend();
            chart.SetXAxis("Time");
            byte[] chartByteArr = chart.GetBytes("jpeg");
            var path = Path.Combine(Server.MapPath("~/Images/Chart"), "chart.jpeg");
            System.IO.File.WriteAllBytes(path, chartByteArr);
        }
        #endregion

        [Authorizer]
        #region BarChartDepartment
        public void BarChartDepartment(List<int> DepartmentIdsForDep, int CategoryIdForDep, int StationeryIdForDep, DateTime TheChosenDateForDep, int trendForDep)
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
            int[] AllDepartmentIds = new int[] { 1, 2, 3, 4, 5, 6, 7 }; //total 7 Departments
            int[] AllStationeryIds = new int[90]; //all stationery items
            if (DepartmentIdsForDep != null)
            {
                int[] ArrayDepartmentIds = DepartmentIdsForDep.ToArray(); //convert input parameter from list to array
                if (trendForDep == 1)
                {
                    if (StationeryIdForDep != 0) // if there is an item selected
                    {
                        foreach (int s in ArrayDepartmentIds)
                        {
                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep); //calling service to pass the list of ChartDTO
                            int[] ItemQtyArray = new int[3]; // for 3 months comparison
                            string[] ThreeMonthRange = new string[3]; // for 3 months comparison


                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"); //convert date into MMMM-yyyy
                                int MonthlyQty = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.RequisitionQuantityDelivered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                    } //sum up the quantity within the month

                                }
                            }
                            if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemQtyArray,
                                name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                            }//add series to chart

                        };
                    }
                    else if (StationeryIdForDep == 0) // if no item selected
                    {
                        foreach (int s in ArrayDepartmentIds)
                        {
                            int[] ItemQtyArray = new int[3];

                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemQtyArray,
                                name: SuNames.First());
                            }

                        };
                    }
                    chart.AddTitle("Department Monthly Volume Comparison");
                    chart.SetYAxis("Volume");
                }
                else
                {
                    if (StationeryIdForDep != 0)
                    {
                        foreach (int s in ArrayDepartmentIds)
                        {


                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep);

                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];

                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                int MonthlyQty = 0;
                                decimal MonthlyPrice = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.RequisitionQuantityDelivered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                        MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                        ItemPriceArray[2 - i] = MonthlyPrice;
                                    }
                                }
                            }
                            if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                            }

                        };
                    }
                    else if (StationeryIdForDep == 0)
                    {
                        foreach (int s in ArrayDepartmentIds)
                        {
                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: SuNames.First());
                            }
                        };
                    }
                    chart.AddTitle("Department Monthly Price Comparison");
                    chart.SetYAxis("Price");

                }
            }
            else
            {
                if (trendForDep == 1)
                {
                    if (StationeryIdForDep != 0)
                    {
                        foreach (int s in AllDepartmentIds)
                        {
                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep);
                            int[] ItemQtyArray = new int[3];
                            string[] ThreeMonthRange = new string[3];

                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                int MonthlyQty = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.RequisitionQuantityDelivered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                    }
                                }
                            }
                            if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemQtyArray,
                                name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                            }
                        };
                    }
                    else if (StationeryIdForDep == 0)
                    {
                        foreach (int s in AllDepartmentIds)
                        {
                            int[] ItemQtyArray = new int[3];

                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                    chartType: "column",
                                    xValue: ThreeMonthRange,
                                    yValues: ItemQtyArray,
                                    name: SuNames.First());
                            }
                        };
                    }
                    chart.AddTitle("Department Monthly Volume Comparison");
                    chart.SetYAxis("Volume");
                }
                else
                {
                    if (StationeryIdForDep != 0)
                    {
                        foreach (int s in AllDepartmentIds)
                        {
                            chartService = new ChartService();
                            List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, StationeryIdForDep);

                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];

                            for (int i = 2; i > -1; i--)
                            {
                                ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                int MonthlyQty = 0;
                                decimal MonthlyPrice = 0;
                                foreach (ChartDTO bci in BarChartInfo)
                                {
                                    if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                    {
                                        MonthlyQty += bci.RequisitionQuantityDelivered;
                                        ItemQtyArray[2 - i] = MonthlyQty;
                                        MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                        ItemPriceArray[2 - i] = MonthlyPrice;
                                    }
                                }
                            }
                            if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName);
                            }

                        };
                    }
                    else if (StationeryIdForDep == 0)
                    {
                        foreach (int s in AllDepartmentIds)
                        {
                            int[] ItemQtyArray = new int[3];
                            decimal[] ItemPriceArray = new decimal[3];
                            string[] ThreeMonthRange = new string[3];
                            List<string> SuNames = new List<string>();
                            for (int j = 0; j < AllStationeryIds.Length; j++)
                            {
                                chartService = new ChartService();
                                List<ChartDTO> BarChartInfo = chartService.TrendChartInfoForDepartment(s, CategoryIdForDep, j + 1);
                                if (BarChartInfo.Any(x => x.RequisitionEmployeeDepartmentId == s))
                                { SuNames.Add(BarChartInfo.FirstOrDefault().RequisitionEmployeeDepartmentName); }
                                for (int i = 2; i > -1; i--)
                                {
                                    ThreeMonthRange[2 - i] = TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy");
                                    int MonthlyQty = 0;
                                    decimal MonthlyPrice = 0;
                                    foreach (ChartDTO bci in BarChartInfo)
                                    {
                                        if (bci.RequisitionDateTime.ToString("MMMM yyyy") == TheChosenDateForDep.AddMonths(-i).ToString("MMMM yyyy"))
                                        {
                                            MonthlyQty += bci.RequisitionQuantityDelivered;
                                            ItemQtyArray[2 - i] = MonthlyQty;
                                            MonthlyPrice += MonthlyQty * bci.RequisitionStationeryItemPrice;
                                            ItemPriceArray[2 - i] = MonthlyPrice;
                                        }
                                    }
                                }
                            }
                            if (SuNames.Count > 0)
                            {
                                chart.AddSeries(
                                chartType: "column",
                                xValue: ThreeMonthRange,
                                yValues: ItemPriceArray,
                                name: SuNames.First());
                            }
                        };
                    }
                    chart.AddTitle("Department Monthly Price Comparison");
                    chart.SetYAxis("Price");
                }

            }
            chart.AddLegend();
            chart.SetXAxis("Time");
            byte[] chartByteArr = chart.GetBytes("jpeg");
            var path = Path.Combine(Server.MapPath("~/Images/Chart"), "chart.jpeg");
            System.IO.File.WriteAllBytes(path, chartByteArr);
        }
        #endregion
    }
}