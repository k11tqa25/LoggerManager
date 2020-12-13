using LoggerManagerLibrary;
using System;

namespace LoggerManagerExample
{
        public class CustomSettingsLogger : ISettingsLogger<SettingsClass>
        {
                public SettingsClass SettingsClassInstance { get; set; }
                public string Filename { get; set; }
                public string Schema { get; set; }

                public event Action<(object sender, Exception exception)> ErrorOccurs;

                public CustomSettingsLogger(string filename)
                {
                        Filename = filename;
                }

                public void Build()
                {
                        throw new NotImplementedException();
                }

                public bool Read()
                {
                        throw new NotImplementedException();
                }

                public bool Save()
                {
                        throw new NotImplementedException();
                }
        }
}




