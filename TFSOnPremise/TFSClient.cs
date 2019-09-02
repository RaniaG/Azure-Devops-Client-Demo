using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFSOnPremise.Entities;
using System.Net.Http;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;

namespace TFSOnPremise
{
    public class TFSClient
    {
        readonly string _uri;
        readonly string _username;
        readonly string _passwordOrPAT;
        private readonly VssConnection _connection;
        private readonly WorkItemTrackingHttpClient workItemClient;
        private readonly TeamHttpClient teamClient;
        private readonly ProjectHttpClient projectClient;
        private readonly HttpClient client = new HttpClient();
        private readonly ServiceHooksPublisherHttpClient serviceHooksClient;
        public TFSClient(string url,string username,string passwordOrPAT)
        {
            _uri = url;
            _username = username;
            _passwordOrPAT = passwordOrPAT;
            VssBasicCredential credentials = new VssBasicCredential(_username, _passwordOrPAT);
            Uri uri = new Uri(_uri);
            _connection = new VssConnection(uri, credentials);
            workItemClient = _connection.GetClient<WorkItemTrackingHttpClient>();
            teamClient = _connection.GetClient<TeamHttpClient>();
            projectClient = _connection.GetClient<ProjectHttpClient>();
            serviceHooksClient = _connection.GetClient<ServiceHooksPublisherHttpClient>();
        }
        
        public IEnumerable<TeamProjectReference> GetAllProjects()
        {
            
            IEnumerable<TeamProjectReference> projects = projectClient.GetProjects().Result;
            return projects;
        }
        public async Task<Project> GetProject(string projectId)
        {
            TeamProject t= await projectClient.GetProject(projectId);
            
            List<ProjectProperty> props= await projectClient.GetProjectPropertiesAsync(new Guid(projectId),keys:new List<string>{ "System.Microsoft.TeamFoundation.Team.Count"});

            return new Project
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                State = t.State,
                NumberOfTeamsAssigned = (Int64)props[0].Value,
                Revision=t.Revision
            };
           
        }
        public IEnumerable<Iteration> GetProjectIterations(string projectId)
        {
            //iterations at depth 1 bec no iteration can be parent of another
             WorkItemClassificationNode rootNode = workItemClient.GetClassificationNodeAsync(
                projectId,
                TreeStructureGroup.Iterations,depth:1).Result;
            List<Iteration> iterations=new List<Iteration>();
            foreach (var iteration in rootNode.Children)
            {
                Iteration i = new Iteration
                {
                    Id = iteration.Id,
                    Name = iteration.Name,
                    StartDate = iteration.Attributes["startDate"] != null ? Convert.ToDateTime(iteration.Attributes["startDate"]) : default(DateTime),
                    EndDate = iteration.Attributes["finishDate"] != null ? Convert.ToDateTime(iteration.Attributes["finishDate"]) : default(DateTime),
                    ProjectId = projectId
                };
                iterations.Add(i);
            }
            return iterations;
        }

        public async Task<IEnumerable<WebApiTeam>> GetTeams(string projectId)
        {
            return await teamClient.GetTeamsAsync(projectId);  
        }
        public async Task GetTeamMembers(string teamId , string projectId)
        {
            List<TeamMember> teamMembers = await teamClient.GetTeamMembersWithExtendedPropertiesAsync(projectId, teamId);
        }
        public void GetIterationsByTeam()
        {
         //teamClient.   
        }

        public void ConnectToServiceHooks(string projectId)
        {
   
            Subscription subscriptionParameters = new Subscription()
            {
                ConsumerId = "webHooks",
                ConsumerActionId = "httpRequest",
                ConsumerInputs = new Dictionary<string, string>
                {
                    { "url", "https://requestb.in/12h6lw21" }
                },
                EventType = "workitem.created",
                PublisherId = "tfs",
                PublisherInputs = new Dictionary<string, string>
                {
                    { "projectId", projectId }
                },
            };
        }
    }
}
