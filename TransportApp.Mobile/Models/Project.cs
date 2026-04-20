using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TransportApp.Mobile.Models
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        [JsonIgnore]
        public int CategoryID { get; set; }

        public Category? Category { get; set; }

        public List<ProjectTask> Tasks { get; set; } = [];

        public List<Tag> Tags { get; set; } = [];

        public override string ToString() => $"{Name}";
    }

    public class ProjectsJson
    {
        public List<Project> Projects { get; set; } = [];
    }
}