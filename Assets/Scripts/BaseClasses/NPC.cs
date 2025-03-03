using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onnaMUD.BaseClasses
{
    public class NPC : Character
    {//this is obviously for NPC characters
        public bool IsUnique { get; set; }//if only 1 is allowed per server to be active


    }
}
