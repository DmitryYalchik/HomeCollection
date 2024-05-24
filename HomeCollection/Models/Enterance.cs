using HomeCollection.Models.Base;

namespace HomeCollection.Models
{
    public class Enterance : Model
    {
        public int Number { get; set; }

        public virtual ICollection<Flat> Flats { get; set; }

        public string BuildingId { get; set; }
        public virtual Building Building { get; set; }
    }
}