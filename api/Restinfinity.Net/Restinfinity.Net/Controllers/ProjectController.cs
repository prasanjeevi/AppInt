using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Restinfinity.Net.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: Config.Orgins, headers: "*", methods: "*")]
    public class ProjectController : ApiController
    {   
        // GET: api/Project
        public IEnumerable<string> Get()
        {
            return Directory.GetDirectories(Config.ProjectRepository)
                .Select(p => p.Replace(Config.ProjectRepository, string.Empty));
        }

        // GET: api/Project/projectName
        public Dictionary<string,IEnumerable<Models.File>> Get(string id)
        {
            string project = id;
            Dictionary<string, IEnumerable<Models.File>> data = new Dictionary<string, IEnumerable<Models.File>>();
            if (!string.IsNullOrEmpty(project))
            {
                string pathModel = Path.Combine(Config.ProjectRepository, project, Config.Models);
                string pathService = Path.Combine(Config.ProjectRepository, project, Config.Controllers);
                string pathUtility = Path.Combine(Config.ProjectRepository, project, Config.Utility);
                string pathDAL = Path.Combine(Config.ProjectRepository, project, Config.DAL);
                
                FileController fileCtrl = new FileController();
                var services = fileCtrl.Get(pathService);
                var models = fileCtrl.Get(pathModel);
                var utilities = fileCtrl.Get(pathUtility);
                var dal = fileCtrl.Get(pathDAL);

                var configs = new List<Models.File>() {
                    new Models.File() { Text= Config.ProjectFile, Id= Path.Combine(Config.Builds, project, project, Config.ProjectFile) },
                    new Models.File() { Text = Config.WebConfigFile, Id = Path.Combine(Config.Builds, project, project, Config.WebConfigFile) },
                    new Models.File() { Text = Config.PubFileName, Id = Path.Combine(Config.Builds, project, Config.PubFileName) },
                    new Models.File() {Text=Config.BuildLog, Id = Path.Combine(Config.Builds, project, Config.BuildLog) }
                };

                data.Add("services", services);
                data.Add("entities", models);
                data.Add("utilities", utilities);
                data.Add("dal", dal);
                data.Add("configs", configs);
            }
            return data;
        }

        // POST: api/Project
        public IHttpActionResult Post([FromBody]string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                return BadRequest("Project name is missing");
            }
            else
            {
                string path = Path.Combine(Config.ProjectRepository, project);
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

        // POST: api/Project/PreBuild 
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
                var pathSolution = Path.Combine(Config.Builds, project);
                Directory.CreateDirectory(pathSolution);

                //2. Copy [packages] to solution folder
                FileManager.Copy(Config.Packages, Path.Combine(pathSolution, "packages"));

                //3. Copy [project] folder to project folder
                var pathProject = Path.Combine(pathSolution, project);
                FileManager.Copy(Path.Combine(Config.ProjectRepository, project), pathProject);
                
                //4. Copy WebApi references to project folder             
                FileManager.Copy(Config.References, pathProject);

                //5. Create Project Files [csproj]                
                createProjectFile(project, pathProject);

                //6. Copy publish profile to solution folder
                File.Copy(Config.PubFile, Path.Combine(pathSolution, Config.PubFileName), true);
                
                return Ok("Project pre build success");
            }
        }

        // POST: api/Project/Build 
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
                if (result.ToLower() == "success")
                    return Ok("Build succeeded");
                else
                    return Ok("Build failure");
            }
        }

        void createProjectFile(string project, string pathProject)
        {
            string pathModel = Path.Combine(pathProject, Config.Models);
            string pathService = Path.Combine(pathProject, Config.Controllers);
            string pathUtility = Path.Combine(pathProject, Config.Utility);
            string pathDAL = Path.Combine(pathProject, Config.DAL);
            string pathProjectFile = Path.Combine(pathProject, Config.ProjectFile);

            var fileContent = File.ReadAllText(pathProjectFile);
            
            StringBuilder controllers = new StringBuilder();
            foreach (var controller in FileManager.GetFileNames(pathService))
            {
                controllers.AppendFormat("<Compile Include=\"{0}\\{1}\" />", Config.Controllers, controller)
                    .AppendLine();
            }
           
            StringBuilder models = new StringBuilder();
            foreach (var model in FileManager.GetFileNames(pathModel))
            {
                models.AppendFormat("<Compile Include=\"{0}\\{1}\" />", Config.Models, model)
                    .AppendLine();
            }

            StringBuilder utilities= new StringBuilder();
            foreach (var utility in FileManager.GetFileNames(pathUtility))
            {
                utilities.AppendFormat("<Compile Include=\"{0}\\{1}\" />", Config.Utility, utility)
                    .AppendLine();
            }

            StringBuilder dals = new StringBuilder();
            foreach (var dal in FileManager.GetFileNames(pathDAL))
            {
                dals.AppendFormat("<Compile Include=\"{0}\\{1}\" />", Config.DAL, dal)
                    .AppendLine();
            }

            fileContent = fileContent.Replace("{0}", Config.ProjectId)
                .Replace("{1}", Config.RootNS)
                .Replace("{2}", Config.Assembly)
                .Replace("{3}", controllers.ToString())
                .Replace("{4}", models.ToString())
                .Replace("{5}", utilities.ToString())
                .Replace("{6}", dals.ToString());

            File.WriteAllText(pathProjectFile, fileContent);
            
        }
        
        string build(string project)
        {
            var pathPubFile = Path.Combine(Config.Builds, project, Config.PubFile);
            var fileContent = File.ReadAllText(pathPubFile);
            fileContent = fileContent.Replace("{0}", Path.Combine(Config.Builds, project, Config.Pub));
            File.WriteAllText(pathPubFile, fileContent);

            var props = new Dictionary<string, string>();
            props.Add("DeployOnBuild", "true");
            props.Add("PublishProfile", pathPubFile);

            var request = new BuildRequestData(Path.Combine(Config.Builds, project, project, Config.ProjectFile), 
                props, null, new string[] { "Build" }, null);
            var parms = new BuildParameters();

            var pathLogFile = Path.Combine(Config.Builds, project, Config.BuildLog);

            FileLogger logger = new FileLogger() { Parameters = "logfile=" + pathLogFile };
            parms.Loggers = new List<ILogger> { logger }.AsEnumerable();

            var result = BuildManager.DefaultBuildManager.Build(parms, request);

            return result.OverallResult.ToString();
            
        }

        // PUT: api/Project/5
        /// <summary>
        /// Rename the project
        /// </summary>
        /// <param name="id">current name</param>
        /// <param name="value">new name</param>
        public IHttpActionResult Put(string id, [FromBody]string value)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(value))
            {
                return BadRequest("Insufficient inputs");
            }
            else
            {
                string newPath = Path.Combine(Config.ProjectRepository, value);
                if (Directory.Exists(newPath))
                {
                    return BadRequest("Project already exists");
                }
                else
                {
                    string currentPath = Path.Combine(Config.ProjectRepository, id);
                    try
                    {
                        Directory.Move(currentPath, newPath);
                        return Ok("Project renamed successfully");
                    }
                    catch (DirectoryNotFoundException)
                    {
                        return BadRequest("Project not exists");
                    }
                }
            }
        }

        // DELETE: api/Project/projectName
        public IHttpActionResult Delete(string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                return BadRequest("Project name is missing");
            }
            else
            {
                string path = Path.Combine(Config.ProjectRepository, project);
                Directory.Delete(path, true);
                return Ok("Project deleted successfully");
            }
        }
    }
}
