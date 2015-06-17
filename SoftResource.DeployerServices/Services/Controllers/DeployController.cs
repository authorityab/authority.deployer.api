﻿using System.Web.Http;
using DeployerServices.Services;

namespace DeployerServices.Controllers
{
    public class DeployController : ApiController
    {
        public string Post(string id)
        {
            var octopusService = new OctopusService();
            var taskId = octopusService.ReleaseTheCracken(id);

            return taskId;
        }
    }
}