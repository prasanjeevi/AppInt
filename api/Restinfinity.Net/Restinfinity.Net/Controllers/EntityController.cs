using Restinfinity.Net.Models;
using Restinfinity.Net.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using Xamasoft.JsonClassGenerator;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: Config.Orgins, headers: "*", methods: "*")]
    public class EntityController : ApiController
    {
        // GET: api/Entity
        public IEnumerable<Entity> Get(string project)
        {
            string path = Path.Combine(Config.ProjectRepository, project, Config.Models);
            List<Entity> entities = new List<Entity>();
            try
            {
                foreach (var file in Directory.GetFiles(path, "*.cs"))
                {
                    var entity = new Entity();
                    entity.Name = file.Replace(path, string.Empty)
                        .Replace("\\", string.Empty)
                        .Replace(".cs", string.Empty);
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

        // GET: api/Entity/entityName
        public Entity Get(string project, string entityName)
        {
            string path = Path.Combine(Config.ProjectRepository, project, Config.Models, entityName + ".cs");
            var entity = new Entity();
            entity.Name = entityName;
            entity.Content = System.IO.File.ReadAllText(path);
            entity.Project = project;
            return entity;
        }

        // POST: api/Entity
        public IHttpActionResult Post([FromBody]Entity entity)
        {
            try
            {
                var classGenerator = new JsonClassGenerator();
                classGenerator.MainClass = entity.Name;
                classGenerator.Example = entity.Json;

                var project = string.IsNullOrEmpty(entity.Project) ? "Global" : entity.Project;

                classGenerator.Namespace = project + "." + Config.Models;
                classGenerator.TargetFolder = Path.Combine(Config.ProjectRepository, project, Config.Models);
                Directory.CreateDirectory(classGenerator.TargetFolder);
                classGenerator.UseProperties = true;
                classGenerator.UsePascalCase = true;
                if (entity.EnableDB)
                    classGenerator.GenerateIdField = true;

                var entities = classGenerator.GenerateClasses();

                if (entity.EnableDB)
                {
                    addDBContext(entity.Project, entities);
                }

                return Ok("Entity created successfully");
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        void addDBContext(string project, IEnumerable<string> entities)
        {            
            var path = Path.Combine(Config.ProjectRepository, project, Config.DAL);
            var file = Path.Combine(path, project + "Context.cs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                string content = createDBContext(project, entities);
                System.IO.File.WriteAllText(file,content);
            }
            else
            {
                var lines = System.IO.File.ReadAllLines(file).ToList();
                int dbSetEnd = lines.FindLastIndex(l => l.StartsWith("public DbSet<"));
                lines.Insert(dbSetEnd, addDBSet(entities));
                System.IO.File.WriteAllLines(file, lines);
            }
        }

        string createDBContext(string project, IEnumerable<string> models)
        {
            project = project.Capitalize();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using {0}." + Config.Models + ";");
            sb.AppendLine("using System.Data.Entity;");
            sb.AppendLine("using System.Data.Entity.ModelConfiguration.Conventions;");
            sb.AppendLine();
            sb.AppendLine("namespace {0}." + Config.DAL);
            sb.AppendLine("{");
            sb.AppendLine("public class {0}Context : DbContext");
            sb.AppendLine("{");
            sb.AppendLine("public {0}Context() : base(\"{0}Context\")");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("}");
            
            sb.AppendLine(addDBSet(models));

            sb.AppendLine("protected override void OnModelCreating(DbModelBuilder modelBuilder)");
            sb.AppendLine("{");
            sb.AppendLine("modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();");
            sb.AppendLine("}");

            sb.AppendLine("}"); //Class end
            sb.AppendLine("}"); //Namespace end

            sb.Replace("{0}", project);

            return sb.ToString();

        }

        string addDBSet(IEnumerable<string> models)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var model in models)
            {
                sb.AppendLine(string.Format("public DbSet<{0}> {1}",
                    model.Capitalize(), model.Pluralize().Capitalize()) + "{ get; set; }");
            }

            return sb.ToString();
        }
        
    }
}
