using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;//获取绑定的接口
namespace Model
{
   public class Country
    {
        private event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        public string Name { get { return _name; }  set { _name = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            } }
        private string _countryPath;
        public string CountryPath { get { return _countryPath; } set { _countryPath = value;

                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(CountryPath)));
                }
            } }
    }
}
