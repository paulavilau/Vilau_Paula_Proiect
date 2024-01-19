namespace Vilau_Paula_Proiect.Models.ToyStoreViewModels
{
    public class CartsIndexData
    {
        public IEnumerable<Cart> Carts { get; set; }
        public IEnumerable<Toy> Toys { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
