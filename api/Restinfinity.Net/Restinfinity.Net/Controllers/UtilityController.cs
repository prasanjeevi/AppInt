using Restinfinity.Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: "http://localhost:8000", headers: "*", methods: "*")]
    public class UtilityController : ApiController
    {
        string projectRepository = @"E:\AppInt\project-repository\";

        public object IO { get; private set; }

        // GET: api/Utility
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Utility/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Utility
        public void Post([FromBody]Entity utility)
        {
            var project = string.IsNullOrEmpty(utility.Project) ? "Global" : utility.Project;
            var path = Path.Combine(projectRepository, project, "Utility");

            try
            {
                Directory.CreateDirectory(path);
                System.IO.File.WriteAllText(Path.Combine(path, utility.Name + ".cs"), utility.Content);
            }
            catch
            {

            }
        }

        // PUT: api/Utility/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Utility/5
        public void Delete(int id)
        {
        }
    }
}
