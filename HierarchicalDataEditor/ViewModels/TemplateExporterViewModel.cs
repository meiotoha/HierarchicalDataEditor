using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HierarchicalDataEditor.Core.Helpers;
using HierarchicalDataEditor.Core.Models;
using HierarchicalDataEditor.Core.Services;

namespace HierarchicalDataEditor.ViewModels
{
    public class TemplateExporterViewModel : ViewModelBase
    {
        public TemplateExporterViewModel()
        {
        }

        private string _template;

        public string Template
        {
            get { return _template; }
            set { _template = value; RaisePropertyChanged(); }
        }

        private ICommand _exportCommand;

        public ICommand ExportCommand
        {
            get { return _exportCommand ?? (_exportCommand = new RelayCommand(Export)); }
        }
        private async void Export()
        {
            List<string> result = new List<string>();
            var source = GlobalDataService.Instance.CurrentProject.Nodes;
            foreach (var node in source)
            {
                Convert(node, result);
            }

            try
            {
                var picker = new Windows.Storage.Pickers.FileSavePicker();
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeChoices.Add("Output File", new List<string> { ".txt" });
                Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    await Windows.Storage.FileIO.WriteLinesAsync(file, result);
                    Windows.Storage.Provider.FileUpdateStatus status =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        await new ContentDialog
                        {
                            Content = "Update File Error",
                        }.ShowAsync();
                    }
                }
            }
            catch (Exception e)
            {
                await new ContentDialog
                {
                    Title   = "Save Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }

        }

        private void Convert(TreeNode node, List<string> container)
        {
            var txt = Template.ParseText(node, node.CoreData);
            container.Add(txt);
            foreach (var item in node.Items)
            {
                Convert(item, container);
            }
        }
    }
}
