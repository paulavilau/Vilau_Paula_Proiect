using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vilau_Paula_Proiect.Models;

namespace Vilau_Paula_Proiect.Models
{
    public class Cart
    {
        public int? CartID { get; set; }
        public DateTime CartDate { get; set; }
        public string? UserId { get; set; }
        public ICollection<CartToy>? CartToys { get; set; }
    }
}
