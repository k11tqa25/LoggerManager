# Logger Manager Library
8/3/2020 11:51:12 AM 

Logger Manager 適用於使用 .Net 環境開發的所有應用程式。目的是為了簡化紀錄/讀取所有型態的 Log 檔，並且統一所有 C# 程式在讀寫上的寫法，方便日後維護。

## Logger 型態介紹

此 Logger Manager 中包含了以下三種型態的 Logger:

  - *Basic Logger: 適合用於輸出系統紀錄或儲錯紀錄文件。*
  - *Result Logger: 適合用於儲存某實驗或流程的結果。(無讀取功能)*
  - *Settings Logger: 適合用於讀寫設定檔。(具讀寫功能)*
  
以上三種在同一程式當中，均是 "一式多份" 的型態。 也就是說，當決定好需要紀錄的項目時，以上三種Logger 都支援以多種不同檔案格式的方式輸出。但是，一旦在同一支程式當中，有多種不同的項目或流程需要紀錄時，則需使用以下三種 Logger:

  - *Multi-Basic Loggers: 適合用於建立及輸出多種不同的系統紀錄或儲錯紀錄文件。*
  - *Multi-Result Loggers: 適合用於建立及儲存多種實驗或流程的結果。(無讀取功能)*
  - *Multi-Settings Loggers: 適合用於讀寫多種設定檔。(具讀寫功能)*
  
  

## 請注意 !
1.  以上六種 Logger 均為 Static 型態。也就是物件將在編譯後就被產生，使用範圍是全域專案,也因此需要注意初始化步驟 (如 2.)。
2.  所有 Logger 在使用之前，一定要先藉由 Construct 初始化，最後 Build 完成建造。 **建議將此初始化流程寫在程式最前面**。
3.  Basic Logger 紀錄使用到多執行續。若在客製化紀錄當中有使用到更新 UI 的狀況，記得要將執行續跳回 UI上。


# 使用說明
## DLL 參考
使用時只需參考 **LoggerManagerLibrary.dll** 即可。

但是同樣的dll 路徑底下，
需含有底下四個相依的 dll:

 -	**Newtonsoft.Json.dll**
 -	**Newtonsoft.Json.Schema.dll**
 -	**DotNetZip.dll**
 -	**Ninject.dll**
 
此四者可以在 LoggerManagerExample 的輸出路徑底下找到。

另外，由於LibreOffice 輸出需要一 **template.ods** 檔案，需要一併複製到輸出路徑底下，使用時會需要指定template路徑。


## 基本使用範例

### 1. BasicLogger

BasicLogger 主要是設計用來除錯。 只要在一開始初始化完成後，可以在專案中任意位置呼叫紀錄。

> #### 1.1 初始化:

>>    
    BasicLogger.Construct()											// 先Construct
		.UseConsoleLogger()											// 使用 ConsoleLogger (記錄在Output當中)
        .UseFileLogger(@".\test.txt")								// 使用 FileLogger (紀錄於指定檔案)
        .UseCustomFunctionAsyncLogger(msg => MessageBox.Show(msg))	// 使用 CustomFunctionLogger (自訂記錄方式。可搭配更新UI之類的功能。)
        .Build();													// 建構此BasicLogger
>> 目前支援的所有Basic Loggers:
>> 
>> 1. **ConsoleLogger** : 紀錄於Console。在Console 專案中可以使用這個直接在cmd 中輸出字樣; 在 Release mode 中，若使用 **偵錯模式 (F5)** 則會顯示於 **"輸出"** 中。
>> 2. **FileLogger** : **非同步** 紀錄於指定檔案當中。在使用 UseFileLogger 時需 **指定檔案路徑及名稱。**
>> 3. **CustomFunctionAsyncLogger** : **非同步** 自定義紀錄功能。提供一個紀錄的 **方法**, 此方法需要有一個 **string** 當引數。**注意:** 由於此方法是非同步呼叫，在自訂義的方法中**若有 UI 相關功能，請記得跳回 UI 執行緒上**。 

> #### 1.2 紀錄方式:

