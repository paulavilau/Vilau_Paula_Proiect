namespace Vilau_Paula_Proiect.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public City? City { get; set; }
        public ICollection<Toy>? Toys { get; set; }
    }
}
