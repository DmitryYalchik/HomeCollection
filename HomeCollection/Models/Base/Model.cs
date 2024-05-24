using System.ComponentModel.DataAnnotations;

namespace HomeCollection.Models.Base
{
    public class Model
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
