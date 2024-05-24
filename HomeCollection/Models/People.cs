using HomeCollection.Models.Base;

namespace HomeCollection.Models
{
    public class People : Model
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateBirth { get; set; }

        public string FlatId { get; set; }
        public virtual Flat Flat { get; set; }
    }
}