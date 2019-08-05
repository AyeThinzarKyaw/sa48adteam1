using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class RetrievalItemDTO
    {
        // hidden in View
        public int StationeryId { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }
        public int NeededQuantity { get; set; }
        public int RetrievedQty { get; set; }

        public List<RetrievalPrepItemDTO> RetrievalPrepItemList { get; set; }
    }
}