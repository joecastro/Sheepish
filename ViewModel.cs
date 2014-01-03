
namespace Hbo.Sheepish
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class ViewModel : INotifyPropertyChanged
    {
        private string _primaryQuery;
        private string _secondaryQuery;

        private int _primaryCount;
        private int _secondaryCount;

        public string PrimaryQuery
        {
            get { return _primaryQuery; }
            set
            {
                _primaryQuery = value;
                _NotifyPropertyChanged("PrimaryQuery");
            }
        }

        public int PrimaryCount
        {
            get { return _primaryCount; }
            set
            {
                _primaryCount = value;
                _NotifyPropertyChanged("PrimaryCount");
            }
        }

        public string SecondaryQuery
        {
            get { return _secondaryQuery; }
            set
            {
                _secondaryQuery = value;
                _NotifyPropertyChanged("SecondaryQuery");
            }
        }

        public int SecondaryCount
        {
            get { return _secondaryCount; }
            set
            {
                _secondaryCount = value;
                _NotifyPropertyChanged("SecondaryCount");
            }
        }

        #region INotifyPropertyChanged implementation

        private void _NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
