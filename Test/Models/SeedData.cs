using Microsoft.EntityFrameworkCore;
using Test.Data;
using System.Drawing;

namespace Test.Models;

public static class SeedData
{
    private static byte[] ReadBytesFromFile(string filePath)
    {
        Image image = Image.FromFile(filePath);
        using MemoryStream ms = new MemoryStream();
        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
    }
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new TestContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<TestContext>>());
        if (context.Product.Any())
        {
            return;
        }
            
        context.Product.AddRange(
            new Product
            {
                Name = "Samsung - Galaxy Book2 Pro 360 2-in-1",
                Description = "Samsung Galaxy Book",
                Color = "Silver",
                Picture = ReadBytesFromFile("wwwroot\\img\\3WJBPrRoqmkhkFA3oRe9Yc.png"),
                Price = 1200M
            },
            new Product
            {
                Name = "Apple - MacBook Pro",
                Description = "Latest Macbook Pro",
                Color = "Space Black",
                Picture = ReadBytesFromFile("wwwroot\\img\\MacBook-Pro-Space-Black-M3-Pro.png"),
                Price = 1999M
            }
        );
        context.SaveChanges();
    }
}