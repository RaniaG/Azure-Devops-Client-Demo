using System;
using System.Threading.Tasks;
using TFSOnPremise;

namespace TFS_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TFSClient client = new TFSClient("https://projects.integrant.com/tfs/BICollection","Rania.Gamal", "mcv64f3ugna7elb5vrv64y7chpzquktaldhwtjmpmqy6hv7lmz6q");
            client.GetAllProjects();
            //client.GetProject("d9dca668-dd69-49d3-800c-4f41ab087c78").Wait();
            //client.GetProjectIterations("d9dca668-dd69-49d3-800c-4f41ab087c78");
            client.GetTeamMembers("85416787-c080-4410-be16-0c02d6ed3c9e", "d9dca668-dd69-49d3-800c-4f41ab087c78").Wait();
            //callGetProject(client).Wait();

        }
       
    }
}
