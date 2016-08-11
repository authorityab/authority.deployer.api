﻿using System;

namespace Authority.Deployer.Api.Models
{
    public class Build
    {
        public string Id { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string StepName { get; set; }

        public string LastBuild { get; set; }

        public DateTime FinishDate { get; set; }

        public string Status { get; set; }

        public string LastModifiedBy { get; set; }
    }
}