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

    public enum UOM
    {
        Box = 0,
        Dozen = 1,
        Each = 2,
        Packet = 3,
        Set = 4,
        Other = 5
    }

    public enum ActiveStatus
    {
        INACTIVE = 0,
        ACTIVE = 1
    }

    public enum POStatus
    {
        OPEN = 0,
        PENDING = 1,
        APPROVED = 2,
        REJECTED = 3,
        CANCELLED = 4,
        CLOSED = 5
    }
}