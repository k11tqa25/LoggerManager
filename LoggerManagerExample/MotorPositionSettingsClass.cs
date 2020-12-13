using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerManagerExample
{
    public class MotorPositionSettingsClass
    {
        public List<Package> List { get; set; }
    }

    public class Package
    {
        public string Name { get; set; }
        public PointF Position { get; set; }
    }
}