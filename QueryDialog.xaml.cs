namespace Hbo.Sheepish
{
    using System.Windows;

    public partial class QueryDialog
    {
        public QueryDialog()
        {
            InitializeComponent();

            // Not binding any of these in XAML because I don't want live updates.
            // The user must explicitly save the changes.
            PrimaryScopeSelect.ItemsSource = ServiceProvider.ViewModel.Scopes;
            PrimaryScopeSelect.SelectedItem = ServiceProvider.ViewModel.PrimaryScope;

            PrimaryQueryBox.Text = ServiceProvider.ViewModel.PrimaryQuery;

            SecondaryScopeSelect.ItemsSource = ServiceProvider.ViewModel.Scopes;
            SecondaryScopeSelect.SelectedItem = ServiceProvider.ViewModel.SecondaryScope;

            SecondaryQueryBox.Text = ServiceProvider.ViewModel.SecondaryQuery;

            // Prevent vertical resize
            Loaded += (sender, e) =>
            {
                MinHeight = ActualHeight;
                MaxHeight = ActualHeight;
            };
        }

        private void _OnSaveClicked(object sender, RoutedEventArgs e)
        {
            ServiceProvider.ViewModel.PrimaryScope = (YouTrackService.SavedSearch)PrimaryScopeSelect.SelectedItem;
            ServiceProvider.ViewModel.SecondaryScope = (YouTrackService.SavedSearch)SecondaryScopeSelect.SelectedItem;
            ServiceProvider.ViewModel.PrimaryQuery = PrimaryQueryBox.Text.Trim();
            ServiceProvider.ViewModel.SecondaryQuery = SecondaryQueryBox.Text.Trim();

            this.Close();
        }

        private void _OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
