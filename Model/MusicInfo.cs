using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Model
{
   public class MusicInfo
    {    //绑定的接口
        private event PropertyChangedEventHandler PropertyChanged;
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(ID)));
                }
            }
        }
        private string _musicName;
        public string MusicName
        {
            get { return _musicName; }
            set { _musicName = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(MusicName)));
                }
            }
        }
        private string _musicCountry;
        public string MusicCountry
        {
            get { return _musicCountry; }
            set { _musicCountry = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(MusicCountry)));
                }
            }
        }
        private string _musicAuthor;
        public string MusicAuthor
        {
            get { return _musicAuthor; }
            set { _musicAuthor = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(MusicAuthor)));
                }
            }
        }
        private string _musicTime;
        public string MusicTime
        {
            get { return _musicTime; }
            set { _musicTime = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(MusicTime)));
                }
            }
        }
        private string _musicSpecial;
        public string MusicSpecial
        {
            get { return _musicSpecial; }
            set { _musicSpecial = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(MusicSpecial)));
                }
            }
        }
        private string _musicFilePath;
        public string MusicFilePath
        {
            get { return _musicFilePath; }
            set { _musicFilePath = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(MusicFilePath)));
                }
            }
        }
        private string _musicSize;
        public string MusicSize
        {
            get { return _musicSize; }
            set { _musicSize = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(MusicSize)));
                }
            }
        }
        /// <summary>
        /// 当前音乐下载的进度
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
        private ImageSource _musicPicture;
        public  ImageSource MusicPicture
        {
            get { return _musicPicture; }
            set { _musicPicture = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(MusicPicture)));
                }
            }
        }
    }
}
