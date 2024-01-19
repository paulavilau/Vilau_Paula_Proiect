namespace Vilau_Paula_Proiect.Models.ToyStoreViewModels
{
    public class OrdersIndexData
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Toy> Toys { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
