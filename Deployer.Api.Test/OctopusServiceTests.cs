using System;
using System.Diagnostics;
using Authority.Deployer.Api.Classes;
using Authority.Deployer.Api.Services;
using Authority.Deployer.Api.Services.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Authority.Deployer.Api.Test
{
    [TestClass]
    public class OctopusServiceTests
    {
        private IOctopusService _octopusService;

        [TestInitialize]
        public void Setup()
        {
            _octopusService = new OctopusService(new CacheManager());

        }

        [TestMethod]
        public void TimeGetReleasePage()
        {
            for (var i = 0; i < 10; i++)
            {
                var timer = new Stopwatch();
                timer.Start();
                
                var releasePage = _octopusService.GetReleases("projects-2");

                timer.Stop();
                Console.WriteLine("Finished " + i + ": " + timer.Elapsed);
            }
           
        }
    }
}
