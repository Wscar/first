using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DownLoadMusic : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string _name;
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
        /// <summary>
        /// 当前音乐下载进度
        /// </summary>
        private int _Percint;
        public int Percint
        {
            get { return _Percint; }
            set
            {
                _Percint = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(Percint)));
                }
            }
        }
        private string _fileSize;
        public string FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(FileSize)));
                }
            }
        }
        private string _complete;
        public string Complete
        {
            get { return _complete; }
            set { _complete = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(Complete)));
                }
            }
        }
        private string _schedule;
        public string Schedule
        {
            get { return _schedule; }
            set { _schedule = value;
                PropertyChangedEventHandler handeler = this.PropertyChanged;
                if (handeler != null)
                {
                    handeler.Invoke(this, new PropertyChangedEventArgs(nameof(Schedule)));
                }
            }
        }
        private string _author;
        public string Author
        {
            get { return _author; }
            set { _author = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(Author)));
                }
            }
            
        }

    }
}
