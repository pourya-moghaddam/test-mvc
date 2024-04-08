namespace Test.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public virtual Category? Parent { get; set; }

    public virtual ICollection<Category>? Children { get; set; }
    public virtual IList<CategoryField>? Fields { get; set; }
}
