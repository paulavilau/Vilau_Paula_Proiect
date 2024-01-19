using Microsoft.AspNetCore.Identity;

namespace Vilau_Paula_Proiect.Models
    
{
    public class Order
    {
        public int? OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string? ClientEmail { get; set; }
        public string? Address { get; set; }
        public string? UserId { get; set; }
        //public int? ClientId { get; set; }
        //public Client? Client { get; set; }
        public ICollection<OrderedToy>? OrderedToys { get; set; }
    }
}
