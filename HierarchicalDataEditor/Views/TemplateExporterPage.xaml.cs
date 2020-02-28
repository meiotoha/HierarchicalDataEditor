using System;

using HierarchicalDataEditor.ViewModels;

using Windows.UI.Xaml.Controls;

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
    }
}
