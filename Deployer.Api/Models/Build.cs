using System;
using log4net;

namespace Authority.Deployer.Api.Models
{
    public class Build
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Build));

        public string Id { get; set; }

        public string Number { get; set; }

        public string Agent { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string StepName { get; set; }

        public string LastBuild { get; set; }

        public DateTime FinishDate { get; set; }

        public string Status { get; set; }

        public string LastModifiedBy { get; set; }

        public string Comment { get; set; }

        public string WebUrl { get; set; }

        public string Href { get; set; }

        public string BuildConfigWebUrl { get; set; }

        public string BuildConfigId { get; set; }

        public string BuildTypeId { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as Build;

            return string.Equals(this.ProjectId, other.ProjectId)
                   && string.Equals(this.ProjectName, other.ProjectName)
                       && string.Equals(this.StepName, other.StepName)
                       && string.Equals(this.Status, other.Status);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode =  ProjectId?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (ProjectName?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (StepName?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Status?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}