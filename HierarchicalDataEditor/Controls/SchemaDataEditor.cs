using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using HierarchicalDataEditor.Core.Models;
using HierarchicalDataEditor.Core.Services;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace HierarchicalDataEditor.Controls
{
    public sealed class SchemaDataEditor : Control
    {
        public static readonly DependencyProperty SchemaItemsProperty = DependencyProperty.Register(
            "SchemaItems", typeof(IEnumerable<DataSchemaItem>), typeof(SchemaDataEditor), new PropertyMetadata(default(IEnumerable<DataSchemaItem>)));

        public IEnumerable<DataSchemaEditItem> SchemaItems
        {
            get { return (IEnumerable<DataSchemaEditItem>)GetValue(SchemaItemsProperty); }
            set { SetValue(SchemaItemsProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(Hashtable), typeof(SchemaDataEditor), new PropertyMetadata(default(Hashtable), (PropertyChangedCallback)));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var host = d as SchemaDataEditor;
            if (e.NewValue is Hashtable ht)
            {
                if (host.SchemaItems != null)
                {
                    foreach (var s in host.SchemaItems)
                    {
                        if (ht.ContainsKey(s.Key))
                        {
                            s.Value = (string)ht[s.Key];
                        }
                        else
                        {
                            s.Value = null;
                        }
                    }
                }
            }
            else
            {
                if (host.SchemaItems != null)
                {
                    foreach (var s in host.SchemaItems)
                    {
                        s.Value = null;
                    }
                }
            }
        }

        public Hashtable Value
        {
            get { return (Hashtable)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public SchemaDataEditor()
        {
            this.DefaultStyleKey = typeof(SchemaDataEditor);
            this.Loaded += SchemaDataEditor_Loaded;
            this.Unloaded += SchemaDataEditor_Unloaded;

        }

        private void SchemaDataEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            if (SchemaItems != null)
            {
                foreach (var dataSchemaEditItem in SchemaItems)
                {
                    dataSchemaEditItem.PropertyChanged -= DataSchemaEditItem_PropertyChanged;
                }
            }
            SchemaItems = null;
        }

        private void SchemaDataEditor_Loaded(object sender, RoutedEventArgs e)
        {
            this.SchemaItems = GlobalDataService.Instance.CurrentProject.Schema?.Properties?
                .Select(x => new DataSchemaEditItem
                {
                    Key = x.Key,
                    Value = null
                }).ToList();
            if (SchemaItems != null)
            {
                foreach (var dataSchemaEditItem in SchemaItems)
                {
                    dataSchemaEditItem.PropertyChanged += DataSchemaEditItem_PropertyChanged;
                }
            }

        }

        private void DataSchemaEditItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataSchemaEditItem.Value))
            {
                if (sender is DataSchemaEditItem item)
                {
                    Value[item.Key] = item.Value;
                }
            }
        }
    }
}
