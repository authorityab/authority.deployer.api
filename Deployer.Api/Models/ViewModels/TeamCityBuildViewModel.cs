﻿using System;
using Authority.Deployer.Api.Classes;
using BuildStatus = TeamCitySharp.Locators.BuildStatus;

namespace Authority.Deployer.Api.Models.ViewModels
{
    public class TeamCityBuildViewModel
    {
        public string Version { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public BuildStatus BuildStatus { get; set; }

        public string LastChangedBy { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }
    }
}