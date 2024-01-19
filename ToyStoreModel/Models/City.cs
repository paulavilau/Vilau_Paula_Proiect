namespace Vilau_Paula_Proiect.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public ICollection<Client>? Clients { get; set; }
        //public ICollection<Supplier>? Suppliers { get; set; }
    }
}
