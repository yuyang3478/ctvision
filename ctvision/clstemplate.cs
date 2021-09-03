using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace leanvision
{
    [Serializable]
    class clstemplate
    {
        public string  name { get; set; }//检测名称

        public clstemplate() {
            name = "";
        }
    }
}
