namespace Vilau_Paula_Proiect.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Toy>? Toys { get; set; }
    }
}
