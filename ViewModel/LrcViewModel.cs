using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
   public class LrcViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public List<LrcInfo> LrcInfo { get; set; }
        private string _upLrc;
        public string UpLrc {
            get { return  _upLrc; }
            set { _upLrc = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(UpLrc)));
                }
            }
        }
        private string _downLrc;
        public string DownLrc
        {
            get { return _downLrc; }
            set { _downLrc = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(DownLrc)));
                }
            }
        }
        private string _nowLrc;

        public string NowLrc {
            get { return _nowLrc; }
            set { _nowLrc = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(NowLrc)));
                }
            }
        }

       
    }
}
