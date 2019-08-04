using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class SupplierDetailsDTO
    {
        public ErrorDTO Error { get; set; }
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "Code cannot be blank.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name cannot be blank.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "ContactName cannot be blank.")]
        public string ContactName { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string GST_No { get; set; }
    }
}