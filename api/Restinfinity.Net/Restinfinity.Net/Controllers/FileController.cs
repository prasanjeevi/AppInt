using Restinfinity.Net.Utility;
using Restinfinity.Utility;
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
    public class FileController : ApiController
    {
        const string DirName =  @"E:\AppInt"; //"e:\\Code\\fileBrowserApp-master\\node_modules";

        // GET: api/File
        public IEnumerable<Models.File> Get()
        {
            return processReq(DirName);
        }

        // GET: api/File/5
        public IEnumerable<Models.File> Get(string id)
        {
            if (id == "1") return processReq(DirName);
            return processReq(Path.Combine(DirName, id));
        }


        [HttpGet]
        [Route("api/File/Resource")]
        public HttpResponseMessage Resource(string resource)
        {
            string content = File.ReadAllText(resource);
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(content, System.Text.Encoding.UTF8, "text/html");
            return resp;
        }

        IEnumerable<Models.File> processReq(string path)
        {
            List<Models.File> resp = new List<Models.File>();
            string[] dirs = FileManager.GetDirectories(path);
            foreach(var dir in dirs)
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
                    Icon = "fa fa-file-text-o", //"jstree -custom-file",
                    State = new Models.State(), //Default false-false-false
                    LiAttr = new Models.LiAttr()
                    {
                        Base = file,
                        IsLeaf = true
                    },
                    Children = false
                });
            }
            return resp; //resp.OrderByDescending(i => i.Text);
        }
}
}
