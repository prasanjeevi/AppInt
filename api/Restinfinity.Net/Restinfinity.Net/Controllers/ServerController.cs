using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: "http://localhost:8000", headers: "*", methods: "*")]
    public class ServerController : ApiController
    {
        // GET: api/Server
        public IEnumerable<Models.File> Get(string id)
        {
            return processReq();
        }

        IEnumerable<Models.File> processReq()
        {
            List<Models.File> resp = new List<Models.File>();
            using (ServerManager iis = new ServerManager())
            {
                foreach (var site in iis.Sites)
                {
                    resp.Add(new Models.File
                    {
                        Id = site.Name,
                        Text = site.Name,
                        Icon = "fa fa-globe",
                        Parent = "#"
                    });
                    foreach (var app in site.Applications)
                    {
                        resp.Add(new Models.File
                        {
                            Id = app.VirtualDirectories[0].PhysicalPath,
                            Text = app.Path,
                            Icon = "fa fa-folder-o",
                            Parent = site.Name
                        });
                    }
                }

                //foreach (var site in iis.Sites)
                //{
                //    resp.Add(new Models.File
                //    {
                //        Id = site.Name,
                //        Text = site.Name,
                //        Icon = "fa fa-folder-o", //"jstree -custom-folder",
                //        State = new Models.State(), //Default false-false-false
                //        LiAttr = new Models.LiAttr()
                //        {
                //            Base = site.Name,
                //            IsLeaf = false
                //        },
                //        Children = true
                //    });
                //    foreach (var app in site.Applications)
                //    {
                //        resp.Add(new Models.File
                //        {
                //            Id = app.VirtualDirectories[0].PhysicalPath,
                //            Text = app.Path,
                //            Icon = "fa fa-folder-o", //"jstree -custom-folder",
                //            State = new Models.State(), //Default false-false-false
                //            LiAttr = new Models.LiAttr()
                //            {
                //                Base = site.Name,
                //                IsLeaf = true
                //            },
                //            Children = false
                //        });
                //    }
                //}
            }            
            return resp; //resp.OrderByDescending(i => i.Text);
        }

        // POST: api/Server
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Server/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Server/5
        public void Delete(int id)
        {
        }
    }
}
