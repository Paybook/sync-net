using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaybookSDK
{
    public class Credentials_Structure
    {
        public string name { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public bool required { get; set; }
        public bool username { get; set; }
        public object validation { get; set; }
    }
}
