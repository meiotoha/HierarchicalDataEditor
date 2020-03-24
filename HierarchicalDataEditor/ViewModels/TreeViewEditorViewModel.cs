using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using HierarchicalDataEditor.Core.Models;
using Windows.UI.Xaml.Controls;
using HierarchicalDataEditor.Core.Services;
using HierarchicalDataEditor.Helpers;
using HierarchicalDataEditor.Views;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace HierarchicalDataEditor.ViewModels
{
    public class TreeViewEditorViewModel : ViewModelBase
    {
        private ICommand _saveCommand;
        private ICommand _itemInvokedCommand;
        private ICommand _addCommand;
        private ICommand _clearCommand;

        private ICommand _addChildCommand;
        private ICommand _baddChildCommand;
        private ICommand _addAfterCommand;
        private ICommand _clearChildrenCommand;
        private ICommand _deleteCommand;

        private ICommand _exportCommand;


        private object _selectedItem;
        public ObservableCollection<TreeNode> Nodes { get; set; }

        public object SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private void Save()
        {
            GlobalDataService.Instance.CurrentProject.Nodes = Nodes;
        }

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.TreeViewItemInvokedEventArgs>(OnItemInvoked));
        public ICommand AddCommand => _addCommand ?? (_addCommand = new RelayCommand(Add));
        public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(Clear));
        public ICommand AddChildCommand => _addChildCommand ?? (_addChildCommand = new RelayCommand<TreeNode>(AddChild));
        public ICommand BatchAddChildCommand => _baddChildCommand ?? (_baddChildCommand = new RelayCommand<TreeNode>(BatchAdd));

        public ICommand ExportChildrenCommand => _exportCommand ?? (_exportCommand = new RelayCommand<TreeNode>(ExportChildren));



        internal void FixNodes()
        {
            Fix(null, Nodes);
        }

        public ICommand AddAfterCommand => _addAfterCommand ?? (_addAfterCommand = new RelayCommand<TreeNode>(AddAfter));
        public ICommand ClearChildrenCommand => _clearChildrenCommand ?? (_clearChildrenCommand = new RelayCommand<TreeNode>(ClearChildren));
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand<TreeNode>(Delete));

        public TreeViewEditorViewModel()
        {
            Nodes = new ObservableCollection<TreeNode>();
        }

        private void Add()
        {
            Nodes.Add(new TreeNode
            {
                NodeName = "NewNode",
                NodeCode = "NewCode"
            });
        }

        private async void Clear()
        {
            if (await new ContentDialog
            { Content = "Clear All Nodes?", PrimaryButtonText = "Yes", SecondaryButtonText = "Cancel" }
                .ShowAsync() == ContentDialogResult.Primary)
            {
                Nodes.Clear();

            }
        }
        private void OnItemInvoked(WinUI.TreeViewItemInvokedEventArgs args)
            => SelectedItem = args.InvokedItem;

        public Task Load()
        {
            this.Nodes = GlobalDataService.Instance.CurrentProject.Nodes;
            FixNodes();
            return Task.CompletedTask;

        }
        private void Fix(TreeNode node, ObservableCollection<TreeNode> items)
        {
            foreach (var item in items)
            {
                item.Parent = node;
                Fix(item, item.Items);
            }
        }
        private void AddChild(TreeNode obj)
        {
            obj.Items.Add(new TreeNode(obj)
            {
                NodeName = "NewNode",
                NodeCode = "NewCode"
            });
        }
        private void AddAfter(TreeNode obj)
        {
            if (obj.Parent == null)
            {
                var index = Nodes.IndexOf(obj);
                Nodes.Insert(index + 1, new TreeNode(obj.Parent)
                {
                    NodeName = "NewNode",
                    NodeCode = "NewCode"
                });
            }
            else
            {
                var index = obj.Parent.Items.IndexOf(obj);
                obj.Parent.Items.Insert(index + 1, new TreeNode
                {
                    NodeName = "NewNode",
                    NodeCode = "NewCode"
                });
            }

        }
        private async void ClearChildren(TreeNode obj)
        {
            if (await new ContentDialog { Content = "Clear All Children?", PrimaryButtonText = "Yes", SecondaryButtonText = "Cancel" }.ShowAsync() == ContentDialogResult.Primary)
            {
                obj.Items.Clear();
            }
        }
        private async void Delete(TreeNode obj)
        {
            if (await new ContentDialog
            { Content = $"Remove This [{obj.NodeName}] Node?", PrimaryButtonText = "Yes", SecondaryButtonText = "Cancel" }
                .ShowAsync() == ContentDialogResult.Primary)
            {
                if (obj.Parent == null)
                {
                    Nodes.Remove(obj);
                }
                else
                {
                    obj.Parent.Items.Remove(obj);
                }
            }
        }
        private async void BatchAdd(TreeNode obj)
        {
            var tb = new TextBox()
            {
                AcceptsReturn = true
            };
            var dlg = new ContentDialog
            {
                Content = tb,
                PrimaryButtonText = "Save"
            };
            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var texts = tb.Text
                    .Replace('\r', '\n')
                    .Split("\n");

                var schema = GlobalDataService.Instance.CurrentProject.Schema.Properties ?? new List<DataSchemaItem>();
                //Import as csv
                foreach (var text in texts)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var arrs = text.Split(',');
                        var nNode = new TreeNode(obj)
                        {
                            NodeName = arrs[0].TrimStart().TrimEnd(),
                            NodeCode = "NewCode"
                        };
                        if (arrs.Length > 1)
                        {
                            nNode.NodeCode = arrs[1].TrimStart().TrimEnd();
                        }
                        if (arrs.Length > 2 && schema.Count > 0)
                        {
                            for (int i = 2; i < arrs.Length; i++)
                            {
                                if (schema.Count > i - 2)
                                {
                                    var val = arrs[i].TrimStart().TrimEnd();
                                    var sce = schema[i - 2];
                                    nNode.CoreData[sce.Key] = val;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        obj.Items.Add(nNode);
                    }
                }
            }
        }

        private async void ExportChildren(TreeNode obj)
        {
            await MenuNavigationHelper.OpenInDialog(typeof(TemplateExporterPage), obj);
        }
    }
}
