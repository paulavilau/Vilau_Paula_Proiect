using System.ComponentModel.DataAnnotations.Schema;

namespace Vilau_Paula_Proiect.Models
{
    public class Toy
    {
        public int ToyID { get; set; }
        public string ?Name { get; set; }
        public string ?Description { get; set; }
        public int ?SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public string ?Image { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category ?Category { get; set; }
        public ICollection<Review>? Reviews { get; set; }

        public ICollection<OrderedToy>? OrderedToys { get; set; }

        [NotMapped] 
        public double AverageStars
        {
            get
            {
                if (Reviews != null && Reviews.Any())
                {
                    return Math.Round(Reviews.Average(r => r.Stars),2);
                }
                return 0; 
            }
        }

    }
}
