namespace Hbo.Sheepish
{
    using System.Collections.Generic;
    using System.ComponentModel;

    class ViewModel : INotifyPropertyChanged
    {
        private string _primaryQuery;
        private string _secondaryQuery;
        private int _primaryCount;
        private int _secondaryCount;
        private YouTrackService.SavedSearch _primaryScope;
        private YouTrackService.SavedSearch _secondaryScope;
        private IList<YouTrackService.SavedSearch> _scopes;
        private bool _showingEditDialog = false;

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

        public YouTrackService.SavedSearch PrimaryScope
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

        public YouTrackService.SavedSearch SecondaryScope
        {
            get { return _secondaryScope; }
            set
            {
                _secondaryScope = value;
                _NotifyPropertyChanged("SecondaryScope");
            }
        }

        public IList<YouTrackService.SavedSearch> Scopes
        {
            get { return _scopes; }
            set
            {
                _scopes = value;
                _NotifyPropertyChanged("Scopes");
            }
        }

        public bool ShowingEditDialog
        {
            get { return _showingEditDialog; }
            set 
            {
                _showingEditDialog = value;
                _NotifyPropertyChanged("ShowingEditDialog");
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
