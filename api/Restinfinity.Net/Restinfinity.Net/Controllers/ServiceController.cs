using Restinfinity.Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: Config.Orgins, headers: "*", methods: "*")]
    public class ServiceController : ApiController
    {   
        //POST: api/Service     
        public string Post([FromBody]Service service)
        {
            try
            {
                StringBuilder template = new StringBuilder();

                template.AppendLine("using System.Collections.Generic;");
                template.AppendLine("using System.Web.Http;");
                template.AppendLine("using System.Linq;");
                template.AppendLine("using System.Web;");
                template.AppendLine();
                template.AppendLine("using " + service.Project + ".Models;");
                template.AppendLine("using " + service.Project + ".Utility;");
                template.AppendLine("using " + service.Project + ".DAL;");
                template.AppendLine("namespace " + service.Project + ".Controllers");
                template.AppendLine("{");//namespace start
                template.AppendLine("public class " + service.Name + "Controller : ApiController");
                template.AppendLine("{");//class start

                foreach (var method in service.Methods)
                {
                    switch ((MethodType)Enum.Parse(typeof(MethodType), method.Name.ToUpper()))
                    {
                        case MethodType.GET:
                        case MethodType.DELETE:
                            template.AppendLine("public " + method.Response.Type + " " + method.Name + "(" + buildParams(method.Request.UriParameters) + ")");
                            template.AppendLine("{");
                            template.AppendLine(getReturnStatement(method.Response.Type));
                            template.AppendLine("}");
                            break;
                        case MethodType.POST:
                        case MethodType.PUT:
                            template.AppendLine("public " + method.Response.Type + " " + method.Name + "(" + buildParams(method.Request.UriParameters, method.Request.BodyParameters) + ")");
                            template.AppendLine("{");
                            template.AppendLine(getMethodDef(service.Project));
                            template.AppendLine(getReturnStatement(method.Response.Type));
                            template.AppendLine("}");
                            break;
                    }
                }
                template.AppendLine("}");//class end
                template.AppendLine("}");//namespace end

                return template.ToString();
            }
            catch(Exception ex)
            {
                return "Error: " + ex.Message; 
            }
        }

        private string getMethodDef(string project)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var db = new " + project + "Context();");
            
            sb.AppendLine("db.SaveChanges();");
            return sb.ToString();
        }

        //POST: api/Service/Create
        [HttpPost]
        [Route("api/Service/Create")]
        public IHttpActionResult Create([FromBody]Service service)
        {
            try
            {
                var project = string.IsNullOrEmpty(service.Project) ? "Global" : service.Project;
                var path = Path.Combine(Config.ProjectRepository, project, Config.Controllers);
                Directory.CreateDirectory(path);
                System.IO.File.WriteAllText(Path.Combine(path, service.Name + "Controller.cs"), service.Content);

                return Ok("Service created successfully");
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        string buildParams(IList<Parameter> uriParameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var param in uriParameters)
            {
                sb.Append(param.Type + " " + param.Name + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        string buildParams(IList<Parameter> uriParameters, IList<Parameter> bodyParameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var param in uriParameters)
            {
                sb.Append(param.Type + " " + param.Name + ",");
            }
            foreach (var param in bodyParameters)
            {
                sb.Append("[FromBody]" + param.Type + " " + param.Name + ",");
            }

            return sb.ToString().TrimEnd(',');
        }

        string getReturnStatement(string type)
        {
            string returnStatement = string.Empty;
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "string") returnStatement = "return string.Empty;";
                else if (type == "int") returnStatement = "return 0;";
                else if (type == "bool") returnStatement = "return false;";
                else if (type == "void") returnStatement = "";
                else returnStatement = "return new " + type + "();";
            }
            return returnStatement;
        }
    }
}
