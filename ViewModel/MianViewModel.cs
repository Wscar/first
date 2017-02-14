using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DataOperation;
using System.ComponentModel;
using System.IO;
using Shell32;
using Control;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Windows;
namespace ViewModel
{
  public   class MianViewModel: INotifyPropertyChanged
    {
        //绑定的结构用来更新 listView的selectitem
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 测试的
        /// </summary>
        public ObservableCollection<People> Peoples { get; set; }
        /// <summary>
        /// listBox展示音乐所属国家的集合
        /// </summary>
        public ObservableCollection<Country> Countrys { get; set; }
        public Country SlectecItem { get; set; }
        public ObservableCollection<MusicInfo> MusicInfos { get; set; }
        public ObservableCollection<MusicInfo> NativeMusic { get; set; }
        private MusicInfo _selectItemNativeMusic;
        public  MusicInfo SelectItemNativeMusic
        {
            get { return _selectItemNativeMusic; }
            set { _selectItemNativeMusic = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                hander.Invoke(this, new PropertyChangedEventArgs(nameof(SelectItemNativeMusic)));
            }
        }
        private MusicInfo _selectItemMusic;
        /// <summary>
        /// listView选择的item
        /// </summary>
        public MusicInfo SelectItemMusic {
            get { return _selectItemMusic; }
            set { _selectItemMusic = value;
                PropertyChangedEventHandler hander = this.PropertyChanged;
                if (hander != null)
                {
                    hander.Invoke(this, new PropertyChangedEventArgs(nameof(SelectItemMusic)));
                }
            }
        }
        /// <summary>
        /// 当前的歌曲数
        /// </summary>
        public string MusicCount { get; set; }
        private MusicInfo _musicPlaying;
        /// <summary>
        /// 当前正在播放的歌曲
        /// </summary>
        public MusicInfo MusicPlaying
        {
            get { return _musicPlaying; }
            set { _musicPlaying = value;
                PropertyChangedEventHandler handel = this.PropertyChanged;
                if (handel != null)
                {
                    handel.Invoke(this, new PropertyChangedEventArgs(nameof(MusicPlaying)));
                }
            }
        }
        private string time;
        public string Time
        {
            get { return time; }
            set { time = value;
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
                }
            }
        }

        
        public ObservableCollection<DownLoadMusic> DownLoadMusic
        {
            get;
            set;
        }
        public int i
        {
            get;set;
        }
        /// <summary>
        /// 检索Country.xml保存的图片路径
        /// </summary>
        /// <returns></returns>
        public   void QueryXml()
        {
            ObservableCollection<Country> info = new ObservableCollection<Country>();
            XDocument doc = XDocument.Load("Country.xml");
            XElement root = doc.Element("Countrys");
            IEnumerable<XElement> Countrys = root.Elements();
            foreach (XElement emp in Countrys)
            {
                Country c = new Country();
                XElement name = emp.Element("Name");
                c.Name = name.Value;
                XElement path = emp.Element("Path");
                c.CountryPath = path.Value;
                info.Add(c);
            }
            this.Countrys = info;
          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Country">按国家查询</param>
        public void GetMusicInfos(string Country)
        {
            GetMusicInfo musicInfo = new GetMusicInfo();
            List<MusicInfo> info= musicInfo.GetMusics(Country);
            this.MusicInfos = new ObservableCollection<MusicInfo>(info);
            this.MusicCount = MusicInfos.Count.ToString();
        }
        /// <summary>
        /// 按ID查询得到当前对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MusicInfo GetMusicInfos(int id)
        {
            GetMusicInfo musicInfo = new GetMusicInfo();
             MusicInfo  info = musicInfo.GetMusics(id);
            return info;
        }
        public  void GetNativeMusic()
        {
            this.NativeMusic = new ObservableCollection<MusicInfo>();
            var dialog = new System.Windows.Forms.OpenFileDialog();
       
            dialog.Title = "请选择要打开的文件";
            dialog.InitialDirectory = "c:\\";
            dialog.Filter = "MP3文件|*.mp3";
            dialog.Multiselect = true;
            dialog.ShowDialog();
            List<string> filepath = dialog.FileNames.ToList();
            for (int i = 0; i < filepath.Count; i++)
            {
                string fileName = Path.GetFileName(filepath[i]);
                ID3V2 id3v2 = new ID3V2();
                BitmapImage image = id3v2.ReadMp3(filepath[i]);
               
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
             ;
               
                int index = fileName.LastIndexOf(".");
                fileName = fileName.Substring(0, index);
                ShellClass sh = new ShellClass();
                Folder dir = sh.NameSpace(Path.GetDirectoryName(filepath[i]));
                FolderItem item = dir.ParseName(Path.GetFileName(filepath[i]));
                string songTime = dir.GetDetailsOf(item, 27);
                int timeIndex = songTime.IndexOf(":");
                songTime = songTime.Substring(timeIndex+1);
                //if (image != null)
                //{
                //    FileStream fileStream = new FileStream("MusicPicture\\" + fileName + ".jpeg", FileMode.Create, FileAccess.ReadWrite);
                //    encoder.Save(fileStream);
                //    fileStream.Close();
                //}
              
                MusicInfo nm = new MusicInfo { MusicFilePath=filepath[i],MusicName=id3v2.Name,MusicAuthor=id3v2.Author ,MusicTime=songTime };
                NativeMusicXML(nm);
                NativeMusic.Add(nm);
            }
        }
        /// <summary>
        /// 把选择的音乐信息保存到XML文档
        /// </summary>
        /// <param name="info"></param>
        public void NativeMusicXML(MusicInfo info)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement musics = null;
            if (File.Exists("NativeMusic.xml"))
            {
                doc.Load("NativeMusic.xml");
                musics = doc.DocumentElement;
            }
            else
            {
            
            }
            //创建子节点
            XmlElement musicInfo = doc.CreateElement("MusicInfo");
            musics.AppendChild(musicInfo);
            XmlElement MusicName = doc.CreateElement("MusicName");
            MusicName.InnerText = info.MusicName;
            musicInfo.AppendChild(MusicName);
            XmlElement author = doc.CreateElement("MusicAuthor");
            author.InnerText = info.MusicAuthor;
            musicInfo.AppendChild(author);
            XmlElement MusicPath = doc.CreateElement("MusicPath");
            MusicPath.InnerText = info.MusicFilePath;
            musicInfo.AppendChild(MusicPath);
           
            XmlElement SongTime = doc.CreateElement("SongTime");
            SongTime.InnerText = info.MusicTime;
            musicInfo.AppendChild(SongTime);
            doc.Save("NativeMusic.xml");
        }
        /// <summary>
        /// 得到当前存储本地音乐信息的xml文档
        /// </summary>
        public void QueryNativeMusicXML()
        {
            ObservableCollection<MusicInfo> NativeMusicInfo = new ObservableCollection<MusicInfo>();
            XDocument doc = XDocument.Load("NativeMusic.xml");
            XElement root = doc.Element("Musics");
            IEnumerable<XElement> Musics = root.Elements();
            foreach (XElement emp in Musics)
            {
                MusicInfo info = new MusicInfo();
                XElement name = emp.Element("MusicName");
                info.MusicName = name.Value;
                XElement path = emp.Element("MusicPath");

               info.MusicFilePath = path.Value;
                ID3V2 id3v2 = new ID3V2();
                BitmapImage image = id3v2.ReadMp3(path.Value);
                info.MusicPicture = image;
                XElement songtime = emp.Element("SongTime");
                info.MusicTime = songtime.Value;
                XElement author = emp.Element("MusicAuthor");
                info.MusicAuthor = author.Value;
                NativeMusicInfo.Add(info);
            }
            this.NativeMusic = NativeMusicInfo;
        }
    }
}
