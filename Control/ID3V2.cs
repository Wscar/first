using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Control
{
  public  class ID3V2
    {

        public string Name { get; set; }
        public string Author { get; set; }
        public byte[] ImageByte { get; set; }
        public BitmapImage ReadMp3(string path)
        {
            BitmapImage image = new BitmapImage();
            string[] tags = new string[6];
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[10];
            // fs.Read(buffer, 0, 128);
            string mp3ID = "";
            fs.Seek(0, SeekOrigin.Begin);
            fs.Read(buffer, 0, 10);
            int size = (buffer[6] & 0x7F) * 0x200000 + (buffer[7] & 0x7F) * 0x400 + (buffer[8] & 0x7F) * 0x80 + (buffer[9] & 0x7F);
            //int size = (buffer[6] & 0x7F) * 0X200000 * (buffer[7] & 0x7f) * 0x400 + (buffer[8] & 0x7F) * 0x80 + (buffer[9]);
            mp3ID = Encoding.Default.GetString(buffer, 0, 3);
            if (mp3ID.Equals("ID3", StringComparison.OrdinalIgnoreCase))
            {
                //如果有扩展标签头就跨过 10个字节
                if ((buffer[5] & 0x40) == 0x40)
                {
                    fs.Seek(10, SeekOrigin.Current);
                    size -= 10;
                }
             image = ReadFrame(fs, size);
            }
            return image;
        }
     private BitmapImage ReadFrame(FileStream fs, int size)
        {
            BitmapImage image= new BitmapImage();
         
            byte[] buffer = new byte[10];
            while (size > 0)
            {
                //fs.Read(buffer, 0, 1);
                //if (buffer[0] == 0)
                //{
                //    size--;
                //    continue;
                //}
                //fs.Seek(-1, SeekOrigin.Current);
                //size++;
                //读取标签帧头的10个字节
                fs.Read(buffer, 0, 10);
                size -= 10;
                //得到标签帧ID
                string FramID = Encoding.Default.GetString(buffer, 0, 4);
                //计算标签帧大小，第一个字节代表帧的编码方式
                int frmSize = 0;

                frmSize = buffer[4] * 0x1000000 + buffer[5] * 0x10000 + buffer[6] * 0x100 + buffer[7];
                if (frmSize == 0)
                {
                    //就说明真的没有信息了
                    break;
                }
                //bFrame 用来保存帧的信息
                byte[] bFrame = new byte[frmSize];
                fs.Read(bFrame, 0, frmSize);
                size -= frmSize;
                string str = GetFrameInfoByEcoding(bFrame, bFrame[0], frmSize - 1);
                if (FramID.CompareTo("TIT2") == 0)
                {
                    this.Name = str;
                }
                else if (FramID.CompareTo("TPE1") == 0)
                {
                    this.Author =  str;
                }
                if (FramID.CompareTo("APIC") == 0)
                {
                    int i = 0;
                    while (true)
                    {
                        if ((255 == bFrame[i] && 216 == bFrame[i + 1]) || (137 == bFrame[i] && 80 == bFrame[i + 1]))
                        {   //在
                            break;
                        }
                        i++;
                    }
                    byte[] imge = new byte[frmSize - i];
                    fs.Seek(-frmSize + i, SeekOrigin.Current);
                    fs.Read(imge, 0, imge.Length);
                    MemoryStream ms = new MemoryStream(imge);
                     
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.EndInit();
                    this.ImageByte = imge;
                    fs.Close();
                }
                
                

            }
            return image;
        }
       private string GetFrameInfoByEcoding(byte[] b, byte conde, int length)
        {
            string str = "";
            switch (conde)
            {
                case 0:
                    str = Encoding.GetEncoding("ISO-8859-1").GetString(b, 1, length);
                    break;
                case 1:
                    str = Encoding.GetEncoding("UTF-16LE").GetString(b, 1, length);
                    break;
                case 2:
                    str = Encoding.GetEncoding("UTF-16BE").GetString(b, 1, length);
                    break;
                case 3:
                    str = Encoding.UTF8.GetString(b, 1, length);
                    break;
            }
            return str;
        }
      
        }
    }

