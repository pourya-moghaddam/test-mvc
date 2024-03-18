using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models;

public class Product
{
    public int Id { get; set; }

    [StringLength(60, MinimumLength = 3)]
    public string Name { get; set; }
    [StringLength(160)]
    public string? Description { get; set; }
    public string? Color { get; set; }
    public byte[]? Picture { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
}
