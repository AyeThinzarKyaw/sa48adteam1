using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class EnumDTO
    {
        public enum UOM
        {
            Box=0,
            Dozen=1,
            Each=2,
            Packet=3,
            Set=4,
            Other=5
        }

        public enum ActiveStatus
        {
            INACTIVE = 0,
            ACTIVE = 1
        }
    }
}