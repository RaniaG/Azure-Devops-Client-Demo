using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace TFSOnPremise.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectState State { get; set; }
        public Int64 NumberOfTeamsAssigned { get; set; }
        public string Collection { get; set; }
        public long Revision { get; set; }
    }
}