>>    
	string msg = "Some message to log.";	
	BasicLogger.Log(msg);											// 專案中任意地方呼叫，紀錄於所有初始化中使用的Logger

> #### 1.3 事件註冊:

>>    
	private void RegisterEvents()
	{
		// 註冊 ErrorOccurs 事件
		BasicLogger.ErrorOccurs += BasicLogger_ErrorOccurs;
	} 	

>>    
	private void BasicLogger_ErrorOccurs((object sender, Exception exception) obj)
    {
		// 事件處理
        MessageBox.Show($"Error sent from {obj.sender}\r\n{obj.exception.Message}");
    }
>> **ErrorOccurs 事件**: 當任何一個Basic Logger 發生錯誤時被觸發。 

### 2. Result Logger

Result Logger 是專門用來紀錄某次實驗或檢測後的結果。 由於寫入格式種類繁多，因此只有寫入功能，不具備讀取功能。

**強烈建議一個專案只使用一種 ResultLogger<TClass>**，也就是只有一種輸出類別(TClass)。 由於一旦 Construct 後，ResultLogger 內部的所有內容會被全部刷新並初始化。 為避免紀錄結果在被Save 前被意外洗掉，使用 Construct 的時機須要多留意。 (可以多次重新 Construct，但一旦Construct 就視為一個新的物件。)

若有需要使用多種 TClass，請見 MultiResultLoggers。

> #### 2.1 準備 Result Class: 
>> 關於Class 定義詳見 [ClassDefinition.html][ClassDef]。 底下將都以 "SettingsClass" 作為範例。

 
> #### 2.2 初始化:


	ResultLogger<SettingsClass>										// 泛型中給定Result Class, 此處以 "SettingsClass" 作為範例 (見 ClassDefinition.html)
				.Construct(@".\result")								// Construct 時給定檔名，但不指定副檔名。
                .UseTextFileLoggerFactory()							// 使用 txt 格式紀錄 (可指定副檔名，預設為txt)
                .UseJsonResultLogger()								// 使用 Json 格式紀錄
                .UseIniFileLoggerFactory()							// 使用 kernel32.dll 格式紀錄 		
                .UseLibreOfficeResultLogger(templateFilename)		// 使用 LibreOffice 格式紀錄 (須給定 template 檔名位置)
                .Build();											// 建構此 Result Logger

>> 宣告ResultLogger<TClass> 時，當中的 TClass 為泛型，用以識別定義好的輸出報告內容。 **首先使用 Construct 初始化，初始化時須傳入檔名(含路徑)**，但不須定義副檔名。副檔名將由後面使用的 Logger 種類附加。

>> 1. **TextFileLoggerFactory**: 使用文字檔紀錄。 在 **UseTextFileLoggerFactory()** 當中，可以 **指定副檔名 (預設為.txt)** ，以及是否輸出欄名稱(預設為 False)。 其輸出格式與輸出 LibreOffice 表單形式相同。 若有指定 [NewSheet] 屬性，則會在檔名後方加上 **"-{NewSheetName}"** 並另存為一個新檔。

>> 2. **JsonResultLoggerFactory**: 使用JSON 格式紀錄。 由於 Result Logger 只有寫入功能，因此不會使用到 Schema。 **註:** 所有屬性皆不適用於 JSON Logger，也就是 [NewSheet], [Skip] 等功能都不會被實現。 詳情請見 [ClassDefinition.html][ClassDef]。

>> 3. **IniFileLoggerFactory**: 使用 ini 格式紀錄。 此格式是使用 **windows 內建 kernel32.dll** 紀錄。此dll 紀錄是以 key-value pair 的形式。 當 ResultClass 中出現巢狀Class 定義時，紀錄的key 會以 Class.Subclass.Subsubclass = value 的方式記錄。

