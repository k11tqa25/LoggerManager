using LoggerManagerLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LoggerManagerExample
{
    public class DistortionSettingsClass
    {
        public string Series_Name { get; set; }
        public bool IsSingleProjector { get; set; }
        public Size Single_Projector_Image_Size { get; set; }
        public double Projector_Pixel_Size_mm { get; set; }
        public int Calibration_Times { get; set; }
        public Size Margin { get; set; }
        public double Criterion { get; set; }
        public Size Grid_Size { get; set; }
        public int Pixel_Offset { get; set; }
        public double XY_Table_Angle { get; set; }
        public string Projector_Transferred_Coordination_X { get; set; }
        public string Projector_Transferred_Coordination_Y { get; set; }
        public string Motor_Transferred_Coordination_X { get; set; }
        public string Motor_Transferred_Coordination_Y { get; set; }
        public PointF ImageCenter_Single { get; set; }
        public PointF ImageCenter_Dual { get; set; }

        [NewSheet]
        public Motor_Grid_Set Motor_Grids { get; set; }
    }

    public class Motor_Grid_Set
    {
        public int A { get; set; }
        public string B { get; set; }
        public double C { get; set; }
    }
}