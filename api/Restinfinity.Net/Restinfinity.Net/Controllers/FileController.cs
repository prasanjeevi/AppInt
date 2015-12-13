using Restinfinity.Net.Utility;
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
    [EnableCors(origins: Config.Orgins, headers: "*", methods: "*")]
    public class FileController : ApiController
    {
        // GET: api/File/5
        public IEnumerable<Models.File> Get(string id, string root = Config.AppDirectory)
        {
            if (id == "1") return buidFileTree(root);
            return buidFileTree(Path.Combine(root,id));
        }

        // GET: api/File/Lib/5
        [HttpGet]
        [Route("api/File/Lib")]
        public IEnumerable<Models.File> LibGet(string id)
        {           
            return Get(id, Config.LibDirectory);
        }
        
        // GET: api/File/Resource/5
        [HttpGet]
        [Route("api/File/Resource")]
        public HttpResponseMessage Resource(string resource)
        {
            try
            {
                string content = File.ReadAllText(resource);
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(content, System.Text.Encoding.UTF8, "text/html");
                resp.Headers.Location = new Uri(resource);
                return resp;
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                if (resource.EndsWith(Config.ProjectFile) || resource.EndsWith(Config.WebConfigFile) || resource.EndsWith(Config.BuildLog)) resp.ReasonPhrase = "Prebuild not completed";
                if (resource.EndsWith(Config.BuildLog)) resp.ReasonPhrase = "Build not completed";
                return resp;
            }
        }

        // POST: api/File/Save
        public IHttpActionResult Save(string file, [FromBody]string content)
        {
            try
            {
                System.IO.File.WriteAllText(file.Split('=')[1], content);
                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        IEnumerable<Models.File> buidFileTree(string path)
        {
            List<Models.File> resp = new List<Models.File>();
            string[] dirs = FileManager.GetDirectories(path);
            foreach (var dir in dirs)
            {
                resp.Add(new Models.File
                {
                    Id = dir,
                    Text = dir.Split('\\').Last(),
                    Icon = "fa fa-folder-o",
                    State = new Models.State(), //Default false-false-false
                    LiAttr = new Models.LiAttr()
                    {
                        Base = dir,
                        IsLeaf = false
                    },
                    Children = true
                });
            }

            string[] files = FileManager.GetFiles(path);
            foreach (var file in files)
            {
                resp.Add(new Models.File
                {
                    Id = file,
                    Text = file.Split('\\').Last(),
                    Icon = "fa fa-file-text-o",
                    State = new Models.State(),
                    LiAttr = new Models.LiAttr()
                    {
                        Base = file,
                        IsLeaf = true
                    },
                    Children = false
                });
            }
            return resp;
        }
    }
}