>> 4. **LibreOfficeResultLogger**: 使用 LibreOffice 格式紀錄。 目前版本支援 **新增分頁** 及 **色彩** 功能。 詳情請見 [ClassDefinition.html][ClassDef]。 更多實際案例可以參考**小機器人 Beta 版本中 Ctf**，以及 **NVT PowerMeter 站**紀錄方式。
目前紀錄 LibreOffice 的方式是直接取代 .ods 解壓縮後當中的 content.xml 檔，因此在寫入時需要給定一個模板 (template.ods)。 **此模板不可以被上鎖，若被上鎖要刪掉後重新新增一個，請隨時注意**。

> #### 2.3 給值方式:

>>    	
	// 此為指定紀錄Class 的實例，可以直接對其中某個屬性修改其值。
	ResultLogger<SettingsClass>.Instance

>>    
 	// 例1: 以此SettingsClass 中的內容為例，可以直接修改當中的 Version 屬性
	ResultLogger<SettingsClass>.Instance.Version = "0.0.0.0";	

>>    
	// 例2: 由於Class 本身是 Reference type, 因此可以直接使用local variable 修改其值
	// 但在使用上還是要特別注意傳遞方式，確保傳遞鏈中的每一步都是 pass by reference
	var myResult = ResultLogger<SettingsClass>.Instance;
	myResult.Version = "0.0.0.0"; 									// 會直接修改 ResultLogger<SettingsClass>.Instance

> #### 2.4 輸出方式:


>>    
    ResultLogger<SettingsClass>.Save();								// 會儲存結果至所有使用的輸出類型

>> **註: 由於讀取內容會有格式上的限制，Result Logger 只有 Save 功能。 若須讀取請使用 SettingsLogger。**

> #### 2.5 事件註冊:


>>    
	private void RegisterEvents()
	{
		// 註冊 ErrorOccurs 事件
    	ResultLogger<SettingsClass>.ErrorOccurs += ResultFactory_ErrorOccurs;
	}

>>    
	private void ResultFactory_ErrorOccurs((object sender, Exception exception) obj)
    {
		// 事件處理
        MessageBox.Show($"Error sent from {obj.sender}\r\n{obj.exception.Message}");
    }


### 3. Settings Logger

Setting Logger 是專門用來讀寫設定檔。 由於具備讀取功能，格式上限制較多。 目前支援的內建格式僅有 INI 及 JSON。 但為了因應更廣泛的需求，因此也 **開放客製化的 Settings 讀寫方式**。 僅須自定義一個 Settings Logger 即可。 詳情請見  [CustomSettingsLogger.html][CustomSettingsDef]。

由於設定檔在使用上只會有一種格式，且不太可能需要通時讀多種格式卻同樣內容的設定檔，因此 Settings Logger 在使用上才會限定只能使用 **單一格式** 的 Settings Logger。

**強烈建議一個專案只使用一種 SettingsLogger<TClass>**，也就是只有一種輸出類別(TClass)。 由於一旦 Construct 後，SettingsLogger 內部的所有內容會被全部刷新並初始化。 為避免內容在被Save 前被意外洗掉，使用 Construct 的時機須要多留意。 (可以多次重新 Construct，但一旦Construct 就視為一個新的物件。)

若有需要使用多種 TClass，請見 MultiSettingsLoggers。

> #### 3.1 準備 Settings Class: 

>> 關於Class 定義詳見 [ClassDefinition.html][ClassDef]

 
> #### 3.2 初始化:

>>    
	//	例1. 使用內建格式紀錄
	SettingsLogger<SettingsClass>
	.Construct(@".\Settings")												// Construct 時給定檔名
    .UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON)				// 使用內建格式
    .Build();																// 建構此 Settings Factory

>>    
	// 例2. 使用自訂紀錄方式
	SettingsLogger<SettingsClass>
	.Construct("")															// Construct 時給定的檔名不會被用到
    .UseCustomSettingsLogger(new CustomSettingsLogger(@".\Settings.txt"))	// 自定義一個 CustomSettingsLogger Class。 請參考 CustomSettingsLogger.html
    .Build();

>> **UseBuiltInFormatSettingsLogger**: 目前支援格式僅有 INI 及 JSON。

