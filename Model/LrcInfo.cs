using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
  public  class LrcInfo:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string lrcStr;
        public string LrcStr
        {
            get { return lrcStr; }
            set { lrcStr = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(lrcStr)));
                }
            }
        }
        private int _lrcTime;
        public int LrcTime
        {
            get { return _lrcTime; }
            set { _lrcTime = value;

                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(LrcTime)));
                }
            }
        }
        
    }
}
