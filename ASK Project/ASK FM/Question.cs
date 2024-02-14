using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASK_FM
{
    internal class Question
    {
        public int IdQ { get; set; }
        public int FromUser { get; set; }
        public int ToUser { get; set; }
        public string Body { get; set; }
        public string? Answer { get; set; } = null;

    }
}
