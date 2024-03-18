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
                Name = "گوشی الکی 1",
                Description = null,
                Color = "black",
                Picture = ReadBytesFromFile("wwwroot\\img\\cartoon-pictures-of-cell-phones-33.png"),
                Price = 123456789M
            },
            new Product
            {
                Name = "گوشی الکی 2",
                Description = null,
                Color = "blue",
                Picture = ReadBytesFromFile("wwwroot\\img\\cartoon-pictures-of-cell-phones-33.png"),
                Price = 987654321M
            },
            new Product
            {
                Name = "گوشی هوشمند گرون",
                Description = "گوشی خیلی گرون",
                Color = "white",
                Picture = ReadBytesFromFile("wwwroot\\img\\smartphone-png-550x620_c42612e1_transparent_202168.png.png"),
                Price = 27000000M
            },
            new Product
            {
                Name = "گوشی هوشمند ارزون",
                Description = "گوشی بدرد نخور",
                Color = "red",
                Picture = ReadBytesFromFile("wwwroot\\img\\smartphone_PNG8533.png"),
                Price = 121000M
            }
        );
        context.SaveChanges();
    }
}