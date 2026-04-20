using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TransportApp.Mobile.Models
{
    public class ProjectTask
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        [JsonIgnore]
        public int ProjectID { get; set; }
    }
}