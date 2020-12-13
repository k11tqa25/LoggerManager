using LoggerManagerLibrary;
using System.Collections.Generic;
using System.Drawing;

namespace LoggerManagerExample
{
    [DefineStyle("Header1", nameof(Color.Black), nameof(Color.Yellow))]
    [DefineStyle("Header2", nameof(Color.Black), nameof(Color.LightGreen))]
    [DefineStyle("CellStyle" ,nameof(Color.Red))]
    [DefineStyle("Failed", nameof(Color.Red))]
    [DefineStyle("Successful", nameof(Color.Green))]
    public class SettingsClass
    {
        [HeaderStyle("Header1")]
        public string Version { get; set; }

        [HeaderStyle("Header1")]
        public string Mode { get; set; }

        [HeaderStyle("Header1")]
        public ControlKey ControlKey { get; set; }

        [NewSheet]
        public Motor Motor { get; set; }

        [NewSheet]
        public Projector Projector { get; set; }

        [NewSheet]
        public Camera Camera { get; set; }

        [NewSheet]
        public PowerMeter PowerMeter { get; set; }

        [NewSheet]
        public Uniformity Uniformity { get; set; }

        [NewSheet]
        public Distortion Distortion { get; set; }

        [NewSheet]
        public ExternalExe ExternalExe { get; set; }
    }

    public class ControlKey
    {
        [HeaderStyle("Header2")]
        public string Positive_X { get; set; }
        [HeaderStyle("Header2")]
        public string Negative_X { get; set; }
        [HeaderStyle("Header2")]
        [ValueCellStyle("CellStyle")]
        public string Positive_Y { get; set; }
        [HeaderStyle("Header2")]
        [ValueCellStyle("CellStyle")]
        public string Negative_Y { get; set; }
    }

    public class Motor
    {
        [ConditionCellStyle(nameof(ApplyMotorDefault), "Successful", "Failed")]
        public bool ApplyMotorDefault { get; set; }

        [ValueCellStyle("CellStyle")]
        public string ControllerType { get; set; }
        public List<Axis> Axes { get; set; }
        public List<Position> Positions { get; set; }
    }

    [HeaderStyle("Header1")]
    public class Axis
    {
        [HeaderStyle("Header2")]
        [ConditionCellStyle(nameof(IsXaxis), "Successful", "Failed")]
        public string ID { get; set; }

        [HeaderStyle("Header2")]
        [ConditionCellStyle(nameof(IsXaxis), "Successful", "Failed")]
        public string Name { get; set; }

        [HeaderStyle("Header2")]
        [ConditionCellStyle(nameof(IsXaxis), "Successful", "Failed")]
        public string Comport { get; set; }

        [HeaderStyle("Header2")]
        [ConditionCellStyle(nameof(IsXaxis), "Successful", "Failed")]
        public int PN_Factor { get; set; }

        [Skip]
        public bool IsXaxis { get; set; } 
    }

    public class Position
    {
        public string Name { get; set; }
        public PointF Location { get; set; }
    }

    public class Projector
    {
        public bool ApplyProjectorDefault { get; set; }
        public double PJON_TEMP { get; set; }
        public List<Individual> Projector_List { get; set; }
        public string Resolution { get; set; }
        public string Pixel_Size { get; set; }
        public string Type { get; set; }
    }

    public class Individual
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string PatternFilename { get; set; }
        public string UniformityMaskFilename { get; set; }
        public string DistortionMaskFilename { get; set; }
    }

    public class Camera
    {
        [SkipHeader]
        public List<Camera_Type> Camera_Type { get; set; }
    }

    public class Camera_Type
    {
        [HeaderStyle("Header1")]
        public string Name { get; set; }
        [HeaderStyle("Header1")]
        public double Pixel_Size_mm { get; set; }
    }

    public class PowerMeter
    {
        public bool ApplyFactor { get; set; }
        public List<MeterList> List { get; set; }
    }

    public class MeterList
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public double Factor405 { get; set; }
        public double Factor385 { get; set; }
        public double Factor365 { get; set; }
    }

    public class Distortion
    {
        public double XY_table_angle { get; set; }
        public PointF ImageCenter_Single { get; set; }
        public PointF ImageCenter_Dual { get; set; }
        public Size Margin_For_1920 { get; set; }
        public Size Grid_Size_For_1920 { get; set; }
        public Size Margin_For_2712 { get; set; }
        public Size Grid_Size_For_2712_92um { get; set; }
        public Size Grid_Size_For_2712_44um { get; set; }
        public CoordinateTransfer LinMot_System_Transfer { get; set; }
        public CoordinateTransfer DMC_B140_M_Transfer { get; set; }
    }

    public class CoordinateTransfer
    {
        public string Trans_Projector_X { get; set; }
        public string Trans_Projector_Y { get; set; }
        public string Trans_Motor_X { get; set; }
        public string Trans_Motor_Y { get; set; }
    }

    public class Uniformity
    {
        public double Thermal_Resistance { get; set; }
        public PointF ImageCenter { get; set; }
        public Size UniformityMeasureSize { get; set; }
    }

    public class ExternalExe
    {
        public List<ExternalExeFilepath> ExternalExeFilepaths { get; set; }
    }

    public class ExternalExeFilepath
    {
        public string ApplicationName { get; set; }
        public string Filepath { get; set; }
    }
}