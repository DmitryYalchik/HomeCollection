using HomeCollection.Models;

namespace HomeCollection.Stores
{
    public class DataStore
    {
        #region CurrentBuilding
        private Building _currentBuilding;
        public Building CurrentBuilding
        {
            get { return _currentBuilding; }
            set { _currentBuilding = value; }
        }
        #endregion

        #region CurrentEnterance
        private Enterance _currentEnterance;
        public Enterance CurrentEnterance
        {
            get { return _currentEnterance; }
            set { _currentEnterance = value; }
        }
        #endregion

        #region CurrentFlat
        private Flat _currentFlat;
        public Flat CurrentFlat
        {
            get { return _currentFlat; }
            set { _currentFlat = value; }
        }
        #endregion

#warning УДАЛИТЬ ЧИ ШО
        private People _currentPeople;
        public People CurrentPeople
        {
            get { return _currentPeople; }
            set { _currentPeople = value; }
        }
    }
}
