using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.MobileDTOs
{
    public class RetrievalDTO
    {
        public DTOs.LoginDTO LoginDTO { get; set; }

        public string RetrievalDate { get; set; }
        public string GeneratedBy { get; set; }
        public int AdHocRetrievalId { get; set; }

        public List<RetrievalItemDTO> RetrievalItem { get; set; }
    }
}