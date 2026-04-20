using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportApp.Mobile.Models
{
    public class CategoryChartData
    {
        public string Title { get; set; } = string.Empty;
        public int Count { get; set; }

        public CategoryChartData(string title, int count)
        {
            Title = title;
            Count = count;
        }
    }
}
