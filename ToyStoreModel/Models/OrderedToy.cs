using System.ComponentModel.DataAnnotations.Schema;

namespace Vilau_Paula_Proiect.Models
{
    public class OrderedToy
    {
        public int? ToyID { get; set; }
        public Toy? Toy { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public int? Quantity { get; set; }
    }

}
