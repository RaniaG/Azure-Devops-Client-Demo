using System;
using System.Collections.Generic;
using System.Text;

namespace TFSOnPremise.Entities
{
    public class Iteration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectId { get; set; }
    }
}
