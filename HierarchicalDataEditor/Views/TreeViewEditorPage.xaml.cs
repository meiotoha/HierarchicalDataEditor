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
        private TreeViewEditorViewModel ViewModel
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

   

        private void ClearChildren(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.ClearChildrenCommand.Execute(((sender) as MenuFlyoutItem).DataContext);
        }

        private void AddChild(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.AddChildCommand.Execute(((sender) as MenuFlyoutItem).DataContext);
        }

        private void AddAfter(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.AddAfterCommand.Execute(((sender) as MenuFlyoutItem).DataContext);
        }

        private void Delete(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.DeleteCommand.Execute(((sender) as MenuFlyoutItem).DataContext);
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

        private void BatchAddChild(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.BatchAddChildCommand.Execute(((sender) as MenuFlyoutItem).DataContext);
        }
    }
}
