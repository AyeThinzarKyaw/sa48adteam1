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
        public bool SelectOnlyConfirm { get; set; }
        public List<KeyValuePair<int, DateTime>> EstimatedDates { get; set; }

        public POCreateDTO()
        {
            this.SelectOnlyConfirm = false;
            this.SelectedItems = new List<Stationery>();
            this.ConfirmedPOs = new List<PurchaseOrder>();
            this.EstimatedDates = new List<KeyValuePair<int, DateTime>>();
        }
    }
}