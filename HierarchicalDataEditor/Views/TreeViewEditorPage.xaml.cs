using System;

using HierarchicalDataEditor.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HierarchicalDataEditor.Views
{
    // For more info about the TreeView Control see
    // https://docs.microsoft.com/windows/uwp/design/controls-and-patterns/tree-view
    // For other samples, get the XAML Controls Gallery app http://aka.ms/XamlControlsGallery
    public sealed partial class TreeViewEditorPage : Page
    {
        public TreeViewEditorViewModel ViewModel
        {
            get { return ViewModelLocator.Current.TreeViewEditorViewModel; }
        }

        public TreeViewEditorPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.Load();
        }

        private void TreeViewItem_DropCompleted(Windows.UI.Xaml.UIElement sender, Windows.UI.Xaml.DropCompletedEventArgs args)
        {
            ViewModel.FixNodes();
        }

        private void treeView_DropCompleted(Windows.UI.Xaml.UIElement sender, Windows.UI.Xaml.DropCompletedEventArgs args)
        {
            ViewModel.FixNodes();
        }

        private void treeView_DragItemsCompleted(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewDragItemsCompletedEventArgs args)
        {
            ViewModel.FixNodes();
        }
    
    }
}
