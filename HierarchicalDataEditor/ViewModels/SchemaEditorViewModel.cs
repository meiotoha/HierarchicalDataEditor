using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HierarchicalDataEditor.Core.Models;
using Windows.UI.Xaml.Controls;
using HierarchicalDataEditor.Core.Services;

namespace HierarchicalDataEditor.ViewModels
{
    public class SchemaEditorViewModel : ViewModelBase
    {
        public ObservableCollection<DataSchemaItem> Source { get; } = new ObservableCollection<DataSchemaItem>();

        public SchemaEditorViewModel()
        {
        }

        private void Source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save();
        }

        public Task Load()
        {
            Source.CollectionChanged -= Source_CollectionChanged;
            foreach (var item in Source)
            {
                item.PropertyChanged -= S_PropertyChanged;
            }
            this.Source.Clear();
            if (GlobalDataService.Instance.CurrentProject.Schema.Properties != null)
            {
                foreach (var s in GlobalDataService.Instance.CurrentProject.Schema.Properties)
                {
                    s.PropertyChanged += S_PropertyChanged;
                    this.Source.Add(s);
                }
            }
            Source.CollectionChanged += Source_CollectionChanged;
            return Task.CompletedTask;
        }

        private void S_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Save();
        }

        private ICommand _saveCommand;
        private ICommand _addCommand;
        private ICommand _clearCommand;
        private ICommand _deleteCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new RelayCommand(Add));
        public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(Clear));
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand<DataSchemaItem>(Delete));
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private void Save()
        {
            GlobalDataService.Instance.CurrentProject.Schema.Properties = this.Source.ToList();
        }

        private void Clear()
        {
            Source.Clear();
        }
        private void Add()
        {
            Source.Add(new DataSchemaItem { Key = "New Property" });
        }
        private void Delete(DataSchemaItem i)
        {
            if (i != null)
            {
                Source.Remove(i);
            }
        }
    }
}
