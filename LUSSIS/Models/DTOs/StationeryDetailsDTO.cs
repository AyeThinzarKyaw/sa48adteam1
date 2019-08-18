using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class StationeryDetailsDTO
    {
        public ErrorDTO Error { get; set; }

        public int StationeryId { get; set; }

        [Required(ErrorMessage = "Code cannot be blank.")]
        public String Code { get; set; }

        [Required(ErrorMessage = "Description cannot be blank.")]
        public String Description { get; set; }

        public String Bin { get; set; }          
        
        public int CategoryId { get; set; }

        public int Supplier1 { get; set; }

        public int Supplier2 { get; set; }

        public int Supplier3 { get; set; }

        [Required(ErrorMessage = "Price cannot be blank.")]
        public decimal Price1 { get; set; }

        [Required(ErrorMessage = "Price cannot be blank.")]
        public decimal Price2 { get; set; }

        [Required(ErrorMessage = "Price cannot be blank.")]
        public decimal Price3 { get; set; }

        public int UOM { get; set; }      
        
        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<Supplier> Suppliers { get; set; }
    }
}