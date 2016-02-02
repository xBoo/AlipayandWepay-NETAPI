using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AW.Webapi.Sample.Controllers
{
    public class AlipayController : ApiController
    {
        // GET: api/Alipay
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Alipay/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Alipay
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Alipay/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Alipay/5
        public void Delete(int id)
        {
        }
    }
}
