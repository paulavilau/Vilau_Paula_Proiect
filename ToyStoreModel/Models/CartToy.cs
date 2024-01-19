using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vilau_Paula_Proiect.Models
{
    public class CartToy
    {
        public int? ToyID { get; set; }
        public Toy? Toy { get; set; }
        public int? CartId { get; set; }
        public Cart? Cart { get; set; }
        public int? Quantity { get; set; }

        [NotMapped]
        public decimal? Value {
            get {
                return Math.Round((decimal)(Toy.Price * Quantity), 2);
            }
            set { }
            } 
    }
}
