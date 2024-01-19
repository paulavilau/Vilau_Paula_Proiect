using Microsoft.EntityFrameworkCore;
using Vilau_Paula_Proiect.Data;
using Vilau_Paula_Proiect.Models;
namespace Nume_Pren_Lab2.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new
           ToyStoreContext(serviceProvider.GetRequiredService
            <DbContextOptions<ToyStoreContext>>()))
            {
                if (context.Toys.Any())
                {
                    return; // BD a fost creata anterior
                }
                context.Toys.AddRange(
                new Toy
                { }


                );

                context.SaveChanges();
            }
        }
    }
}