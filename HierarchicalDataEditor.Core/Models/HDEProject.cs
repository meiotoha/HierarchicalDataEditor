using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HierarchicalDataEditor.Core.Models
{
    public class HDEProject
    {
        public HDEProject()
        {
            Schema = new DataSchema();
            Nodes = new ObservableCollection<TreeNode>();
        }
        public DataSchema Schema { get; set; }
        public ObservableCollection<TreeNode> Nodes { get; set; }
    }
}
