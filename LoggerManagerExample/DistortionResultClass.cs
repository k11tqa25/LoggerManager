using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerManagerExample
{
    public class DistortionResultClass
    {
        public int Projector_ID { get; set; }
        public string Projector_Name { get; set; }
        public List<CalibratedPoint> Calibrated_Points { get; set; }
        public List<CalibratedPoint> Failed { get; set; }
    }

    public class CalibratedPoint
    {
        public int ID { get; set; }
        public Point Position { get; set; }
        public PointF Delta { get; set; }
    }
}