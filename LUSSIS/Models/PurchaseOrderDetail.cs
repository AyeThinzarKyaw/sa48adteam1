//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LUSSIS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseOrderDetail
    {
        public int PurchaseOrderId { get; set; }
        public int StationeryId { get; set; }
        public int QuantityOrdered { get; set; }
        public Nullable<int> QuantityDelivered { get; set; }
    
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Stationery Stationery { get; set; }
    }
}
