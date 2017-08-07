using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Xrm.Oss.CrmListener.Modules
{
    public class BaseModule : NancyModule
    {
        public BaseModule()
        {
            Get["/"] = _ => "Running";
        }
    }
}
