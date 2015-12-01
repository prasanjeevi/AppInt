using Restinfinity.Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Xamasoft.JsonClassGenerator;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: "http://localhost:8000", headers: "*", methods: "*")]
    public class EntityController : ApiController
    {
        string projectRepository = @"E:\AppInt\project-repository\";

        // GET: api/Entity
        public IEnumerable<Entity> Get(string project)
        {
            string path = Path.Combine(projectRepository, project, "Models");
            List<Entity> entities = new List<Entity>();
            try
            {
                foreach (var file in Directory.GetFiles(path, "*.cs"))
                {
                    var entity = new Entity();
                    entity.Name = file.Replace(path, string.Empty).Replace("\\", string.Empty).Replace(".cs", string.Empty);
                    entity.Content = System.IO.File.ReadAllText(file);
                    entity.Project = project;
                    entities.Add(entity);
                }
            }
            catch(IOException)
            {

            }
            return entities;
        }

        // GET: api/Entity/5
        public Entity Get(string project, string id)
        {
            string path = Path.Combine(projectRepository, project, "Models", id + ".cs");
            var entity = new Entity();
            entity.Name = id;
            entity.Content = System.IO.File.ReadAllText(path);
            entity.Project = project;
            return entity;
        }

        // POST: api/Entity
        public void Post([FromBody]Entity value)
        {
            var classGenerator = new JsonClassGenerator();
            classGenerator.MainClass = value.Name;
            classGenerator.Example = value.Json;

            var project = string.IsNullOrEmpty(value.Project) ? "Global" : value.Project;

            classGenerator.Namespace = project + ".Models";
            classGenerator.TargetFolder = Path.Combine(projectRepository, project, "Models");
            Directory.CreateDirectory(classGenerator.TargetFolder);
            classGenerator.UseProperties = true;
            classGenerator.UsePascalCase = true;

            classGenerator.GenerateClasses();
        }

        // PUT: api/Entity/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Entity/5
        public void Delete(int id)
        {
        }
    }
}
