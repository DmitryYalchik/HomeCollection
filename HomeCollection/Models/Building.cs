using HomeCollection.Models.Base;

namespace HomeCollection.Models
{
    public class Building : Model
    {
        public string Address { get; set; }
        public string ShortName { get; set; }

        public virtual ICollection<Enterance> Enterances { get; set; }
    }
}
