using HomeCollection.Models.Base;

namespace HomeCollection.Models
{
    public class Flat : Model
    {
        public int Number { get; set; }

        public virtual ICollection<People> Peoples { get; set; }

        public string EnteranceId { get; set; }
        public virtual Enterance Enterance { get; set; }
    }
}