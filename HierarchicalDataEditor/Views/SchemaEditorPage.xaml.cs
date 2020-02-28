using System;

using HierarchicalDataEditor.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HierarchicalDataEditor.Views
{
    public sealed partial class SchemaEditorPage : Page
    {
        private SchemaEditorViewModel ViewModel
        {
            get { return ViewModelLocator.Current.SchemaEditorViewModel; }
        }

        // TODO WTS: Change the grid as appropriate to your app, adjust the column definitions on SchemaEditorPage.xaml.
        // For more details see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid
        public SchemaEditorPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.Load();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.DeleteCommand.Execute((sender as Button).DataContext);
        }
    }
}
