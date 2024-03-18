using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class ImageFileModel
    {
        [DataType(DataType.Upload)]
        public IFormFile? File { get; set; }
    }
}
