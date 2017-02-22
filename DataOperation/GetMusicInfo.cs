using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace DataOperation
{
  public  class GetMusicInfo
    {
        public List<MusicInfo> GetMusics(string country)
        { List<MusicInfo> musicinfos = new List<MusicInfo>();
            string sql = "select musicID,musicname,musicauthor,musictime,MusicFileSize,musicfilepath,musiccountry,MusicSpecial from musicinfo where musiccountry=" + "'"+country+"'";
            DataTable dt= SqlHelper.ExcuterQuery(sql);
            foreach (DataRow item in dt.Rows)
            {
                MusicInfo info = new MusicInfo();
                info.ID = int.Parse(item["MusicID"].ToString());
                info.MusicName = item["MusicName"].ToString();
                info.MusicCountry = item["MusicCountry"].ToString();
                info.MusicAuthor = item["MusicAuthor"].ToString();
                info.MusicTime = item["MusicTime"].ToString();
                int index = info.MusicTime.IndexOf(":");
                info.MusicTime = info.MusicTime.Substring(index + 1);
                info.MusicSpecial = item["MusicSpecial"].ToString();
                info.MusicFilePath = item["MusicFilePath"].ToString();
                info.MusicSize = item["MusicFileSize"].ToString();
                musicinfos.Add(info);
            }
            return musicinfos;
        }

        public  MusicInfo GetMusics( int id)
        {
            MusicInfo info = new MusicInfo();
            string sql = "select * from musicinfo where MusicID=" + id;
            DataTable dt = SqlHelper.ExcuterQuery(sql);
            foreach (DataRow item in dt.Rows)
            {
               
                info.ID = int.Parse(item["MusicID"].ToString());
                info.MusicName = item["MusicName"].ToString();
                info.MusicCountry = item["MusicCountry"].ToString();
                info.MusicAuthor = item["MusicAuthor"].ToString();
                info.MusicTime = item["MusicTime"].ToString();
                info.MusicSpecial = item["MusicSpecial"].ToString();
                info.MusicFilePath = item["MusicFilePath"].ToString();
                info.MusicSize = item["MusicFileSize"].ToString();
               
            }
            return info;
        }
        public byte[] GetMusicPicture(int id)
        {
            byte[] image = null;
            string sql = "select musicpicture from MusicInfo where musicid=" + id;
            image = SqlHelper.QueryMusicPicture(sql);
            //MemoryStream ms = new MemoryStream(image);
            //BitmapImage musicPicture = new BitmapImage();
            //musicPicture.BeginInit();
            //musicPicture.StreamSource = ms;
            //musicPicture.EndInit();
            return image;
        }
    }
}
