# Custom Settings Definition

在使用 Settings Logger 時，當中包含了讀取功能。 由於讀取功能受格式影響較大，因此開放使用者自訂讀寫功能，並且在使用的同時還能保持程式碼的統一性。

## 使用方法:

欲使用自訂讀寫功能，只需要讓自訂的類別繼承 **ISettingsLogger< TClass >** 即可。 **其中 TClass 為 紀錄用類別** (見 [ClassDefinition.html][ClassDef])。

### 繼承範例

> 1. 首先新增一類別，繼承 LoggerManagerLibrary 中的 ISettingsLogger< TClass >

>
		public class CustomSettingsLogger : ISettingsLogger<SettingsClass> 
		{	
	    }

> 2. 繼承後 ISettingsLogger 底下會標紅線，因為尚未實作介面成員。 此時將游標移動到 ISettingsLogger上，會出現小燈泡圖示。

> 3. 點下小燈泡圖示，選取 "實作介面成員"，程式碼將自動產生所有該實作的介面成員。 由於
 **自訂類別並不會自動代入 Construct 中的檔名，因此在這裡建構子中必須自己帶入檔名**。
> 
> 例:

>
        public class CustomSettingsLogger : ISettingsLogger<SettingsClass>
        {
                public SettingsClass SettingsClassInstance { get; set; }
                public string Filename { get; set; }
                public string Schema { get; set; }
>
                public event Action<(object sender, Exception exception)> ErrorOccurs;
>
                public CustomSettingsLogger(string filename)
                {
                        Filename = filename;
                }
>
                public void Build()
                {
                    // Leave it empty.
                }
>
                public bool Read()
                {
					// Do something.
                }
>
                public bool Save()
                {
					// Do something.
                }
        }

>    接下來只需要實作 Read/ Save/ Build 即可。 (自定義可以省略 Build，直接留白)。

### 使用自訂讀寫功能:


> 自訂讀寫功能在使用上與使用內建格式的方法無異，僅須記得在Construct 中的檔名不會進到自訂讀寫功能中。
>
    SettingsLogger<SettingsClass>.Construct("")									// Construct 中的檔名並不會被使用到，因此可以直接留白
        .UseCustomSettingsLogger(new CustomSettingsLogger(@".\Settings.txt"))	// 在此新增出自訂的讀寫功能Class，並代入檔名。
        .Build();																//完成建置
>
**註: 建置過後自訂讀寫功能中的錯誤也會被 SettingsLogger 中的 ErrorOccurs 捕捉，並且自訂類別中的 SettingsClassInstance 也會被連接到 SettingsLogger.instance。 因此後續使用與內建均無差別。**

[ClassDef]: ./ClassDefinition.html