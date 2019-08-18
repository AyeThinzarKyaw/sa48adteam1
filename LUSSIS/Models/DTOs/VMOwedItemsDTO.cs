using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class VMOwedItemsDTO
    {
        public LoginDTO LoginDTO { get; set; }

        public List<DeptOwedItemDTO> DeptOwedItems { get; set; }
    }
}