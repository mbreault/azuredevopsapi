using System;
using System.Linq;

namespace TeamFoundationServerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildWrapper buildWrapper = new BuildWrapper();
            var buildDefinitions = buildWrapper.ListBuildDefinitions();
            var firstBuild = buildDefinitions.ToList().Where(x => x.Name == "mbreault.typescript").FirstOrDefault();
            buildWrapper.QueueBuild(firstBuild);
        }
    }
}
