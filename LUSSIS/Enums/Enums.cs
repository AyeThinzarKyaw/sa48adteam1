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
        Box,
        Dozen,
        Each,
        Packet,
        Set,
        Other
    }

    public enum ActiveStatus
    {
        INACTIVE,
        ACTIVE
    }

    public enum POStatus
    {
        OPEN,
        PENDING,
        APPROVED,
        REJECTED,
        CANCELLED,
        CLOSED 
    }

    public enum AdjustmentVoucherDefaultReasons
    {
        RETRIEVAL_SHORTAGE,
        DISBURSEMENT_SHORTAGE

    }

    public enum RequisitionDetailStatusEnum
    {
        RESERVED_PENDING,
        WAITLIST_PENDING,
        PREPARING,
        WAITLIST_APPROVED,
        PENDING_COLLECTION,
        COLLECTED,
        REJECTED,
        CANCELLED
    }

    public enum RequisitionStatusEnum
    {
        PENDING,
        APPROVED,
        COMPLETED,
        REJECTED,
        CANCELLED
    }

    public enum AdjustmentVoucherStatus
    {
        Open,
        Submitted,
        Acknowledged
    }

    public enum Roles
    {
        DepartmentHead=1,
        DepartmentStaff=2,
        DepartmentRepresentative=3,
        DepartmentCoverHead=4,
        StoreManager=5,
        StoreSupervisor=6,
        StoreClerk=7
    }
}