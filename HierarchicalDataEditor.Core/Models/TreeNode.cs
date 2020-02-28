using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace HierarchicalDataEditor.Core.Models
{
    public class TreeNode : Observable
    {
        private string _code;
        private string _displayName;

        public TreeNode() : this(null)
        {
         
        }
        public TreeNode(TreeNode parent)
        {
            Parent = parent;
            Items = new ObservableCollection<TreeNode>();
            CoreData = new Hashtable();
        }
        public string ParentCode => Parent?.Code;
        public string Code { get => _code; set => SetAndNotify(ref _code, value); }
        public string DisplayName { get => _displayName; set => SetAndNotify(ref _displayName, value); }
        public ObservableCollection<TreeNode> Items { get; }


        [JsonIgnore]
        [XmlIgnore]
        public TreeNode Parent { get; set; }

        public Hashtable CoreData { get; set; }

    }
}