>> 若是使用自訂讀取，詳情參考 [CustomSettingsLogger.html][CustomSettingsDef]。此自訂設定讀取類別若需要指定檔名，可以在建構子(Constructor)中引入(如上方範例所示)。 **註: 當自訂類別被建立之後，Settings Logger 會自動幫忙接上 ErrorOccurs Event。**，因此同樣適用 3.5 節中事件的註冊方式。 

> #### 3.3 給值方式 (與 2.3 節 相同，不再贅述):

> #### 3.4 讀寫方式:

>>    
	SettingsLogger<SettingsClass>.Read(); 							// 讀取
    SettingsLogger<SettingsClass>.Save();							// 寫入

>> **註: 讀取及寫入的檔案皆已經在 Construct 時就被定義。讀取時尤其要注意使否使用格式與讀取檔案的內容格式相同**。

> #### 3.5 事件註冊:

>>    
	private void RegisterEvents()
	{
		// 註冊 ErrorOccurs 事件 (無論內建或自訂類別，皆適用)
    	SettingsLogger<SettingsClass>.ErrorOccurs += SettingsFactory_ErrorOccurs;
	}

>>    
	private void SettingsFactory_ErrorOccurs((object sender, Exception exception) obj)
    {
		// 事件處理
        MessageBox.Show($"Error sent from {obj.sender}\r\n{obj.exception.Message}");
    }

---

### 4. MultiBasicLoggers

MultiBasicLoggers 適用於在單一專案使用多個 BasicLogger 的時候。 每一個BasicLogger 使用方式與第 1 節相同，但在此提供一個可以新增多個BasicLogger 在單一專案中的方案。

> #### 4.1 初始化:


>>    
    MultiBasicLoggers.Construct()																// 建構 MultiBasicLoggers
        .AddFactory("Basic1", new BasicLoggerFactory()											// 新增一個BasicLoggerFactory (須給定Factory 名稱，並 new 出一個BasicLoggerFactory
            .UseCustomFunctionAsyncLogger(t => Invoke(new Action(() => labelLog1.Text = t)))	// 使用 CustomFunctionAsyncLogger 於 第一個 BasicLoggerFactory)
            .UseFileLogger(@".\Basic1.ini"))													// 使用 FileLogger 於 第一個 BasicLoggerFactory
        .AddFactory("Basic2", new BasicLoggerFactory()											// 再新增一個BasicLoggerFactory (須給定Factory 名稱，並 new 出一個BasicLoggerFactory)
            .UseCustomFunctionAsyncLogger(t => Invoke(new Action(() => labelLog2.Text = t)))	// 使用 CustomFunctionAsyncLogger 於 第二個 BasicLoggerFactory
            .UseFileLogger(@".\Basic2.ini"))													// 使用 FileLogger 於 第二個 BasicLoggerFactory
        .Build();																				// 完成建構


>> MultiBasicLoggers 首先在 Construct 完成之後，可以直接使用 AddFactory 方法來新增一個 BasicLoggerFactory。 這些 LoggerFactories 會以 Dictionary 的方式存在 MultiBasicLoggers 當中，因此 **須要給定 Factory 名稱** 以便之後呼叫。

>> 在 AddFactory 的第二個參數中，直接 new 出一個新的 BasicLoggerFactory，選擇好所有需要使用的輸出格式即可。

>> Construct 後所接續的 AddFactory 為初始化的 Factories。 但情況常常是沒有一定要在最一開始就新增好所有可能會用到的 Factories, 因此也 **支援在之後動態增減 Factories (見 4.2)**。

>> **註 1: Construct 後會刷新 MultiBasicLoggers 的 Dictionary，設計原則是整個專案中只須初始化一次就好，因此強烈建議使用 註2 的方式撰寫。**

>> **註 2: MultiBasicLoggers 可以動態增減 Loggers，因此可以在程式最一開始時就只使用 `MultiBasicLoggers.Construct().Build();` 初始化，並在之後式情況動態增減 Loggers。**

> #### 4.2 動態增減 BasicFactory

