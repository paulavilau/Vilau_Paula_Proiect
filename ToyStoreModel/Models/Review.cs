namespace Vilau_Paula_Proiect.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public DateTime ReviewDate { get; set; }
        public int ?ClientId { get; set; }
        public Client ?Client { get; set; }
        public int? ToyId { get; set; }
        public Toy? Toy { get; set; }
        public int Stars { get; set; }
        public string Text {  get; set; } 
    }
}
