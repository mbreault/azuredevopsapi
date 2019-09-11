using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamFoundationServerClient
{
    public class BuildWrapper
    {
        BuildHttpClient _buildClient;
        VssConnection _connection;

        public BuildWrapper()
        {
            // Get a build client instance
            string collectionUri = Environment.GetEnvironmentVariable("VSS__URL");
            string username = Environment.GetEnvironmentVariable("VSS__USERNAME");
            string token = Environment.GetEnvironmentVariable("VSS__TOKEN");

            VssCredentials clientCredentials = new VssCredentials(new VssBasicCredential(username, token));
            _connection = new VssConnection(new Uri(collectionUri), clientCredentials);
            _buildClient = _connection.GetClient<BuildHttpClient>();
        }

        public Build QueueBuild(BuildDefinitionReference definition)
        {
            DefinitionReference definitionReference = new DefinitionReference();
            definitionReference.Id = definition.Id;
            definitionReference.Project = definition.Project;
            Build build = new Build();
            build.Definition = definitionReference;
            build.Project = definition.Project;
            var result = _buildClient.QueueBuildAsync(build).GetAwaiter().GetResult();
            return result;
        }

        public IEnumerable<BuildDefinitionReference> ListBuildDefinitions()
        {
            string projectName = Environment.GetEnvironmentVariable("VSS__PROJECTNAME");

            List<BuildDefinitionReference> buildDefinitions = new List<BuildDefinitionReference>();

            // Iterate (as needed) to get the full set of build definitions
            string continuationToken = null;
            do
            {
                IPagedList<BuildDefinitionReference> buildDefinitionsPage = _buildClient.GetDefinitionsAsync2(
                    project: projectName,
                    continuationToken: continuationToken).Result;

                buildDefinitions.AddRange(buildDefinitionsPage);

                continuationToken = buildDefinitionsPage.ContinuationToken;
            } while (!String.IsNullOrEmpty(continuationToken));

            // Show the build definitions
            foreach (BuildDefinitionReference definition in buildDefinitions)
            {
                Console.WriteLine("{0} {1}", definition.Id.ToString().PadLeft(6), definition.Name);
            }

            return buildDefinitions;
        }
    }
}