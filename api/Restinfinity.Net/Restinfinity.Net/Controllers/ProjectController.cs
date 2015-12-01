using Microsoft.Build.Execution;
using Restinfinity.Net.Utility;
using Restinfinity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: "http://localhost:8000", headers: "*", methods: "*")]
    public class ProjectController : ApiController
    {
        string projectRepository =  @"E:\AppInt\project-repository\";
        string packages = @"E:\AppInt\repository\packages\";
        string references = @"E:\AppInt\repository\WebApi\";
        string builds = @"E:\AppInt\builds\";
        string pubFile = @"E:\AppInt\repository\Local.pubxml";

        // GET: api/Project
        public IEnumerable<string> Get()
        {
            return Directory.GetDirectories(projectRepository).Select(p => p.Replace(projectRepository,"")) ;
        }

        // GET: api/Project/5
        public Dictionary<string,IEnumerable<Models.File>> Get(string id)
        {
            Dictionary<string, IEnumerable<Models.File>> data = new Dictionary<string, IEnumerable<Models.File>>(); 
            if (string.IsNullOrEmpty(id))
            {
                //HttpStatusCode.BadRequest;
                return data;
            }
            else
            {
                string pathModel = Path.Combine(projectRepository, id, "Models");
                string pathService = Path.Combine(projectRepository, id, "Controllers");
                string pathUtility = Path.Combine(projectRepository, id, "Utility");

                FileController fileCtrl = new FileController();
                var services = fileCtrl.Get(pathService);
                var models = fileCtrl.Get(pathModel);
                var utilities = fileCtrl.Get(pathUtility);

                data.Add("services", services);
                data.Add("entities", models);
                data.Add("utilities", utilities);

                return data;
            }
        }

        // POST: api/Project
        public IHttpActionResult Post([FromBody]string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return BadRequest("Project name is missing");
            }
            else
            {
                string path = Path.Combine(projectRepository, value);
                if (Directory.Exists(path))
                {
                    return BadRequest("Project already exists");
                }
                else
                {
                    var dir = Directory.CreateDirectory(path);
                    return Ok("Project created successfully");
                }
            }
        }

        [Route("api/Project/PreBuild")]
        public IHttpActionResult PreBuild([FromBody]string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                return BadRequest("Project name is missing");
            }
            else
            {
                //1. Create project solution folder [builds/project]
                var pathSolution = Path.Combine(builds, project);
                Directory.CreateDirectory(pathSolution);

                //2. Copy [packages] to solution folder
                FileManager.Copy(packages, Path.Combine(pathSolution, "packages"));

                //3. Copy [project] folder to project folder
                var pathProject = Path.Combine(pathSolution, project);
                FileManager.Copy(Path.Combine(projectRepository, project), pathProject);
                
                //4. Copy WebApi references to project folder             
                FileManager.Copy(references, pathProject);

                //5. Create Project Files [csproj]                
                createProjectFile(project, pathProject);

                //6. Copy publish profile to solution folder
                File.Copy(pubFile, Path.Combine(pathSolution, "Local.pubxml"));
                
                return Ok("Project created successfully");
            }
        }

        [Route("api/Project/Build")]
        public IHttpActionResult Build([FromBody]string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                return BadRequest("Project name is missing");
            }
            else
            {
                var result = build(project);
                return Ok(result);
            }
        }

        bool createProjectFile(string project, string pathProject)
        {
            string pathModel = Path.Combine(pathProject, "Models");
            string pathService = Path.Combine(pathProject, "Controllers");
            string pathUtility = Path.Combine(pathProject, "Utility");
            string pathProjectFile = Path.Combine(pathProject, "WebApi.csproj");

            var fileContent = File.ReadAllText(pathProjectFile);

            var projectId = "F66B60DB-DDE2-4300-BB14-6FF84094DC13"; //0
            var rootNS = "WebApi";
            var assembly = "WebApi";

            StringBuilder controllers = new StringBuilder();
            foreach (var controller in FileManager.GetFileNames(pathService))
            {
                controllers.AppendFormat("<Compile Include=\"Controllers\\{0}\" />", controller)
                    .AppendLine();
            }
           
            StringBuilder models = new StringBuilder();
            foreach (var model in FileManager.GetFileNames(pathModel))
            {
                models.AppendFormat("<Compile Include=\"Models\\{0}\" />", model)
                    .AppendLine();
            }

            StringBuilder utilities= new StringBuilder();
            foreach (var utility in FileManager.GetFileNames(pathUtility))
            {
                utilities.AppendFormat("<Compile Include=\"Models\\{0}\" />", utility)
                    .AppendLine();
            }

            fileContent = fileContent.Replace("{0}", projectId)
                .Replace("{1}", rootNS)
                .Replace("{2}", assembly)
                .Replace("{3}", controllers.ToString())
                .Replace("{4}", models.ToString())
                .Replace("{5}", utilities.ToString());

            File.WriteAllText(pathProjectFile, fileContent);

            //<Compile Include = "Controllers\DefaultController.cs" /> //3
            //<Compile Include="Models\Model.cs" /> //4
            //<Compile Include="Utility\Utility.cs" /> //5

            return true;
        }

        string build(string project)
        {
            // http://msdn.microsoft.com/en-gb/library/ms165431.aspx
            
            var props = new Dictionary<string, string>();
            props.Add("DeployOnBuild", "true");
            props.Add("PublishProfile", Path.Combine(builds, project, "Local.pubxml"));

            var request = new BuildRequestData(Path.Combine(builds, project, project ,"WebApi.csproj"), 
                props, null, new string[] { "Build" }, null);
            var parms = new BuildParameters();
            
            var result = BuildManager.DefaultBuildManager.Build(parms, request);

            return result.OverallResult.ToString();
            
        }

        // PUT: api/Project/5
        /// <summary>
        /// Rename the project
        /// </summary>
        /// <param name="id">current name</param>
        /// <param name="value">new name</param>
        public void Put(string id, [FromBody]string value)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(value))
            {
                //HttpStatusCode.BadRequest;
            }
            else
            {
                string newPath = Path.Combine(projectRepository, value);
                if (Directory.Exists(newPath))
                {
                    //HttpStatusCode.BadRequest;
                }
                else
                {
                    string currentPath = Path.Combine(projectRepository, id);
                    try
                    {
                        Directory.Move(currentPath, newPath);
                        //HttpStatusCode.Created;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        //HttpStatusCode.BadRequest;
                    }
                }
            }
        }

        // DELETE: api/Project/5
        public void Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                //HttpStatusCode.BadRequest;
            }
            else
            {
                string path = Path.Combine(projectRepository, id);
                Directory.Delete(path, true);
            }
        }
    }
}
