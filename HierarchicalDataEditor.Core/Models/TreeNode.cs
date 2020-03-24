using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace HierarchicalDataEditor.Core.Models
{
    public class TreeNode : Observable
    {
        private string _nodeCode;
        private string _nodeName;

        public TreeNode() : this(null)
        {

        }
        public TreeNode(TreeNode parent)
        {
            Parent = parent;
            Items = new ObservableCollection<TreeNode>();
            CoreData = new Hashtable();
        }

        [JsonIgnore]
        [XmlIgnore]
        public string ParentCode => Parent?.NodeCode;
        public string NodeCode { get => _nodeCode; set => SetAndNotify(ref _nodeCode, value); }
        public string NodeName { get => _nodeName; set => SetAndNotify(ref _nodeName, value); }
        public ObservableCollection<TreeNode> Items { get; }

        [JsonIgnore]
        [XmlIgnore]
        public TreeNode Parent { get; set; }
        public Hashtable CoreData { get; set; }


        public bool ShouldSerializeItems()
        {
            return Items?.Any() ?? false;
        }

        public bool ShouldSerializeCoreData()
        {
            return CoreData != null && CoreData.Keys.Count > 0;
        }
    }
}
