using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Model;

namespace Control
{
 public   class DataLrc
    {
        private List<string> LrcFile { get; set; }
        public List<LrcInfo> LrcInfo { get; set; }
        public List<int> TimeSpan { get; set; }
        public List<string> Title { get; set; }
        //获得目录下的Lrc文件
        public void GetLrcFile(string fileName)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(fileName);
                //获得文件名
                string name = Path.GetFileName(fileName);
                int index = name.LastIndexOf(".");
                name = name.Substring(0, index);
                //判断当前目录是否存在文件
                if (File.Exists(folderPath + "\\" + name + ".lrc"))
                {
                    string lrcPath = folderPath + "\\" + name + ".lrc";
                    //如果存在，就读取文件
                    List<string> lrc = File.ReadAllLines(lrcPath).ToList();
                    for (int i = 0; i < lrc.Count; i++)
                    {
                        if (lrc[i] == "")
                        {
                            lrc.RemoveAt(i);
                        }
                    }
                    //当前移除空数据完成
                    LrcFile = lrc;

                }
            }
            catch(Exception ex)

            {
                string smg = ex.Message;
                return;
            }
        }
        //解析每一行数据
        public void MatchLrcInfo()
        {
            if (LrcFile == null)
            {
                return;
            }
            LrcInfo = new List<Model.LrcInfo>();
            Regex r = new Regex(@"\[\d{2}:\d{2}(.\d{2})*\]");
            Title = new List<string>();
            TimeSpan = new List<int>();
            //移除LrcFile前两句没有的数据
            LrcFile.RemoveRange(0, 2);
            for (int i = 0; i < LrcFile.Count; i++)
            {
                var mathches = r.Match(LrcFile[i]);
                //获得当前的时间 和 文字
                string time = mathches.Value;
                int index = time.LastIndexOf("]");
                time = time.Substring(1, index-1);
                string geci = LrcFile[i].Substring(index + 1);
                int muintue = int.Parse(time.Substring(1, 1));
                 string second = time.Substring(3);
                double d = double.Parse(second);
                LrcInfo info = new Model.LrcInfo();
                info.LrcStr = geci;
                
                Title.Add(geci);
                info.LrcTime = muintue * 60 + (int)d;
                
                TimeSpan.Add(muintue * 60 + (int)d);
                LrcInfo.Add(info);
            }
        }
    }
}
