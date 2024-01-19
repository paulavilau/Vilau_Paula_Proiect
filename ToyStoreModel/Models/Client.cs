namespace Vilau_Paula_Proiect.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public City? City { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
