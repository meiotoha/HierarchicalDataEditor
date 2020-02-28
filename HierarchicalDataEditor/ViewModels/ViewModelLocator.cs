using System;

using GalaSoft.MvvmLight.Ioc;

using HierarchicalDataEditor.Services;
using HierarchicalDataEditor.Views;

namespace HierarchicalDataEditor.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());

        private ViewModelLocator()
        {
            SimpleIoc.Default.Register(() => new NavigationServiceEx());
            Register<SchemaEditorViewModel, SchemaEditorPage>();
            Register<TreeViewEditorViewModel, TreeViewEditorPage>();
            Register<TemplateExporterViewModel, TemplateExporterPage>();
        }

        public TemplateExporterViewModel TemplateExporterViewModel => SimpleIoc.Default.GetInstance<TemplateExporterViewModel>();

        public TreeViewEditorViewModel TreeViewEditorViewModel => SimpleIoc.Default.GetInstance<TreeViewEditorViewModel>();

        public SchemaEditorViewModel SchemaEditorViewModel => SimpleIoc.Default.GetInstance<SchemaEditorViewModel>();

        public NavigationServiceEx NavigationService => SimpleIoc.Default.GetInstance<NavigationServiceEx>();

        public void Register<VM, V>()
            where VM : class
        {
            SimpleIoc.Default.Register<VM>();

            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
