# Class Definition

ResultLogger 及 SettingsLogger 這兩個在使用時須先定義一個類別，用以紀錄所有需要紀錄的內容。 此類別中僅會含有:

- **Public Classes**
- **Public Properties**
- **Other Self-Defined Classes**

底下會一步步帶您走過撰寫紀錄用類別的過程，並且解釋 **紀錄原則**。

若想直接看結果，可以滑到文件底端見  **SettingsClass.cs 完整程式碼**。

## 撰寫紀錄用類別步驟說明

### 1. 定義所有要紀錄的內容為 Public Properties:

	public class SettingsClass
    {
        public string Version { get; set; }

        public string Mode { get; set; }

        public ControlKey ControlKey { get; set; }

        public Motor Motor { get; set; }

        public Projector Projector { get; set; }

        public Camera Camera { get; set; }

        public PowerMeter PowerMeter { get; set; }

        public Uniformity Uniformity { get; set; }

        public Distortion Distortion { get; set; }

        public ExternalExe ExternalExe { get; set; }
    }

可以看出以上我們需要定義的東西有 **Version, Mode, ControlKey, Motor, Projector, Camea, PowerMeter, Uniformity, Distortion 及 ExternalExe** 這幾項。 前兩項 Version 及 Mode 都屬於 string type，其餘所有屬性皆為 **自定義類別**。

### 2. 自定義類別 (以Motor 為例)

	public class Motor
    {
        public bool ApplyMotorDefault { get; set; }

        public string ControllerType { get; set; }

        public List<Axis> Axes { get; set; }

        public List<Position> Positions { get; set; }
    }

    public class Axis
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Comport { get; set; }

        public int PN_Factor { get; set; }

        public bool IsXaxis { get; set; } 
    }

上方程式碼秀出了 Motor 當中的內容。另外可見在Motor 當中又使用了另外一個自定義的 Axis 類別型態，並且在Motor 下方定義出了 Axis 中的內容。


### 3. List<T> 型態

C# 中，List<T> 當中的 T 指的是泛型，可以帶入所有合理的資料型態。 當需要紀錄的東西**為一系列同一格式的內容**時，可以使用List<T> 紀錄。 詳見底下 **"輸出邏輯"** 查看 List<T> 如何輸出。


### 4. Attributes (屬性特徵 / 屬性標籤)

Attribute 在 C# 中的用法有點類似 Python 中的裝飾子 (decorator)，基本上就是在一個屬性上再掛上一個標籤。 LoggerManagerLibrary 中定義了幾種 Attribute，主要是為了輸出 LibreOffice 中的 Style，和簡易設定輸出內容。 以下介紹所有定義的 Attribute:

**註: 所有屬性特徵均不適用於 JSON 格式。**

> 內容相關:

1. **[NewSheet]:** 定義從 **此屬性開始將紀錄於 LibreOffice 新的頁簽當中**，並且新頁簽名稱即為該屬性名稱。 若為文字檔，則會在檔案後方加入 "-{屬性名稱}" 並將後續內容存於新檔中。 
2. **[Skip]:** **不記錄此屬性**。
3. **[SkipHeader]:** **不將此屬性名稱紀錄成Header (即略過 Header)**。由於List<T> 會將 List 的屬性名稱也紀錄於文件中，而這名稱並不常被需要，因此 [SkipHeader] 常裝飾於List<T>。  
4. **[HeaderName]:** **更改 Header 名字**。 此屬性標籤需帶有一個 string 引數。 在引數中輸入新的 Header 名稱。 例: [HeaderStyle("Header1")]

> LibreOffice 色彩相關:

5. **[DefineStyle]:** **定義 LibreOffice 輸出樣式。此屬性標籤必須定義於最外層的Class** (見完整程式碼)。此屬性特徵帶有三個參數: **樣式名稱、文字顏色、背景顏色**。唯樣式名稱為 required。 顏色名稱為所有在 .Net Color 中所有定義的名稱，因此建議使用 **nameof(Color.{SomeColor})** 這種方式給值。 

	例1: 定義文字顏色及背景顏色。 如： [DefineStyle("Header1", nameof(Color.Black), nameof(Color.Yellow))]

	例2: 僅定義文字顏色。 如： [DefineStyle("CellStyle", nameof(Color.Red))]

	例3: 僅定義背景顏色。 如： [DefineStyle("Successful", background: nameof(Color.Green))]

