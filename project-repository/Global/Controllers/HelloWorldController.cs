using System.Collections.Generic;
using System.Web.Http;
namespace Global.Controllers
{
public class HelloWorldController : ApiController
{
public string Get()
{
    return "hello";
}
public int Post([FromBody]string message)
{
    return 7;
}
}
}
