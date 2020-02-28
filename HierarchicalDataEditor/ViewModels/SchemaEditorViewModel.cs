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

        public Task Load()
        {
            this.Source.Clear();
            if (GlobalDataService.Instance.CurrentProject.Schema.Properties != null)
            {
                foreach (var s in GlobalDataService.Instance.CurrentProject.Schema.Properties)
                {
                    this.Source.Add(s);
                }
            }
            return Task.CompletedTask;
        }

        private static List<DataSchemaType> DataTypeCore = new List<DataSchemaType>
        {
             {DataSchemaType.String},
             {DataSchemaType.Boolean},
             {DataSchemaType.Float},
             {DataSchemaType.Integer},
        };

        public List<DataSchemaType> DataType => DataTypeCore;

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
            Source.Add(new DataSchemaItem { Key = "New Property", Type = DataSchemaType.String });
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