6. **[HeaderStyle]:** **套用指定樣式於此屬性 Header**。 帶有一個 Style 名稱參數。 例: [HeaderStyle("Header1")]
7. **[ConditionCellStyle]:** **將此屬性的 Value 根據某個條件來套用不同 Style。 此條件必須仰賴同一曾 Class 中的某個 Public Boolean Property**。 帶有三個必要參數: Boolean 屬性名稱、True 時的樣式、False 時的樣式。	例: [ConditionCellStyle(nameof(ApplyMotorDefault), "Successful", "Failed")]
8. **[ValueCellStyle]:** **套用指定樣式於此屬性的 Value。 此屬性優先於 ConditionCellStyle 的樣式**。 帶有一個 Style 名稱參數。 例: [ValueCellStyle("CellStyle")]

## 輸出邏輯

定義好的Class 中，**只要不是 List Type，一律是縱向紀錄，並且 Header 置左，Value 置右**。 若遇到自定義類別，或非System Type(int, string, bool...)，則會一路 **往右展開**。 

>	
	例1 - SettingsClass 中 SettingsClass 的輸出結果:
>	
		Version     0.0.0.0              
		Mode        OP                   
		ControlKey  Positive_X  D        
		            Negative_X  A        
		            Positive_Y  W        
		            Negative_Y  S    
>	
	例2 - SettingsClass 中 DistortionClass 部分輸出結果:
>	
		ImageCenter_Dual         IsEmpty            False    
		                         X                  84.2     
		                         Y                  158.6    
		Margin_For_1920          IsEmpty            False    
		                         Width              16       
		                         Height             17      

> 在 例 1 中，由於 Version 及 Mode 皆為 System Type，左欄為 Header, 右欄為 Value。 縱向紀錄結果。

> 在 例 1 中，由於 SettingsClass 中的 ControlKey 為 ControlKey 型態(見底下完整程式碼)，**為自定義類別**，因此**向右展開 自定義類別 中的內容，並且縱向紀錄**。 

> 在 例 2 中，由於 DistortionClass 中的 ImageCenter _ Dual 為 PointF 型態，而 Margin _ For _ 1920 為 Size 型態(見底下完整程式碼)，兩者皆 **非系統類別**，因此**向右展開 自定義類別 中的內容，並且縱向紀錄**。 


若為 **List 型態，則一律是橫向紀錄，並且Header 置上，Value 則一路往下紀錄**。若遇到自定義類別，或非System Type(int, string, bool...)，則會繼續**往右展開**。 

 
>	
	例1 [標準情況] - SettingsClass 中 Motor 中 Axes 的輸出結果:
>	
		Axes                                            
		ID                 Name     Comport  PN_Factor  
		0x08               X-AXIS   Refresh  1          
		0x11               Y-AXIS   Refresh  1       
>
	例2 [List 中包含自定義類別或非系統類別] - DistortionResultClass 中 Calibrated_Points 的輸出結果 
	(DistortionResultClass 完整定義可以在 LoggerManagerExample 中找到，此處拿來作範例使用):		
>
		Calibrated_Points                                                                           
		ID                 Position  IsEmpty  X        Y        Delta    IsEmpty  X        Y        
		1                            False    1        1                 False    2        2        
		2                            False    3        3                 False    2        2        
		3                            False    3        1                 False    1        2        
		4                            False    2        0                 False    3        5        
		5                            False    1        3                 False    0        3        
		6                            False    3        5                 False    2        2    


>  在例 1 中，Motor Class 中定義了一個 `public List<Axis> Axes { get; set; }`，紀錄結果會將 "Axes" 定為 Header 名稱(若不想紀錄可以使用 **SkipHeader** 省略掉)，並**往下一列開始橫向展開 Axis 中的紀錄內容，並將其結果一列列往下紀錄**。
>  
>  在例 2 中，DistortionResultClass 中 Calibrated_Points 為一自定義的CalibratedPoint類別，其定義如下:
>>
>> 
    public class CalibratedPoint
    {
        public int ID { get; set; }
        public Point Position { get; set; }
        public PointF Delta { get; set; }
    }
> 
> 當中的 Position 及 Delta 皆非系統類別，**因此會先空一列顯示名稱，再繼續向右展開其內容**。

## SettingsClass.cs 完整程式碼 (Style 僅作範例及測試使用)

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
 