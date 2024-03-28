using System.Collections;

namespace Test.Models;

public class CategoryField
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual IList<Category>? Categories { get; set; }
}