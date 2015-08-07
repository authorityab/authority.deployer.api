﻿using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class DeployController : ApiController
    {
        [HttpPost]
        public string Post([FromBody]dynamic data)
        {
            string projectId = data.projectId;
            string releaseId = data.releaseId;
            string environmentId = data.environmentId;


            var octopusService = new OctopusService();
            var taskId = octopusService.ReleaseTheCracken(projectId, releaseId, environmentId);

            return taskId;
        }

        [HttpGet]
        public string Status(string taskId)
        {
            var octopusService = new OctopusService();

            var task = octopusService.GetTaskProgress(taskId);

            return JsonConvert.SerializeObject(task);
        }

        //[HttpGet]
        //public string GetLatest(string projectId)
        //{
        //    var octopusService = new OctopusService();

        //    var deploys = octopusService.GetLatestDeploys(projectId);

        //    return JsonConvert.SerializeObject(deploys);
        //}

        //[HttpGet]
        //public string GetLatestDeployTasks(string projectId)
        //{
        //    var octopusService = new OctopusService();

        //    var tasks = octopusService.GetTasksFromLatestDeploys(projectId);

        //    return JsonConvert.SerializeObject(tasks);
        //}
    }
}