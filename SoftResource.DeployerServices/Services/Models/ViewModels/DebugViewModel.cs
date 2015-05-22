using System.Collections.Generic;
using DeployerServices.Classes;

namespace DeployerServices.Models.ViewModels
{
    public class DebugViewModel
    {
        public string ProjectId { get; set; } 

        public string ProjectName { get; set; }

        public BuildStatus BuildStatus { get; set; }

        public string LastChangedBy { get; set; }
    }
}