using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
     public class SetViewModel:INotifyPropertyChanged
    {public event PropertyChangedEventHandler PropertyChanged;
        private string _folder;
        public string Folder
        {
            get { return _folder; }
            set { _folder = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(Folder)));
                }
            }
        }

        
    }
}
