using Restinfinity.Net.Models;
using Restinfinity.Net.Utility;
using System;
using System.IO;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: Config.Orgins, headers: "*", methods: "*")]
    public class UtilityController : ApiController
    {
        // POST: api/Utility
        public IHttpActionResult Post([FromBody]Entity utility)
        {
            var project = string.IsNullOrEmpty(utility.Project) ? "Global" : utility.Project;
            var path = Path.Combine(Config.ProjectRepository, project, Config.Utility);

            try
            {
                Directory.CreateDirectory(path);
                System.IO.File.WriteAllText(Path.Combine(path, utility.Name + ".cs"), 
                    utility.Content.Replace("ProjectName.", utility.Project + "." )
                        .Replace("class ClassName",  "class " + utility.Name.Capitalize())
                    );
                return Ok("Utility created successfully");
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //GET: api/Utility
        public string Get()
        {
            return getTemplate();
        }

        string getTemplate()
        {
            StringBuilder template = new StringBuilder();
            template.AppendLine("using System;");
            template.AppendLine("using System.Collections.Generic;");
            template.AppendLine("using System.Linq;");
            template.AppendLine("using System.Web;");
            template.AppendLine();
            template.AppendLine("using ProjectName.Models;");
            template.AppendLine("using ProjectName.Utility;");
            template.AppendLine("using ProjectName.DAL;");
            template.AppendLine();
            template.AppendLine("namespace ProjectName." + Config.Utility);
            template.AppendLine("{");
            template.AppendLine("public class ClassName");
            template.AppendLine("{");
            template.AppendLine("public static string FunctionName()");
            template.AppendLine("{");
            template.AppendLine("return string.Empty;");
            template.AppendLine("}");
            template.AppendLine("}"); //Class end
            template.AppendLine("}"); //Namespace end
            return template.ToString();
        }
    }
}
