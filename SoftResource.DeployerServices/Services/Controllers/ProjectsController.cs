﻿using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class ProjectsController : ApiController
    {
        public string GetAllProjects()
        {
            var octopusService = new OctopusService();

            var projects = octopusService.GetAllProjects();

            return JsonConvert.SerializeObject(projects);
        }
    }
}
