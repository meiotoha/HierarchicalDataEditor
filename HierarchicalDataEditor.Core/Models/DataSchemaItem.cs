﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HierarchicalDataEditor.Core.Models
{
    public class DataSchemaItem : Observable
    {
        private string _key;
        public string Key { get => _key; set => SetAndNotify(ref _key, value); }

    }

    public class DataSchemaEditItem : DataSchemaItem
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetAndNotify(ref _value, value); }
        }

    }
}
