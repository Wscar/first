using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class NataveMusic : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
                }
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }
    }
}
