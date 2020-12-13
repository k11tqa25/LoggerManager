using System.Collections.Generic;
using System.Drawing;

namespace LoggerManagerExample
{
    public class CtfPointSetClass
    {
        public List<ctf_point> Point_List { get; set; }
    }

    public class ctf_point
    {
        public int ID { get; set; }
        public PointF Position { get; set; }
        public double CTF_Value { get; set; }
        public string Level { get; set; }
        public bool Pass { get; set; }
    }
}