>>    
	// 初始化過後，程式任一處可以在MultiBasicLoggers 後直接使用 AddFactory, 不需要 Construct 及 Build。
 	MultiBasicLoggers.AddFactory("Basic1", new BasicLoggerFactory()
          .UseCustomFunctionAsyncLogger(t => Invoke(new Action(() => labelLog1.Text = t)))
          .UseFileLogger(@".\Basic1.ini"));

>>    
	// 初始化過後，程式任一處可以在MultiBasicLoggers 後直接使用 RemoveFactory 來移除不再使用的 Factory (給定要移除的 Factory 名稱)。
 	MultiBasicLoggers.RemoveFactory("Basic1");

>> **註1: 使用動態新增之前一定要先經過 4.1 的初始化過程。**
>
>> **註2: 由於已經初始化過，在這裡不需要再 Construct 及 Build。**

> #### 4.3 紀錄方式

>>    
	string msg = "Some message to log.";	
    MultiBasicLoggers.Log("Basic1", msg);						// 首先指定一個BasicLoggerFactory的名稱，再傳入要紀錄的訊息。

>> 若指定的BasicLoggerFactory名稱不存在，會丟出不存在的錯誤訊息。

> #### 4.4 事件註冊:
 

>>    
    MultiBasicLoggers.ErrorOccurs += BasicLogger_ErrorOccurs;	// 註冊ErrorOccurs 事件。

>> 事件註冊基本與 BasicLoggers 相同。 所有Logger 丟出的錯誤訊息都會集中在MultiBasicLoggers.ErrorOccurs 被觸發。



### 5. MultiResultLoggers

MultiResultLoggers 適用於在單一專案使用多個 ResultLogger 的時候。 ResultLogger 使用方法與第 2 節相同，但提供一個方案可以新增多個ResultLogger 在單一專案中。

> #### 5.1 初始化:

>>
    MultiResultLoggers.Construct()													// 建構 MultiResultLoggers
            .AddFactory(new ResultLoggerFactory<SettingsClass>(@".\MultiResult1")	// 新增一個新的 SettingsClass typed ResultLoggerFactory 
                    .UseLibreOfficeResultLogger(templateFilename)					// 在第一個Factory 中使用 LibreOffice format 輸出	
                    .UseJsonResultLogger())											// 在第一個Factory 中使用 JSON format 輸出
            .AddFactory(new ResultLoggerFactory<CtfPointSetClass>(@".\MultiResult2")// 新增一個 CtfPointSetClass typed ResultLoggerFactory
                    .UseTextFileLoggerFactory()										// 使用文字檔格式輸出
                    .UseIniFileLoggerFactory())										// 使用 kernel32.dll ini 輸出格式格式輸出
            .Build();																// 完成建構

>> MultiResultLoggers 首先在 Construct 完成之後，可以直接使用 AddFactory 方法來新增一個 ResultLoggerFactory。 這些 LoggerFactories 會以 Dictionary 的方式存在 MultiResultLoggers 當中，並且 **其Factory 名稱即為代入的 ClassType 名稱**。

>> 在 AddFactory 中直接 new 出一個新的 ResultLoggerFactory。 new 出的 ResultLoggerFactory **需要給定一個輸出檔名(含路徑)**，選擇好所有需要使用的輸出格式即可。

>> Construct 後所接續的 AddFactory 為初始化的 Factories。 但情況常常是沒有一定要在最一開始就新增好所有可能會用到的 Factories, 因此也 **支援在之後動態增減 Factories (見 5.2)**。

>> **註 1: Construct 後會刷新 MultiResultLoggers 的 Dictionary，設計原則是整個專案中只須初始化一次就好，因此強烈建議使用 註2 的方式撰寫。**

>> **註 2: MultiResultLoggers 可以動態增減 Loggers，因此可以在程式最一開始時就只使用 `MultiResultLoggers.Construct().Build();` 初始化，並在之後式情況動態增減 Loggers。**

> #### 5.2 動態增減 ResultLoggerFactory 

