using System;
using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;

namespace DeployerServices.Services
{
    public class OctopusService
    {
        private const string Server = "http://octopus.softresourcehosting.com";
        private const string ApiKey = "API-PIIYT5NQEY4ZTKWEEHRQ93PEOQ";

        public List<ProjectResource> GetAllProjects()
        {

            try
            {
                var endpoint = new OctopusServerEndpoint(Server, ApiKey);
                var repository = new OctopusRepository(endpoint);

                return repository.Projects.FindAll();
            }
            catch (Exception e)
            {

            }

            return null;


        }


        public DashboardResource GetDashboardDynamic()
        {

            try
            {
                var endpoint = new OctopusServerEndpoint(Server, ApiKey);
                var repository = new OctopusRepository(endpoint);

                var dash =  repository.Dashboards.GetDynamicDashboard(new[] { "projects-1", "projects-33" }, new[] { "Environments-1" });

                return dash;
            }
            catch (Exception e)
            {

            }

            return null;


        }
    }
}