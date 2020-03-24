using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace HierarchicalDataEditor.Controls
{
    public sealed class SchemaDataEntry : Control
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key", typeof(string), typeof(SchemaDataEntry), new PropertyMetadata(default(string)));

        public string Key
        {
            get { return (string) GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(string), typeof(SchemaDataEntry), new PropertyMetadata(default(string)));

        public string Value
        {
            get { return (string) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public SchemaDataEntry()
        {
            this.DefaultStyleKey = typeof(SchemaDataEntry);
        }
    }
}
