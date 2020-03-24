using System;

using HierarchicalDataEditor.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using HierarchicalDataEditor.Core.Models;

namespace HierarchicalDataEditor.Views
{
    public sealed partial class TemplateExporterPage : Page
    {
        private TemplateExporterViewModel ViewModel
        {
            get { return ViewModelLocator.Current.TemplateExporterViewModel; }
        }

        public TemplateExporterPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is TreeNode nd)
            {
                ViewModel.Parameters = nd.Items;
            }
            else
            {
                ViewModel.Parameters = null;
            }
        }
    }
}
