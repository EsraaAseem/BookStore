using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullkyB.Model
{
    public class coverType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string coverName { get; set; }
    }
}
