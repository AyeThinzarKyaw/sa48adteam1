using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class POCreateDTO
    {
        public List<PO_getPOCatalogue_Result> Catalogue { get; set; }
        public List<Stationery> SelectedItems { get; set; }
        public List<PurchaseOrder> ConfirmedPOs { get; set; }
        public bool selectOnlyConfirm { get; set; }

        public POCreateDTO()
        {
            this.selectOnlyConfirm = false;
            this.SelectedItems = new List<Stationery>();
            this.ConfirmedPOs = new List<PurchaseOrder>();
        }
    }
}