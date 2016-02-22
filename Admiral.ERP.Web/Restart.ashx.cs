using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Admiral.ERP.Module.BusinessObjects.SYS;

namespace Admiral.ERP.Web
{
    /// <summary>
    /// Restart1 的摘要说明
    /// </summary>
    public class Restart1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            BusinessBuilder.Reset();
            HttpRuntime.UnloadAppDomain();
            //Thread.CurrentThread.Abort();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}