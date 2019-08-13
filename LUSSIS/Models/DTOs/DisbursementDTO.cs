using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class DisbursementDTO
    {
        public List<DisbursementDetailsDTO> DisbursementDetailsDTOList { get; set; }
        public LoginDTO LoginDTO { get; set; }
        public int DisbursementId { get; set; }
        public int DeliveredEmployeeId { get; set; }
        public int ReceivedEmployeeId { get; set; }
    }
}