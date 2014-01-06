namespace Hbo.Sheepish
{
    using System;
    using System.ComponentModel;

    class ViewModel : INotifyPropertyChanged
    {
        private string _primaryQuery;
        private string _secondaryQuery;
        private int _primaryCount;
        private int _secondaryCount;
        private string _primaryScope;
        private string _secondaryScope;

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

        public string PrimaryScope
        {
            get { return _primaryScope; }
            set
            {
                _primaryScope = value;
                _NotifyPropertyChanged("PrimaryScope");
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

        public string SecondaryScope
        {
            get { return _secondaryScope; }
            set
            {
                _secondaryScope = value;
                _NotifyPropertyChanged("SecondaryScope");
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
