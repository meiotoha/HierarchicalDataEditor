using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using HierarchicalDataEditor.Core.Models;

namespace HierarchicalDataEditor.Core.Services
{
    public class GlobalDataService
    {
        private static readonly Lazy<GlobalDataService> _instance = new Lazy<GlobalDataService>(() => new GlobalDataService());
        public static GlobalDataService Instance => _instance.Value;
        private GlobalDataService()
        {
            CurrentProject = new HDEProject();
        }

        public HDEProject CurrentProject { get; set; }
        public string CurrentProjectFile { get; set; }

    }
}