>>
	MultiResultLoggers.AddFactory(new ResultLoggerFactory<SettingsClass>(@".\MultiResult1")	// 初始化後，在 MultiResultLoggers 後直接呼叫 AddFactory，並 new 出一個 ResultLoggerFactory
	                .UseLibreOfficeResultLogger(templateFilename)							// 選擇使用LibreOffice 輸出格式
	                .UseJsonResultLogger());												// 選擇使用 Json 輸出格式
>>
	MultiBasicLoggers.RemoveFactory(nameof(SettingsClass));									// 移除指定 ResultLoggerFactory

>> **註 1: 請一定記得先經過 5.1 的初始化過程。 動態增減不需要 Construct 及 Build。**

>> **註 2: ResultLoggerFactory 的名稱即為代入的 ClassType 的名稱，因此在移除時可以多利用 "nameof()" 這個方法。如範例所示。**


> #### 5.3 給值方式

>>    	
	// 指定特定紀錄類別的實例，可以直接對其中某個屬性修改其值。
	MultiResultLoggers.Instance<SettingsClass>();

>>    
 	// 例1: 以此SettingsClass 中的內容為例，可以直接修改當中的 Version 屬性
	MultiResultLoggers.Instance<SettingsClass>().Version = "0.0.0.0";	

>>    
	// 例2: 由於Class 本身是 Reference type, 因此可以直接使用local variable 修改其值
	// 但在使用上還是要特別注意傳遞方式，確保傳遞鏈中的每一步都是 pass by reference
	var myResult = MultiResultLoggers.Instance<SettingsClass>();
	myResult.Version = "0.0.0.0"; 									// 會直接修改 MultiResultLoggers.Instance<SettingsClass>()


> #### 5.4 儲存方式

>>
    MultiResultLoggers.Save(nameof(SettingsClass));					// 存下指定 ResultLogger 的結果
    MultiResultLoggers.SaveAll();									// 儲存所有 ResultLogger 的結果

> #### 5.5 事件註冊
>>    
    MultiResultLoggers.ErrorOccurs += ResultLogger_ErrorOccurs;	// 註冊ErrorOccurs 事件。

>> 事件註冊基本與 ResultLoggers 相同。 所有Logger 丟出的錯誤訊息都會集中在MultiResultLoggers.ErrorOccurs 被觸發。


### 6. MultiSettingsLoggers

MultiSettingsLoggers 適用於在單一專案使用多個 SettingsLogger 的時候。 SettingsLogger 使用方法與第 3 節相同，但提供一個方案可以新增多個 SettingsLogger 在單一專案中。

> #### 6.1 初始化:

>>
	MultiSettingsLoggers.Construct()													// 建構 MultiSettingsLoggers
	    .AddFactory(new SettingsLoggerFactory<CtfPointSetClass>(@".\settings1")			// 新增一個 CtfPointSetClass typed SettingsLoggerFactory，檔名為 @".\settings1"
			.UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON))					// 使用 JSON Format
	    .AddFactory(new SettingsLoggerFactory<DistortionResultClass>(@".\settings2"		// 新增一個 DistortionResultClass typed SettingsLoggerFactory，檔名為 @".\settings2"
			.UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON))					// 使用 JSON Format
	    .AddFactory(new SettingsLoggerFactory<SettingsClass>(@".\settings3")			// 新增一個 SettingsClass typed SettingsLoggerFactory，檔名為 @".\settings3"
			.UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON))					// 使用 JSON Format
	    .AddFactory(new SettingsLoggerFactory<DistortionSettingsClass>(@".\settingsIni")// 新增一個 DistortionSettingsClass typed SettingsLoggerFactory，檔名為 @".\settingsIni"
			.UseBuiltInFormatSettingsLogger(SettingsFileFormat.INI))					// 使用 kernal32.dll ini 輸出格式
	    .Build();																		// 完成建構
>> MultiSettingsLoggers 首先在 Construct 完成之後，可以直接使用 AddFactory 方法來新增一個 SettingsLoggerFactory。 這些 LoggerFactories 會以 Dictionary 的方式存在 MultiSettingsLoggers 當中，並且 **其Factory 名稱即為代入的 ClassType 名稱**。

