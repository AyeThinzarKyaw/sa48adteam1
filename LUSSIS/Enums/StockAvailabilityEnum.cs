using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Enums
{
    public enum StockAvailabilityEnum
    {
        OutOfStock,
        LowStock,
        InStock
    }

    public enum RequisitionStatusEnum
    {
        PENDING,
        CANCELLED,
        APPROVED,
        REJECTED,
        COMPLETED
    }

    public enum RequisitionDetailStatusEnum
    {
        RESERVED_PENDING,
        WAITLIST_PENDING,
        WAITLIST_APPROVED,
        PREPARING,
        PENDING_COLLECTION,
        COLLECTED,
        CANCELLED,
        REJECTED
    }
}