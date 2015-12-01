using Restinfinity.Net.Models;
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
    public class ServiceController : ApiController
    {
        string projectRepository = @"E:\AppInt\project-repository\";

        // GET: api/Service
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Service/5
        public string Get(int id)
        {
            return "value";
        }

        //// POST: api/Service
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT: api/Service/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Service/5
        public void Delete(int id)
        {
        }

        public string Post([FromBody]Service service)
        {
            StringBuilder template = new StringBuilder();

            template.AppendLine("using System.Collections.Generic;");
            template.AppendLine("using System.Web.Http;");

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
                        template.AppendLine("public " + method.Response.Type + " "+ method.Name +"()" + buildParams(method.Request.UriParameters));
                        template.AppendLine("{");
                        //template.AppendLine("return new string[] { \"value1\", \"value2\" };");
                        template.AppendLine("}");
                        break;
                    case MethodType.POST:
                    case MethodType.PUT:
                        template.AppendLine("public " + method.Response.Type + " " + method.Name + "([FromBody]" + method.Request.UriParameters[0].Type + " " + method.Request.UriParameters[0].Name + ")");
                        template.AppendLine("{");
                        template.AppendLine("}");
                        break;
                }
            }
            template.AppendLine("}");//class end
            template.AppendLine("}");//namespace end

            return template.ToString();
        }

        [HttpPost]
        [Route("api/Service/Create")]
        public void Create([FromBody]Service service)
        {
            var project = string.IsNullOrEmpty(service.Project) ? "Global" : service.Project;
            var path = Path.Combine(projectRepository, project, "Controllers");
            Directory.CreateDirectory(path);
            System.IO.File.WriteAllText(Path.Combine(path, service.Name + "Controller.cs"), service.Content);

        }
        private string buildParams(IList<Parameter> uriParameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var param in uriParameters)
            {
                sb.Append(param.Type + " " + param.Name + ",");
            }
            return sb.ToString().TrimEnd(',');
        }
    }
}
