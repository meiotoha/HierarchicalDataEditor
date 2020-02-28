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
                DisplayName = "NewNode",
                Code = "NewCode"
            });
        }

        private void Clear()
        {
            Nodes.Clear();
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
                DisplayName = "NewNode",
                Code = "NewCode"
            });
        }
        private void AddAfter(TreeNode obj)
        {
            if (obj.Parent == null)
            {
                var index = Nodes.IndexOf(obj);
                Nodes.Insert(index + 1, new TreeNode(obj.Parent)
                {
                    DisplayName = "NewNode",
                    Code = "NewCode"
                });
            }
            else
            {
                var index = obj.Parent.Items.IndexOf(obj);
                obj.Parent.Items.Insert(index + 1, new TreeNode
                {
                    DisplayName = "NewNode",
                    Code = "NewCode"
                });
            }

        }
        private void ClearChildren(TreeNode obj)
        {
            obj.Items.Clear();
        }
        private void Delete(TreeNode obj)
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
        private async void BatchAdd(TreeNode obj)
        {
            var tb = new TextBox();
            var dlg = new ContentDialog
            {
                Content = tb,
                PrimaryButtonText = "Save"
            };
            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var texts = tb.Text.TrimStart().TrimEnd().Replace("\r", "").Replace("\n", " ").Split(' ');
                foreach (var text in texts)
                {
                    obj.Items.Add(new TreeNode(obj)
                    {
                        DisplayName = text.TrimStart().TrimEnd(),
                        Code = "NewCode"
                    });
                }
            }
        }
    }
}
