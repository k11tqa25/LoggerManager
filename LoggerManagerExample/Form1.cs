using LoggerManagerExample.Properties;
using LoggerManagerLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LoggerManagerExample
{
        public partial class LoggerManagerExample : Form
        {
                BindingSource bindingSource = new BindingSource();
                DataSet ds;
                string templateFilename = @"template.ods";

                public LoggerManagerExample()
                {
                        InitializeComponent();

                        //Setup LoggerManager Configuration
                        LoggerManagerConfiguration.OverrideDebugFile = true;  // Override the existing file.
                        LoggerManagerConfiguration.SaveDebugFile = false;     // Set to false if you don't want to save the dubug file of the library.            

                        UseBasicLogger();

                        UseResultLogger();

                        UseSettingsLogger();

                        UseMultiBasicLoggers();

                        UseMulitDefaultSettingsLoggers();

                        // Bind the data grid to a binding source
                        mDataGrid.DataSource = bindingSource;
                }

                private void UseBasicLogger()
                {
                        // Test for basic logger
                        BasicLogger.Construct()
                            .UseFileLogger(@".\test.txt")
                            .UseCustomFunctionAsyncLogger(msg => MessageBox.Show(msg))
                            .Build();

                        BasicLogger.ErrorOccurs += BasicLogger_ErrorOccurs;
                }

                private void BasicLogger_ErrorOccurs((object sender, Exception exception) obj)
                {
                        MessageBox.Show(obj.exception.Message);
                }

                private void UseResultLogger()
                {
                        // Test for result logger
                        ResultLogger<DistortionResultClass>.Construct(@".\result")
                            .UseTextFileLoggerFactory()
                            .UseJsonResultLogger()
                            .UseIniFileLoggerFactory()
                            .UseLibreOfficeResultLogger(templateFilename)
                            .Build();

                        ResultLogger<DistortionResultClass>.ErrorOccurs += ResultFactory_ErrorOccurs;

                        InitializeDistortionResultClass(ResultLogger<DistortionResultClass>.Instance);
                        //InitializeDefaultSettingsClass(ResultLogger<SettingsClass>.Instance);
                }

                private void UseSettingsLogger()
                {
                        // Test for settings logger
                        SettingsLogger<SettingsClass>.Construct(@".\Settings")
                            .UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON)
                            .Build();

                        SettingsLogger<SettingsClass>.Construct(@".\Settings")
                            .UseCustomSettingsLogger(new CustomSettingsLogger(@".\Settings.txt"))
                            .Build();

                        SettingsLogger<SettingsClass>.ErrorOccurs += DefaultSettingsLogger_ErrorOccurs;

                        InitializeDefaultSettingsClass(SettingsLogger<SettingsClass>.Instance);
                }

                private void UseMultiBasicLoggers()
                {
                        MultiBasicLoggers.Construct()
                            .AddFactory("Basic1", new BasicLoggerFactory()
                                .UseCustomFunctionAsyncLogger(t => Invoke(new Action(() => labelLog1.Text = t)))
                                .UseFileLogger(@".\Basic1.ini"))
                            .AddFactory("Basic2", new BasicLoggerFactory()
                                .UseCustomFunctionAsyncLogger(t => Invoke(new Action(() => labelLog2.Text = t)))
                                .UseFileLogger(@".\Basic2.ini"))
                            .Build();

                        MultiBasicLoggers.ErrorOccurs += BasicLogger_ErrorOccurs;
                }

                private void UseMultiResultLoggers()
                {
                        MultiResultLoggers.Construct()
                                .AddFactory(new ResultLoggerFactory<SettingsClass>(@".\MultiResult1")
                                        .UseLibreOfficeResultLogger(templateFilename)
                                        .UseJsonResultLogger())
                                .AddFactory(new ResultLoggerFactory<CtfPointSetClass>(@".\MultiResult2")
                                        .UseTextFileLoggerFactory()
                                        .UseIniFileLoggerFactory())
                                .Build();

                        MultiResultLoggers.AddFactory(new ResultLoggerFactory<SettingsClass>(@".\MultiResult1")
                                        .UseLibreOfficeResultLogger(templateFilename)
                                        .UseJsonResultLogger());

                        MultiResultLoggers.RemoveFactory(nameof(SettingsClass));

                        MultiResultLoggers.Save(nameof(SettingsClass));
                        MultiResultLoggers.SaveAll();

                }

                private void UseMulitDefaultSettingsLoggers()
                {
                        MultiSettingsLoggers.Construct()
                            .AddFactory(new SettingsLoggerFactory<CtfPointSetClass>(@".\settings1").UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON))
                            .AddFactory(new SettingsLoggerFactory<DistortionResultClass>(@".\settings2").UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON))
                            .AddFactory(new SettingsLoggerFactory<SettingsClass>(@".\settings3").UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON))
                            .AddFactory(new SettingsLoggerFactory<DistortionSettingsClass>(@".\settingsIni").UseBuiltInFormatSettingsLogger(SettingsFileFormat.INI))
                            .Build();
                        InitializeCtfDefaultSettings(MultiSettingsLoggers.Instance<CtfPointSetClass>());
                        InitializeDistortionResultClass(MultiSettingsLoggers.Instance<DistortionResultClass>());
                        InitializeDefaultSettingsClass(MultiSettingsLoggers.Instance<SettingsClass>());
                        //InitializeDistortionSettingsClass(MultiSettingsLoggers.Instance<DistortionSettingsClass>());
                }

                private void InitializeCtfDefaultSettings(CtfPointSetClass instance)
                {
                        instance.Point_List = new List<ctf_point>()
            {
                new ctf_point()
                {
                     CTF_Value = 1,
                     ID = 1,
                     Level = "A",
                     Pass = true,
                     Position = new PointF(0,0)
                },
                new ctf_point()
                {
                     CTF_Value = .5,
                     ID = 2,
                     Level = "C",
                     Pass = true,
                     Position = new PointF(1,0)
                },
                new ctf_point()
                {
                     CTF_Value = 1,
                     ID = 1,
                     Level = "A",
                     Pass = true,
                     Position = new PointF(0,0)
                },
                new ctf_point()
                {
                     CTF_Value = 1,
                     ID = 1,
                     Level = "A",
                     Pass = true,
                     Position = new PointF(0,0)
                }
            };
                }

                private void InitializeDefaultSettingsClass(SettingsClass settings)
                {
                        settings.Version = "0.0.0.0";
                        settings.Mode = "OP";
                        settings.ControlKey = new ControlKey()
                        {
                                Positive_X = "D",
                                Negative_X = "A",
                                Positive_Y = "W",
                                Negative_Y = "S"
                        };

                        settings.Uniformity = new Uniformity()
                        {
                                Thermal_Resistance = 0,
                                ImageCenter = new System.Drawing.PointF(200, 200),
                                UniformityMeasureSize = new Size(19, 6)
                        };

                        settings.Distortion = new Distortion()
                        {
                                Grid_Size_For_1920 = new System.Drawing.Size(38, 20),
                                Grid_Size_For_2712_44um = new System.Drawing.Size(24, 14),
                                Grid_Size_For_2712_92um = new System.Drawing.Size(40, 24),
                                Margin_For_1920 = new System.Drawing.Size(16, 17),
                                Margin_For_2712 = new System.Drawing.Size(10, 16),
                                ImageCenter_Dual = new System.Drawing.PointF(84.2f, 158.6f),
                                ImageCenter_Single = new System.Drawing.PointF(150.3f, 158.0f),
                                XY_table_angle = 89.9719, //第一台線性馬達xy table的夾角
                                LinMot_System_Transfer = new CoordinateTransfer()
                                {
                                        Trans_Projector_X = "y",
                                        Trans_Projector_Y = "-x",
                                        Trans_Motor_X = "-y",
                                        Trans_Motor_Y = "-x"
                                },
                                DMC_B140_M_Transfer = new CoordinateTransfer
                                {
                                        Trans_Projector_X = "y",
                                        Trans_Projector_Y = "-x",
                                        Trans_Motor_X = "-y",
                                        Trans_Motor_Y = "-x"
                                }
                        };

                        settings.Camera = new Camera()
                        {
                                Camera_Type = new List<Camera_Type>()
                {
                    new Camera_Type()
                   {
                       Name = "Flea3",
                       Pixel_Size_mm = 0.0097
                   },
                   new Camera_Type()
                   {
                       Name = "CMOS",
                       Pixel_Size_mm = 0.0022
                   }
                }
                        };

                        settings.PowerMeter = new PowerMeter()
                        {
                                ApplyFactor = true,
                                List = new List<MeterList>()
                        };

                        settings.Motor = new Motor()
                        {
                                ApplyMotorDefault = true,
                                ControllerType = "LinMot",
                                Axes = new List<Axis>()
                    {
                        new Axis()
                        {
                            Name = "X-AXIS",
                            ID = "0x08",
                            Comport = "Refresh",
                            PN_Factor = 1,
                            IsXaxis = true
                        },
                        new Axis()
                        {
                            Name = "Y-AXIS",
                            ID = "0x11",
                            Comport = "Refresh",
                            PN_Factor = 1,
                            IsXaxis = false
                        }
                    },
                                Positions = new List<Position>()
                        };

                        settings.Projector = new Projector()
                        {
                                ApplyProjectorDefault = true,
                                Resolution = "2712x1528",
                                Pixel_Size = "P_92",
                                Type = "_4k",
                                PJON_TEMP = 25.0,
                                Projector_List = new List<Individual>()
                {
                    new Individual()
                    {
                        Name = "Projector0",
                        ID = "0",
                    }
                }
                        };

                        settings.ExternalExe = new ExternalExe()
                        {
                                ExternalExeFilepaths = new List<ExternalExeFilepath>()
                {
                    new ExternalExeFilepath()
                    {
                        ApplicationName = "NQM_Controller.exe",
                        Filepath = ""
                    },
                    new ExternalExeFilepath()
                    {
                        ApplicationName = "NDH_ShopFloorCollector.exe",
                        Filepath = ""
                    },
                    new ExternalExeFilepath()
                    {
                        ApplicationName = "TestPatternUpload.exe",
                        Filepath = ""
                    }
                }
                        };

                }

                private void InitializeDistortionResultClass(DistortionResultClass instance)
                {
                        instance.Projector_ID = 1;
                        instance.Projector_Name = "Test";
                        instance.Calibrated_Points = new List<CalibratedPoint>()
                            {
                                new CalibratedPoint(){ Delta= new System.Drawing.PointF(2,2), ID = 1, Position = new System.Drawing.Point(1,1)},
                                new CalibratedPoint(){ Delta= new System.Drawing.PointF(2,2), ID = 2, Position = new System.Drawing.Point(3,3)},
                                new CalibratedPoint(){ Delta= new System.Drawing.PointF(1,2), ID = 3, Position = new System.Drawing.Point(3,1)},
                                new CalibratedPoint(){ Delta= new System.Drawing.PointF(3,5), ID = 4, Position = new System.Drawing.Point(2,0)},
                                new CalibratedPoint(){ Delta= new System.Drawing.PointF(0, 3), ID = 5, Position = new System.Drawing.Point(1,3)},
                                new CalibratedPoint(){ Delta= new System.Drawing.PointF(2,2), ID = 6, Position = new System.Drawing.Point(3,5)}
                            };
                }

                private void InitializeDistortionSettingsClass(DistortionSettingsClass instance)
                {
                        instance.Series_Name = "Name";
                        instance.IsSingleProjector = true;
                        instance.Single_Projector_Image_Size = new Size(100, 200);
                        instance.Projector_Pixel_Size_mm = 100;
                        instance.Calibration_Times = 1;
                        instance.Margin = new Size(10, 20);
                        instance.Criterion = 20;
                        instance.Grid_Size = new Size(16, 19);
                        instance.Pixel_Offset = 0;
                        instance.XY_Table_Angle = 0.01;
                        instance.Projector_Transferred_Coordination_X = "x";
                        instance.Projector_Transferred_Coordination_Y = "y";
                        instance.Motor_Transferred_Coordination_X = "-y";
                        instance.Motor_Transferred_Coordination_Y = "-x";
                        instance.ImageCenter_Single = new PointF(10, 20);
                        instance.ImageCenter_Dual = new PointF(20, 40);
                        instance.Motor_Grids = new Motor_Grid_Set()
                        {
                                A = 1,
                                B = "B",
                                C = 2.5
                        };
                }

                #region Helper Functions

                private void DisplayDataset(DataSet dataset)
                {
                        if (dataset == null)
                        {
                                MessageBox.Show("Table is null");
                                return;
                        }
                        ds = dataset;
                        bindingSource.DataSource = ds.Tables[0];

                        comboBoxTab.Items.Clear();
                        foreach (DataTable t in ds.Tables) comboBoxTab.Items.Add(t.TableName);
                        comboBoxTab.SelectedIndex = 0;
                }

                #endregion

                #region Buttons

                private void buttonLog_Click(object sender, EventArgs e)
                {
                        string msg = textBoxMsg.Text;
                        BasicLogger.Log(msg);
                        textBoxMsg.Text = string.Empty;

                        MultiBasicLoggers.Log("Basic1", msg);
                }

                private void buttonLog2_Click(object sender, EventArgs e)
                {
                        string msg = textBoxMsg.Text;

                        MultiBasicLoggers.Log("Basic2", msg);
                }

                private void buttonShowResultDataset_Click(object sender, EventArgs e)
                {
                        DisplayDataset(ResultLogger<SettingsClass>.Instance.ToDataset());

                }

                private void buttonSaveResultDataset_Click(object sender, EventArgs e)
                {
                        ResultLogger<DistortionResultClass>.Save();
                }

                private void buttonShowDefaultSettingsDataset_Click(object sender, EventArgs e)
                {
                        DisplayDataset(SettingsLogger<SettingsClass>.Instance.ToDataset());
                }

                private void buttonSaveDefaultSettingsDataset_Click(object sender, EventArgs e)
                {
                        SettingsLogger<SettingsClass>.Save();
                }

                private void buttonReadDefaultSettingsAndDisplay_Click(object sender, EventArgs e)
                {
                        // Read a existing default settings file
                        // To test this function, disable the InitializeDefaultSettingsClass function

                        if (SettingsLogger<SettingsClass>.Read())
                                // Display
                                DisplayDataset(SettingsLogger<SettingsClass>.Instance.ToDataset());

                }

                private void buttonSaveMultiDefulatSettings_Click(object sender, EventArgs e)
                {
                        MultiSettingsLoggers.SaveAll();
                }

                private void buttonReadMultiDefaultSettings_Click(object sender, EventArgs e)
                {
                        MultiSettingsLoggers.ReadAll();
                        var a = MultiSettingsLoggers.Instance<DistortionSettingsClass>();
                }

                private void comboBoxTab_SelectedIndexChanged(object sender, EventArgs e)
                {
                        bindingSource.DataSource = ds.Tables[(sender as ComboBox).SelectedIndex];
                }
                #endregion

                #region Events

                private void ResultFactory_ErrorOccurs((object sender, Exception exception) obj)
                {
                        MessageBox.Show($"Error sent from {obj.sender}\r\n:{obj.exception.Message}");
                }

                private void DefaultSettingsLogger_ErrorOccurs((object sender, Exception exception) obj)
                {
                        MessageBox.Show($"Error sent from {obj.sender}\r\n:{obj.exception.Message}");
                }

                #endregion

        }
}