>> 在 AddFactory 中直接 new 出一個新的 SettingsLoggerFactory。 new 出的 SettingsLoggerFactory **需要給定一個輸出檔名(含路徑)**，選擇好所有需要使用的輸出格式即可。

>> Construct 後所接續的 AddFactory 為初始化的 Factories。 但情況常常是沒有一定要在最一開始就新增好所有可能會用到的 Factories, 因此也 **支援在之後動態增減 Factories (見 6.2)**。

>> **註 1: Construct 後會刷新 MultiSettingsLoggers 的 Dictionary，設計原則是整個專案中只須初始化一次就好，因此強烈建議使用 註2 的方式撰寫。**

>> **註 2: MultiSettingsLoggers 可以動態增減 Loggers，因此可以在程式最一開始時就只使用 `MultiSettingsLoggers.Construct().Build();` 初始化，並在之後式情況動態增減 Loggers。**

> #### 6.2 動態增減 ResultLoggerFactory 

>>
	MultiSettingsLoggers
		.AddFactory(new SettingsLoggerFactory<CtfPointSetClass>(@".\settings1")	// 新增一個 CtfPointSetClass typed SettingsLoggerFactory，檔名為 @".\settings1"
			.UseBuiltInFormatSettingsLogger(SettingsFileFormat.JSON))			// 使用 JSON Format
>>
	MultiSettingsLoggers.RemoveFactory(nameof(CtfPointSetClass));				// 移除指定 SettingsLoggerFactory

>> **註 1: 請一定記得先經過 5.1 的初始化過程。 動態增減不需要 Construct 及 Build。**

>> **註 2: SettingsLoggerFactory 的名稱即為代入的 ClassType 的名稱，因此在移除時可以多利用 "nameof()" 這個方法。如範例所示。**


> #### 6.3 給值方式

>>    	
	// 指定特定紀錄類別的實例，可以直接對其中某個屬性修改其值。
	MultiSettingsLoggers.Instance<SettingsClass>();

>>    
 	// 例1: 以此SettingsClass 中的內容為例，可以直接修改當中的 Version 屬性
	MultiSettingsLoggers.Instance<SettingsClass>().Version = "0.0.0.0";	

>>    
	// 例2: 由於Class 本身是 Reference type, 因此可以直接使用local variable 修改其值
	// 但在使用上還是要特別注意傳遞方式，確保傳遞鏈中的每一步都是 pass by reference
	var mySettings = MultiSettingsLoggers.Instance<SettingsClass>();
	mySettings.Version = "0.0.0.0"; 									// 會直接修改 MultiSettingsLoggers.Instance<SettingsClass>()


> #### 6.4 讀寫方式

>>
    MultiSettingsLoggers.Save(nameof(SettingsClass));					// 儲存指定 SettingsLogger 的結果
    MultiSettingsLoggers.SaveAll();										// 儲存所有 SettingsLogger 的結果
    MultiSettingsLoggers.Read(nameof(SettingsClass));					// 讀取指定 SettingsLogger 的結果
    MultiSettingsLoggers.ReadAll();										// 讀取所有 SettingsLogger 的結果

> #### 6.5 事件註冊
>>    
    MultiSettingsLoggers.ErrorOccurs += SettingsLogger_ErrorOccurs;		// 註冊ErrorOccurs 事件。


## LoggerManagerLibrary 設定 

此版本的 LoggerManager 可以設定如何存下 LoggerManagerLibrary 中本身的錯誤訊息。 預設情況為存下 LoggerManagerLibrary.log 於應用程式路徑底下，且不覆蓋檔案而是持續附加在舊檔後面。

> ### 設定方法
>>     
    LoggerManagerConfiguration.OverrideDebugFile = true;  // 是否每次都覆蓋舊檔 (預設為 False)
    LoggerManagerConfiguration.SaveDebugFile = false;     // 是否存下由 LoggerManagerLibrary 中丟出的debug訊息 (預設為 True)         

>> 此設定方法建議寫在程式最一開始。

[ClassDef]: ./ClassDefinition.html
[CustomSettingsDef]: ./CustomSettingDefinition.html



