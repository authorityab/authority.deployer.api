﻿using System;
using System.Diagnostics;
using Authority.Deployer.Api.Services.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Authority.Deployer.Api.Test
{
    [TestClass]
    public class Timing
    {
        private IOctopusService _octopusService;

        [TestInitialize]
        public void Setup(IOctopusService octopusService)
        {
            _octopusService = octopusService;

        }

        [TestMethod]
        public void TimeCache()
        {
            for (var i = 0; i < 10; i++)
            {
                var timer = new Stopwatch();
                timer.Start();
                
                var releasePage = _octopusService.GetReleases("projects-33");

                timer.Stop();
                Console.WriteLine("Finished " + i + ": " + timer.Elapsed);
            }
           
        }
    }
}