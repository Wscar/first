using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Un4seen.Bass;
using System.Net;
namespace Control
{
   public class MusicControl
    {

        BackgroundWorker bgWorker = new BackgroundWorker();
        public MusicControl()
        {
            bgWorker.DoWork += ThreadPlay;
         
        }
        public int CurrentTime
        {
            get;
            set;
        }
        public int TotalTime {
            get;
            set;
        }

        public enum PlayState
        {   /// <summary>
        /// 正在播放
        /// </summary>
            Paly=BASSActive.BASS_ACTIVE_PLAYING,
            /// <summary>
            /// 暂停
            /// </summary>
            Pause=BASSActive.BASS_ACTIVE_PAUSED,
            /// <summary>
            /// 停止
            /// </summary>
            Stop=BASSActive.BASS_ACTIVE_STOPPED,
            /// <summary>
            /// 延迟
            /// </summary>
            Stalled = BASSActive.BASS_ACTIVE_STALLED,
        }
        /// <summary>
        /// 音频流句柄
        /// </summary>
        public int Stream { get; set; }
        /// <summary>
        /// 音乐播放获取FTP服务器的音乐文件路径
        /// </summary>
        /// <param name="FtpFilePath">ftp服务器的音乐路径</param>
        /// <param name="handle">窗体的句柄</param>
        public  void MusicPlay(string FtpFilePath, IntPtr handle)
        {
            //Bass.BASS_Stop();
            //Bass.BASS_Free();
          
           this.Stream= Bass.BASS_StreamCreateURL("ftp://120.55.169.75/"+FtpFilePath, 0, BASSFlag.BASS_DEFAULT, null, handle);
            if (Stream == 0)
            {
                MessageBox.Show("系统错误无法播放");
            }
            else
            {
                Bass.BASS_ChannelPlay(Stream, false);
            }
        }
        /// <summary>
        /// 用来创建本地音乐的文件流
        /// </summary>
        /// <param name="filepath"></param>
        public  void CreateStream(string filepath)
        {
       this.Stream=  Bass.BASS_StreamCreateFile(filepath, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
            Bass.BASS_ChannelPlay(this.Stream, false);
        }
        public void MusicPlay(int stream)
        {
            if (stream != 0 && playstate == PlayState.Pause)
            {
                Bass.BASS_ChannelPlay(stream,false);
            }
        }
        public void ThreadPlay(object sender, DoWorkEventArgs s)
        {
       
        }
        /// <summary>
        /// 音乐暂停
        /// </summary>
        /// <param name="stream">音频流</param>
        public void MusicPause(int stream)
        {
            if (stream != 0)
            {
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Bass.BASS_ChannelPause(stream);
                }
            }
        }
        PlayState state;
        public PlayState playstate
        {
            get {
                switch (Bass.BASS_ChannelIsActive(this.Stream))
                {
                    case BASSActive.BASS_ACTIVE_PAUSED:
                        state = PlayState.Pause;
                        break;
                    case BASSActive.BASS_ACTIVE_PLAYING:
                        state = PlayState.Paly;
                        break;
                    case BASSActive.BASS_ACTIVE_STOPPED:
                        state = PlayState.Stop;
                        break;
                    case BASSActive.BASS_ACTIVE_STALLED:
                        state = PlayState.Stalled;
                        break;

                }
                return state;
            }
        }
        /// <summary>
        /// 获得当前歌曲的播放位置
        /// </summary>
        /// <param name="stream">音频流</param>
        /// <returns></returns>
        public long MusicNowPosition( int stream)
        {
          long position = Bass.BASS_ChannelGetPosition(stream);
            return position;
        }
        /// <summary>
        /// 设置音乐的播放位置
        /// </summary>
        /// <param name="stream">音频流</param>
        /// <param name="position">音乐的播放位置</param>
        public void SetMusicPosition(int stream,long position)
        {
            Bass.BASS_ChannelSetPosition(stream, position);
        }
        /// <summary>
        /// 获得当前歌曲总持续时间
        /// </summary>
        /// <param name="stream"></param>
        public void GetTotalTime(int stream)
        {
           long len=  Bass.BASS_ChannelGetLength(stream);
            double TotalTime = Bass.BASS_ChannelBytes2Seconds(stream, len);
            this.TotalTime = (int)TotalTime;
        }
        /// <summary>
        /// 获得当前的音乐时间（进度条的数据）
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public int GetCurrentTime(int stream)
        {
            long pos = Bass.BASS_ChannelGetPosition(stream);
            int currentTime = (int)Bass.BASS_ChannelBytes2Seconds(stream, pos);
            this.CurrentTime = currentTime;
            return currentTime;

        }
        /// <summary>
        /// 释放文件流
        /// </summary>
        /// <param name="stream"></param>
        public void FreeStream(int stream)
        {
            Bass.BASS_ChannelStop(stream);
            Bass.BASS_StreamFree(stream);
        }
        public void DownLoadMusic(IProgress<int> progress, string filePath,string NativePath)
        {
            FileStream fs = new FileStream(NativePath, FileMode.Create, FileAccess.Write);
            try
            {
                long len = GetFileLength(filePath);
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create("ftp://120.55.169.75//" + filePath);
                ftp.KeepAlive = false;
                ftp.UseBinary = true;
                ftp.Credentials = new NetworkCredential();
                ftp.Method = WebRequestMethods.Ftp.DownloadFile; 
                FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
                 
                Stream strm = response.GetResponseStream();

                
                //strm.Close();
                //response.Close();
                //ftp.Method = WebRequestMethods.Ftp.DownloadFile;
                // FtpWebResponse downLoadResponse = (FtpWebResponse)ftp.GetResponse();
                //Stream dowmLoadStrm = downLoadResponse.GetResponseStream();
                byte[] buffer = new byte[4096*10];
                int readCount;
                long writeByte = 0;
                readCount = strm.Read(buffer, 0, buffer.Length);
                while (readCount != 0)
                {
                    writeByte += readCount;
                    fs.Write(buffer, 0, readCount);
                    readCount = strm.Read(buffer, 0, buffer.Length);
                    int percint =(int) ((writeByte * 100) / len);
                    progress.Report(percint);
                }
                fs.Close();
                strm.Close();
                response.Close();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }
        }
        public long GetFileLength(string filePath)
        {
            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create("ftp://120.55.169.75//" + filePath);
            ftp.KeepAlive = false;
            ftp.UseBinary = true;
            ftp.Credentials = new NetworkCredential();
            ftp.Method = WebRequestMethods.Ftp.GetFileSize;
            FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();

            Stream strm = response.GetResponseStream();

            long len = response.ContentLength;
            strm.Close();
            response.Close();
            return len;
        }
    }
}
