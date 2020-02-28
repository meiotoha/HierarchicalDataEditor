using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using HierarchicalDataEditor.Core.Models;
using HierarchicalDataEditor.Core.Services;
using HierarchicalDataEditor.Helpers;
using HierarchicalDataEditor.Services;

using Microsoft.Toolkit.Uwp.Helpers;

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using HierarchicalDataEditor.Views;

namespace HierarchicalDataEditor.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
        private IList<KeyboardAccelerator> _keyboardAccelerators;

        private ICommand _newProjectCommand;
        private ICommand _openProjectCommand;
        private ICommand _saveProjectCommand;
        private ICommand _importSchemaCommand;
        private ICommand _exportSchemaCommand;
        private ICommand _importDataCommand;
        private ICommand _exportDataCommand;
        private ICommand _exportDataWithTemplateCommand;
        private ICommand _exportDataWithCsvCommand;
        public ICommand NewProjectCommand => _newProjectCommand ?? (_newProjectCommand = new RelayCommand(NewProject));
        public ICommand OpenProjectCommand => _openProjectCommand ?? (_openProjectCommand = new RelayCommand(OpenProject));
        public ICommand SaveProjectCommand => _saveProjectCommand ?? (_saveProjectCommand = new RelayCommand(SaveProject));
        public ICommand ImportSchemaCommand => _importSchemaCommand ?? (_importSchemaCommand = new RelayCommand(ImportSchema));
        public ICommand ExportSchemaCommand => _exportSchemaCommand ?? (_exportSchemaCommand = new RelayCommand(ExportSchema));
        public ICommand ImportDataCommand => _importDataCommand ?? (_importDataCommand = new RelayCommand(ImportData));
        public ICommand ExportDataCommand => _exportDataCommand ?? (_exportDataCommand = new RelayCommand(ExportData));
        public ICommand ExportDataWithTemplateCommand => _exportDataWithTemplateCommand ?? (_exportDataWithTemplateCommand = new RelayCommand(ExportDataEx));
        public ICommand ExportDataWithCSVCommand => _exportDataWithCsvCommand ?? (_exportDataWithCsvCommand = new RelayCommand(ImportDataEx));

        private async void ImportDataEx()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".csv");
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    using (var fs = await file.OpenReadAsync())
                    {
                        var text = await fs.ReadTextAsync(Encoding.UTF8);
                        var arrs = text.Split("\n");
                        var result = new List<TreeNodeFlatData>();
                        foreach (var txt in arrs)
                        {
                            var strs = txt.Split(",");
                            if (strs.Length == 3)
                            {
                                var x = new TreeNodeFlatData { ParentCode = strs[0].Trim(), Code = strs[1].Trim(), Name = strs[2].Trim() };
                                result.Add(x);
                            }
                            else if (strs.Length == 2)
                            {
                                var x = new TreeNodeFlatData { ParentCode = strs[0].Trim(), Code = strs[1].Trim(), Name = strs[1].Trim() };
                                result.Add(x);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        GlobalDataService.Instance.CurrentProject.Nodes = BuildTree(result);
                        MenuNavigationHelper.UpdateView(typeof(TreeViewEditorViewModel).FullName, "reload" + DateTime.Now.Ticks);
                    }
                }
            }
            catch (Exception e)
            {
                await new ContentDialog
                {
                    Title = "Load Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }
        }

        private ObservableCollection<TreeNode> BuildTree(List<TreeNodeFlatData> source)
        {
            var root = source.Where(x => x.ParentCode == "0").ToList();
            var result = new ObservableCollection<TreeNode>();
            foreach (var flatData in root)
            {
                var node = new TreeNode() {Code = flatData.Code, DisplayName = flatData.Name};
                BuildChildren(node, source);
                result.Add(node);
            }
            return result;
        }

        private void BuildChildren(TreeNode node, List<TreeNodeFlatData> source)
        {
            var children = source.Where(x => x.ParentCode == node.Code).ToList();
            foreach (var child in children)
            {
                var chNode = new TreeNode(node){ Code = child.Code, DisplayName = child.Name};
                BuildChildren(chNode, source);
                node.Items.Add(chNode);
            }
        }


        private void NewProject()
        {
            GlobalDataService.Instance.CurrentProjectFile = null;
            GlobalDataService.Instance.CurrentProject = new HDEProject();
            MenuNavigationHelper.UpdateView(typeof(TreeViewEditorViewModel).FullName, "reload" + DateTime.Now.Ticks);
        }
        private async void OpenProject()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".hdeproj");
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    using (var fs = await file.OpenReadAsync())
                    {
                        using (var sr = new StreamReader(fs.AsStreamForRead()))
                        {
                            var alltext = await sr.ReadToEndAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<HDEProject>(alltext);
                            if (result != null)
                            {
                                GlobalDataService.Instance.CurrentProject = result;
                                GlobalDataService.Instance.CurrentProjectFile = file.Path;
                                MenuNavigationHelper.UpdateView(typeof(TreeViewEditorViewModel).FullName, "reload" + DateTime.Now.Ticks);
                            }
                            else
                            {
                                throw new Exception("parse failed");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await new ContentDialog
                {
                    Title = "Load Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }
        }

        private async void SaveProject()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileSavePicker();
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeChoices.Add("HDE Project File", new List<string> { ".hdeproj" });
                Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    var obj = GlobalDataService.Instance.CurrentProject;
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    await Windows.Storage.FileIO.WriteTextAsync(file, json);
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
                    Title = "Save Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }
        }

        private async void ImportSchema()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".hdes");
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    using (var fs = await file.OpenReadAsync())
                    {
                        using (var sr = new StreamReader(fs.AsStreamForRead()))
                        {
                            var alltext = await sr.ReadToEndAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSchema>(alltext);
                            if (result != null)
                            {
                                GlobalDataService.Instance.CurrentProject.Schema = result;
                                MenuNavigationHelper.UpdateView(typeof(SchemaEditorViewModel).FullName, "reload" + DateTime.Now.Ticks);
                            }
                            else
                            {
                                throw new Exception("parse failed");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await new ContentDialog
                {
                    Title = "Load Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }
        }

        private async void ExportSchema()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileSavePicker();
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeChoices.Add("HDE Schema File", new List<string> { ".hdes" });
                Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    var obj = GlobalDataService.Instance.CurrentProject.Schema;
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    await Windows.Storage.FileIO.WriteTextAsync(file, json);
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
                    Title = "Save Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }
        }

        private async void ImportData()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".hdex");
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    using (var fs = await file.OpenReadAsync())
                    {
                        using (var sr = new StreamReader(fs.AsStreamForRead()))
                        {
                            var alltext = await sr.ReadToEndAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<TreeNode>>(alltext);
                            if (result != null)
                            {
                                GlobalDataService.Instance.CurrentProject.Nodes = result;
                                MenuNavigationHelper.UpdateView(typeof(TreeViewEditorViewModel).FullName, "reload" + DateTime.Now.Ticks);
                            }
                            else
                            {
                                throw new Exception("parse failed");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await new ContentDialog
                {
                    Title = "Load Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }
        }

        private async void ExportData()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileSavePicker();
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeChoices.Add("HDE Data File", new List<string> { ".hdex" });
                Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    var obj = GlobalDataService.Instance.CurrentProject.Nodes;
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    await Windows.Storage.FileIO.WriteTextAsync(file, json);
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
                    Title = "Save Failed",
                    Content = $"{e.Message}",
                }.ShowAsync();
            }
        }

        private async void ExportDataEx()
        {
           await MenuNavigationHelper.OpenInDialog(typeof(TemplateExporterPage));
        }


        private ICommand _loadedCommand;
        private ICommand _menuViewsSchemaEditorCommand;
        private ICommand _menuViewsTreeViewEditorCommand;
        private ICommand _menuViewsTemplateExporterCommand;
        private ICommand _menuFileExitCommand;

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand MenuViewsSchemaEditorCommand => _menuViewsSchemaEditorCommand ?? (_menuViewsSchemaEditorCommand = new RelayCommand(OnMenuViewsSchemaEditor));

        public ICommand MenuViewsTreeViewEditorCommand => _menuViewsTreeViewEditorCommand ?? (_menuViewsTreeViewEditorCommand = new RelayCommand(OnMenuViewsTreeViewEditor));

        public ICommand MenuViewsTemplateExporterCommand => _menuViewsTemplateExporterCommand ?? (_menuViewsTemplateExporterCommand = new RelayCommand(OnMenuViewsTemplateExporter));

        public ICommand MenuFileExitCommand => _menuFileExitCommand ?? (_menuFileExitCommand = new RelayCommand(OnMenuFileExit));

        public static NavigationServiceEx NavigationService => ViewModelLocator.Current.NavigationService;

        public ShellViewModel()
        {
        }

        public void Initialize(Frame shellFrame, SplitView splitView, Frame rightFrame, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            NavigationService.Frame = shellFrame;
            MenuNavigationHelper.Initialize(splitView, rightFrame);
            _keyboardAccelerators = keyboardAccelerators;
        }

        private void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);
        }


        private void OnMenuViewsSchemaEditor() => MenuNavigationHelper.UpdateView(typeof(SchemaEditorViewModel).FullName);

        private void OnMenuViewsTreeViewEditor() => MenuNavigationHelper.UpdateView(typeof(TreeViewEditorViewModel).FullName);

        private void OnMenuViewsTemplateExporter() => MenuNavigationHelper.UpdateView(typeof(TemplateExporterViewModel).FullName);

        private void OnMenuFileExit()
        {
            Application.Current.Exit();
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }
    }
}
