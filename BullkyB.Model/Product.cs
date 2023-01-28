using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullkyB.Model
{
    public class Product 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1, 2000, ErrorMessage = "Range Must Be Between 1:2000")]

        public double Price { get; set; }
        [Required]
        [Range(1, 2000, ErrorMessage = "Range Must Be Between 1:2000")]

        public double ListPrice { get; set; }
        [Required]
        [Range(1, 2000, ErrorMessage = "Range Must Be Between 1:2000")]

        public double Price50 { get; set; }
        [Required]
        [Range(1,2000,ErrorMessage ="Range Must Be Between 1:2000")]
        public double Price100 { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category category { get; set; }
        public int coveTypeId { get; set; }
        [ForeignKey("coveTypeId")]
        [ValidateNever]
        public coverType CoverType { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }

    }
}
