using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class ReceiveDoDTO
    {
        public ErrorDTO error { get; set; }
        public PurchaseOrder purchaseOrder { get; set; }
        public HttpPostedFileBase DO { get; set; }
        public HttpPostedFileBase Invoice { get; set; }
    
    }
}