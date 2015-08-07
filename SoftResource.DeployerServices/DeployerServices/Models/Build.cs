using System;

namespace DeployerServices.Models
{
    public class Build
    {
        public string ProjectName { get; set; }

        public string StepName { get; set; }

        public string LastBuild { get; set; }

        public DateTime FinishDate { get; set; }

        public string Status { get; set; }

        public string LastModifiedBy { get; set; }
    }
}