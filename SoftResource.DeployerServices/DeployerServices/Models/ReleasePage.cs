﻿using System.Collections.Generic;

namespace DeployerServices.Models
{
    public class ReleasePage
    {
        public ReleasePage()
        {
            Releases = new List<Release>();
        }

        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public List<Release> Releases { get; set; }
        
    }
}