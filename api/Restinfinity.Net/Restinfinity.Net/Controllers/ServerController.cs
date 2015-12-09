using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    [EnableCors(origins: Config.Orgins, headers: "*", methods: "*")]
    public class ServerController : ApiController
    {
        // GET: api/Server
        public IEnumerable<Models.File> Get(string id)
        {
            return getSites();
        }

        IEnumerable<Models.File> getSites()
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
            }            
            return resp;
        }

        // POST: api/Server
        public IHttpActionResult Post([FromBody]string site)
        {
            var project = site.Split(':')[0];
            var port = int.Parse(site.Split(':')[1]);

            return createSite(project, port);
        }

        IHttpActionResult createSite(string project, int port)
        {
            try
            {
                var pathPub = Path.Combine(Config.Builds, project, Config.Pub);

                using (ServerManager iis = new ServerManager())
                {
                    //var pool = iis.ApplicationPools.Add(project);

                    var site = iis.Sites.Add(project, pathPub, port);
                    site.ApplicationDefaults.ApplicationPoolName = Config.AppPool;
                    iis.CommitChanges();

                    return Ok("Site deployed successfully");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

      
    }
}
