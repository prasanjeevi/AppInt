using Restinfinity.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
namespace Restinfinity.Net
{
    public partial class Index : System.Web.UI.Page
    {
        List<ApiDocumentation> apiDocs;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                LoadService();
            }
        }

        protected void LoadService()
        {
            string uri = "http://localhost:61081/api/Help/";
            string response = WebRequest(uri);
            if (!string.IsNullOrEmpty(response))
            {
                apiDocs = JsonConvert.DeserializeObject<List<ApiDocumentation>>(response);

                var services = from e in apiDocs
                               group e by e.Service
                               into g
                               select new
                               {
                                   Name = g.Key,
                                   ApiDoc = g
                               };

                ddlService.DataSource = services;
                ddlService.DataTextField = "Name";
                ddlService.DataValueField = "ApiDoc";
                ddlService.DataBind();

                LoadMethods();
            }
        }

        protected void LoadMethods()
        {
            string service = ddlService.SelectedItem.Text;

            var methods = apiDocs.Where(a => a.Service.Equals(service));

            ddlMethod.DataSource = methods;
            ddlMethod.DataTextField = "ApiId";
            ddlMethod.DataValueField = "ApiId";
            ddlMethod.DataBind();
                       
        }

        protected void GetMethodDetail()
        {
            string apiId = ddlMethod.SelectedItem.Value;
            string uri = "http://localhost:61081/api/Help/" + apiId;
            string response = WebRequest(uri);
            if (!string.IsNullOrEmpty(response))
            {
                divResult.InnerText = response;
                var service = JsonConvert.DeserializeObject<Service>(response);
            }
        }

        protected void ddlService_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMethods();
        }

        protected void ddlMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetMethodDetail();
        }

        public string WebRequest(string uri, string method="GET", string data = "", Dictionary<string,string> headers = null)
        {
            string response = string.Empty;
            var webRequest = System.Net.WebRequest.Create(uri) as System.Net.HttpWebRequest;
            try
            {
                if (webRequest != null)
                {
                    webRequest.Method = method;
                    webRequest.ServicePoint.Expect100Continue = false;
                    webRequest.Timeout = 2000000;
                    webRequest.ContentType = "application/json";
                    webRequest.Accept = "application/json";

                    if (headers != null)
                    {
                        foreach (var header in headers)
                            webRequest.Headers.Add(header.Key, header.Value);
                    }

                    if((method == "POST" || method == "PUT") && !string.IsNullOrEmpty(data))
                    {
                        using (var requestWriter = new System.IO.StreamWriter(webRequest.GetRequestStream()))
                        {
                            requestWriter.Write(data);
                        }
                    }

                    using (var webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse())
                    {
                        using (var responseStream = webResponse.GetResponseStream())
                        {
                            var reader = new System.IO.StreamReader(responseStream);
                            response = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch(System.Net.WebException ex)
            {

            }
            catch(Exception ex)
            {

            }
            return response;
         }
    }

    public class ApiService
    {
        public string Name { get; set; }
        public IEnumerable<ApiDocumentation> Docs { get; set; }
    }
}