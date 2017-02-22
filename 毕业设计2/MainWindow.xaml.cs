using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Control;
using System.IO;
using ViewModel;
using Model;
using System.Windows.Interop;
using System.Collections.ObjectModel;
using Un4seen.Bass;
using System.ComponentModel;
using System.Threading;
using 毕业设计2;

namespace 毕业设计2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker bgWork = new BackgroundWorker();
        IntPtr handle;
        private MianViewModel vm;
        public bool ListViewclick = false;
        public bool ListViewChanged = false;
        public bool UpDownClick = false;
        public bool ShowMusic = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {

            //该值指示BackgroundWorker能否报告进度更新
            bgWork.WorkerReportsProgress = true;
            //是否支持异步取消
            bgWork.WorkerSupportsCancellation = true;
            bgWork.DoWork += ThreadPaly;
            bgWork.ProgressChanged += ProgessSchedule;
            vm = new MianViewModel();
            vm.Peoples = new ObservableCollection<People>();
            for (int i = 0; i < 10; i++)
            {
                vm.Peoples.Add(new People { Name = "我是第" + i + "个对象", Age = i * 1000 });
            }
            vm.i = 60;
            vm.QueryXml();
            //this.ListCountry.DataContext = Country;
            this.DataContext = vm;

            InitializeComponent();

        }


        /// <summary>
        /// 进度条更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgessSchedule(object sender, ProgressChangedEventArgs e)
        {
            slider.Value = e.ProgressPercentage;
            StringBuilder sb = new StringBuilder();
          
            if (slider.Value < 60)
            {
                vm.Time = "0:" + slider.Value;

            }
            else
            {
                vm.Time = (int)(slider.Value) / 60 + ":" +(int)(slider.Value) % 60;
            }
        }

        /// <summary>
        /// 后台进行播放，不卡死UI线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThreadPaly(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Bass.BASS_ChannelStop(mc.Stream);
            Bass.BASS_StreamFree(mc.Stream);


            if (vm.SelectItemMusic != null && ListViewclick == true)
            {

                //MessageBox.Show("OK");
                //获得当前选中的item的索引
                int index = vm.MusicInfos.IndexOf(vm.SelectItemMusic);
                //默认为循环播放
                //这个while是为了保持当前线程不被销毁所创建的同时也是为了不让vm.SelectItemMusic干扰我们当前播放的歌曲
                //vm.SelectItemMusic是绑定的，只要listView的选择改变就改变把当前的索引束缚在这个while里面
                while (true)
                {
                    while (true)
                    {
                        //说明当前ListViewclick一点击结束
                        ListViewclick = false;
                        byte[] image = vm.CreateImage(vm.MusicInfos[index].ID);
                        vm.MusicInfos[index].MusicPicture = image;
                        mc.MusicPlay(vm.MusicInfos[index].MusicFilePath, handle);

                        vm.MusicPlaying = vm.MusicInfos[index];
                   
                        //vm.MusicPlaying.MusicPicture = image;
                        //得到歌曲的持续时间
                        mc.GetTotalTime(mc.Stream);
                        vm.MusicTotalTime = mc.TotalTime;
                       
                        int totalTime = mc.TotalTime;
                        int remainingtime = 1;
                        while (true)
                        {
                            //当前歌曲的进度
                            int currentTime = mc.GetCurrentTime(mc.Stream);
                            //剩余时间
                            remainingtime = totalTime - currentTime;
                            Thread.Sleep(500);
                            if (currentTime == 0 && mc.Stream == 0)
                            {
                                Bass.BASS_StreamFree(mc.Stream);
                                Bass.BASS_ChannelStop(mc.Stream);
                                //MessageBox.Show("currentTime=0");

                            }
                            //当前的索引不等于当前选择的vm.SelectItemMusic的索引就跳出当前循环，重现播放当前选择的歌曲
                            if (index != vm.MusicInfos.IndexOf(vm.SelectItemMusic) && ListViewclick == true)
                            {
                                Bass.BASS_StreamFree(mc.Stream);
                                Bass.BASS_ChannelStop(mc.Stream);
                                index = -1;
                                //MessageBox.Show(ListViewclick.ToString() + "---" + "跳出了获取当前歌曲进度的循环");
                                break;
                            }
                            //如果取消了后台线程就跳出循环
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                //Bass.BASS_StreamFree(mc.Stream);
                                //Bass.BASS_ChannelStop(mc.Stream);
                                break;
                            }
                            //向slider传递当前歌曲的进度
                            worker.ReportProgress(currentTime);

                            //remainingtime == 0 就代表歌曲播放完毕跳出循环
                            if (remainingtime == 0)
                            {
                                worker.ReportProgress(0);
                                break;
                            }

                            if (vm.MusicPlaying != vm.SelectItemMusic && UpDownClick == true)
                            {
                                //上一曲判断
                                //我们的歌曲使使用vm.SelectItemMusic的索引进行播放的
                                //我们点击上一曲按钮的时候，索引改变，但是当前的播放歌曲的对象不等于当前vm.SelectItemMusic对象
                                //要跳出当前的循环
                                //index = vm.MusicInfos.IndexOf(vm.SelectItemMusic);
                                //mc.FreeStream(mc.Stream);
                                //MessageBox.Show("我判断上一曲模块的--我要跳出了当前循环"+"----"+ ListViewChanged.ToString());

                                break;
                            }

                            ////监测当前歌曲播放状态
                            //if (mc.playstate == MusicControl.PlayState.Pause)
                            //{
                            //    mc.MusicPlay(mc.Stream);

                            //}


                        }//第三个while
                        if (vm.MusicPlaying != vm.SelectItemMusic && UpDownClick == true)
                        {
                            //上一曲判断
                            //我们的歌曲使使用vm.SelectItemMusic的索引进行播放的
                            //我们点击上一曲按钮的时候，索引改变，但是当前的播放歌曲的对象不等于当前vm.SelectItemMusic对象
                            //要跳出当前的循环
                            index = vm.MusicInfos.IndexOf(vm.SelectItemMusic);
                            mc.FreeStream(mc.Stream);
                            //MessageBox.Show("我是判断上一曲的，我在第二个while里面---跳出了循环" + "----" + ListViewChanged.ToString());
                            worker.ReportProgress(0);
                            UpDownClick = false;
                            break;
                        }


                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            Bass.BASS_StreamFree(mc.Stream);
                            Bass.BASS_ChannelStop(mc.Stream);
                            worker.ReportProgress(0);
                            return;
                        }
                        //index == -1代表用户有重现选择了一个item跳出循环销毁后台线程，
                        if (index == -1 && ListViewclick == true)
                        {
                            worker.ReportProgress(0);
                            MessageBox.Show("ListViewclick=" + ListViewclick.ToString() + "--" + "跳出了当前播放歌曲的循环");
                            index = vm.MusicInfos.IndexOf(vm.SelectItemMusic);
                            break;
                        }
                        else
                        {
                            index++;
                            if (index == vm.MusicInfos.Count)
                            {
                                index = 0;
                            }
                            vm.SelectItemMusic = vm.MusicInfos[index];
                            mc.FreeStream(mc.Stream);

                        }

                    }




                    //      }
                    //    }
                    //    else
                    //    {
                    //        //mc.MusicPlay(mc.Stream);
                    //    }


                    ////当前用户双击了ListView要播放歌曲
                    ////要保持后台线程持续的运行，进行while循环
                    //while (true)
                    //{
                    //    //判断当前的文件流是否！=0
                    //    if (mc.Stream != 0)
                    //    {
                    //        //释放文件流
                    //        Bass.BASS_ChannelStop(mc.Stream);
                    //        Bass.BASS_StreamFree(mc.Stream);
                    //    }
                    //    else
                    //    {
                    //        //等于0就说明要重新播放歌曲
                    //        BackgroundWorker work = sender as BackgroundWorker;
                    //    }
                    //}

                }
            }
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            handle = wndHelper.Handle;
            BassNet.Registration("546307885@qq.com", "2X34243714232222");
            if ((Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_CPSPEAKERS, handle)))
            {
                MessageBox.Show("加载成功");
            }
            vm.QueryNativeMusicXML();
            lisBox.ItemsSource = vm.NativeMusic;
           sliderVomule.AddHandler(Slider.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonDown), true);
        }

        private void Slider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(sliderVomule.Value.ToString());
        }

        /// <summary>
        /// 使窗体可以移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClose(object sender, RoutedEventArgs e)
        {
            this.Close();
            Bass.BASS_Stop();
            Bass.BASS_Free();

        }
        /// <summary>
        /// 全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowMaxSize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// 窗体最小化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowSmall(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// listbox点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoutryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.vm.SlectecItem != null)
            {
                CountryBorder.Visibility = Visibility.Hidden;
                MusicInfo.Visibility = Visibility.Visible;
                vm.GetMusicInfos(vm.SlectecItem.Name);
                listView.ItemsSource = vm.MusicInfos;
                MusicCountry.Content = vm.SlectecItem.Name;
                MusicCount.Content = vm.MusicCount;
                MuiscCountryImag.Source = new BitmapImage(new Uri(vm.SlectecItem.CountryPath, UriKind.Relative));




            }
            else
            {
                MessageBox.Show("所选内容为空");
            }
        }

        MusicControl mc = new MusicControl();
        /// <summary>
        /// 选中listView中的音乐播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MusicPlay(object sender, MouseButtonEventArgs e)
        {
            ListViewclick = true;
            try
            {

                //执行后台播放
                if (!bgWork.IsBusy)
                {
                    bgWork.RunWorkerAsync();
                    if (vm.SelectItemMusic != null && ListViewclick == true)
                    {

                        DateTime dt = DateTime.Parse("00:"+vm.SelectItemMusic.MusicTime);
                        int sec = dt.Minute * 60 + dt.Second;
                        slider.Maximum = sec;
                        MessageBox.Show(sec.ToString());
                    }
                }
                else
                {
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());


            }


        }


        /// <summary>
        /// listView改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { //在ListView
          //Bass.BASS_ChannelStop(mc.Stream);
          //Bass.BASS_StreamFree(mc.Stream);

            if (vm.SelectItemMusic == vm.MusicPlaying)
            {
                //MessageBox.Show("我们相等");
                ListViewChanged = true;
            }
            else
            {
                ListViewChanged = false;
                //MessageBox.Show("我们不相等");
            }
            //int index = vm.MusicInfos.IndexOf(vm.SelectItemMusic);
            //MessageBox.Show(index.ToString());
        }

        /// <summary>
        /// 右边窗口返回事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack(object sender, RoutedEventArgs e)
        {
            if (CountryBorder.Visibility == Visibility.Hidden && MusicInfo.Visibility == Visibility.Visible)
            {
                CountryBorder.Visibility = Visibility.Visible;
                MusicInfo.Visibility = Visibility.Hidden;
            }
            //if (DownLoadBorder.Visibility == Visibility&&MusicInfo.Visibility==Visibility.Hidden)
            //{
            //    DownLoadBorder.Visibility = Visibility.Hidden;
            //    MusicInfo.Visibility = Visibility.Visible;
            //}
            if (DownLoadBorder.Visibility == Visibility.Visible && CountryBorder.Visibility == Visibility.Hidden)
            {
                DownLoadBorder.Visibility = Visibility.Hidden;
                CountryBorder.Visibility = Visibility.Visible;
            }
            if (borderMusicMian.Visibility == Visibility.Visible)
            {
                borderMusicMian.Visibility = Visibility.Hidden;
                CountryBorder.Visibility = Visibility.Visible;
                MusicInfo.Visibility = Visibility.Hidden;
            }

            if (borderMusicMian.Visibility == Visibility.Visible && CountryBorder.Visibility == Visibility.Visible)
            {
                CountryBorder.Visibility = Visibility.Hidden;
                CountryBorder.Visibility = Visibility.Visible;
                MusicInfo.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// 点击下载音乐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnShow(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int id = int.Parse(btn.Tag.ToString());
            MusicInfo info=  vm.GetMusicInfos(id);
            DownLoadMusic dm = new DownLoadMusic { Name = info.MusicName, FileSize = info.MusicFilePath,Author=info.MusicAuthor };
            var p = new Progress<int>();
             p.ProgressChanged += (s, n) => { dm.Percint = n;
                 dm.Schedule = n.ToString()+"%";
                 if (n == 100)
                 {
                     dm.Complete = "完成";
                 }
             };           
            vm.DownLoadMusic = new ObservableCollection<DownLoadMusic>(); 
            vm.DownLoadMusic.Add(dm);
            DownLoadView.ItemsSource = vm.DownLoadMusic;
            var task = Task.Run(() => mc.DownLoadMusic(p,info.MusicFilePath, @"C:\Users\PC-DELL\Desktop" + "\\" + "22.mp3"));
            try
            {
                await task;
                if (task.Exception == null)
                {
                    
                    MessageBox.Show("下载完成");
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 音乐暂停/播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PalyorPause(object sender, RoutedEventArgs e)
        {
            //在歌曲暂停的时候进行上一曲/下一曲，按钮会显示暂停状态


            Button btn = (Button)sender;
            if ((string)btn.Tag == "Pause" && Play.Visibility == Visibility.Hidden)
            {
                //音乐暂停
                //MessageBox.Show(btn.Name);
                if (mc.Stream != 0)
                {
                    Play.Visibility = Visibility.Visible;
                    Pause.Visibility = Visibility.Hidden;
                    mc.MusicPause(mc.Stream);
                    MessageBox.Show(mc.playstate.ToString());
                }

                //MessageBox.Show("隐藏了");

            }   //音乐播放
            else if ((string)btn.Tag == "Play" && Pause.Visibility == Visibility.Hidden)
            {
                MessageBox.Show(btn.Name);
                Play.Visibility = Visibility.Hidden;
                Pause.Visibility = Visibility.Visible;
                MessageBox.Show(mc.Stream.ToString());
                mc.MusicPlay(mc.Stream);
            }
            else
            {
                MessageBox.Show("错误");
            }

        }
        /// <summary>
        /// 上一曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpMusic(object sender, RoutedEventArgs e)
        {
            int index = -1;
            //判断当前是否在播放歌曲
            if (vm.MusicPlaying != null)
            {
                UpDownClick = true;
                //获得当前播放歌曲在数组中的位置
                if (vm.MusicPlaying.MusicCountry != null)
                {
                    index = vm.MusicInfos.IndexOf(vm.MusicPlaying);
                    index--;
                    if (index < 0)
                    {
                        index = vm.MusicInfos.Count;
                        index = index - 1;
                    }
                    //index--;
                    vm.SelectItemMusic = vm.MusicInfos[index];
                    //在歌曲暂停的时候进行上一曲/下一曲，按钮会显示暂停状态
                    if (Play.Visibility == Visibility.Visible)
                    {
                        Play.Visibility = Visibility.Hidden;
                        Pause.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    index = vm.NativeMusic.IndexOf(vm.MusicPlaying);
                    index--;
                    if (index < 0)
                    {
                        index = vm.NativeMusic.Count;
                        index = index - 1;
                    }
                    //index--;
                    vm.SelectItemNativeMusic = vm.NativeMusic[index];
                    //在歌曲暂停的时候进行上一曲/下一曲，按钮会显示暂停状态
                    if (Play.Visibility == Visibility.Visible)
                    {
                        Play.Visibility = Visibility.Hidden;
                        Pause.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        /// <summary>
        /// 下一曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextMusic(object sender, RoutedEventArgs e)
        {
            int index = -1;
            //判断当前是否在播放歌曲
            if (vm.MusicPlaying != null)
            {
                UpDownClick = true;
                //获得当前播放歌曲在数组中的位置
                if (vm.MusicPlaying.MusicCountry != null)
                {

                    index = vm.MusicInfos.IndexOf(vm.MusicPlaying);
                    index++;
                    if (index == vm.MusicInfos.Count)
                    {
                        index = 0;
                    }
                    //index++;
                    vm.SelectItemMusic = vm.MusicInfos[index];
                    //在歌曲暂停的时候进行上一曲/下一曲，按钮会显示暂停状态
                    if (Play.Visibility == Visibility.Visible)
                    {
                        Play.Visibility = Visibility.Hidden;
                        Pause.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    index = vm.NativeMusic.IndexOf(vm.MusicPlaying);
                    index++;
                    if (index == vm.NativeMusic.Count)
                    {
                        index = 0;
                    }
                    //index++;
                    vm.SelectItemNativeMusic = vm.NativeMusic[index];
                    //在歌曲暂停的时候进行上一曲/下一曲，按钮会显示暂停状态
                    if (Play.Visibility == Visibility.Visible)
                    {
                        Play.Visibility = Visibility.Hidden;
                        Pause.Visibility = Visibility.Visible;
                    }
                }
            }
        }

      
        /// <summary>
        /// 打开文件选择对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileDiaLog(object sender, RoutedEventArgs e)
        {
            vm.GetNativeMusic();
            if (vm.NativeMusic != null)
            {
                MessageBox.Show(vm.NativeMusic.Count.ToString());
                vm.QueryNativeMusicXML();
                lisBox.ItemsSource = vm.NativeMusic;
            }
        }

        /// <summary>
        /// 本地音乐点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayNativeMusic(object sender, MouseButtonEventArgs e)
        {
            bgWork.CancelAsync();
            if (mc.Stream != 0)
            {
                mc.FreeStream(mc.Stream);
            }
            mc.CreateStream(vm.SelectItemNativeMusic.MusicFilePath);
            vm.MusicPlaying = vm.SelectItemNativeMusic;
       
            mc.GetTotalTime(mc.Stream);
            vm.MusicTotalTime = mc.TotalTime;
            
            var pp = new Progress<int>();
            var task = Task.Run(() => MySilder(pp, 500));
            pp.ProgressChanged += (s, n) => { slider.Value = n;
                StringBuilder sb = new StringBuilder();
                if (slider.Value < 60)
                {
                    vm.Time = "0:" + slider.Value;

                }
                else
                {
                    vm.Time = (int)(slider.Value) / 60 + ":" + (int)(slider.Value) % 60;
                }
            };
          


            //可写一个异步的方法，去播放本地歌曲 

        }
        /// <summary>
        /// 本地音乐循环播放和更新进度条
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="delply"></param>
        public void MySilder(IProgress<int> progress, int delply)
        {
            int currentTime = 1;
            //if (vm.MusicPlaying.MusicCountry != null && bgWork.IsBusy == true)
            //{
            //    progress.Report(0);
            //    return;

            //}
            //else
            //{
            //    while (true)
            //    {
            //        if (vm.MusicPlaying.MusicCountry != null)
            //        {
            //            MessageBox.Show("我是本地音乐的线程，我跳出循环了,我在第一个while里面");
            //            progress.Report(0);
            //            break; ;
            //        }
            //        while (true)
            //        {
            //            //获得当前播放歌曲的索引
            //            int index = vm.NativeMusic.IndexOf(vm.MusicPlaying);
            //            //获得当前的歌曲的持续时间
            //            mc.GetTotalTime(mc.Stream);
            //            int toltalTime = mc.TotalTime;

            //            //代表当前歌曲播放完毕
            //            if (currentTime == toltalTime)
            //            {
            //                index++;
            //                mc.FreeStream(mc.Stream);
            //                vm.MusicPlaying = vm.NativeMusic[index];
            //                mc.CreateStream(vm.MusicPlaying.MusicFilePath);
            //            }
            //            if (index == vm.NativeMusic.Count)
            //            {
            //                index = 0;
            //                vm.MusicPlaying = vm.NativeMusic[index];
            //            }
            //            if (vm.MusicPlaying.MusicCountry != null)
            //            {
            //                MessageBox.Show("我是本地音乐的线程，我跳出循环了，我在第2个while里面");
            //                progress.Report(0);
            //                break;

            //            }
            //            if (vm.MusicPlaying.MusicCountry != null)
            //            {
            //                //通知当前歌曲的播放进度
            //                while (true)
            //                {
            //                    Thread.Sleep(delply);
            //                    currentTime = mc.GetCurrentTime(mc.Stream);
            //                    if (currentTime == mc.TotalTime)
            //                    {
            //                        progress.Report(0);
            //                        break;
            //                    }
            //                    if (vm.MusicPlaying.MusicCountry != null)
            //                    {
            //                        MessageBox.Show("我是本地音乐的线程，我跳出循环了，我在第3个while里面" + bgWork.IsBusy.ToString());
            //                        progress.Report(0);
            //                        break;

            //                    }

            //                    progress.Report(currentTime);
            //                }
            //            }


            //        }
            //    }
            //}

            //先判断当前vm.MusicPlaying.MusicCountry != null 和bgWork.IsBus是否在运行
            //在运行状态就没必要在这方法内，再去更新进度条
            if (bgWork.CancellationPending == true)
            {
                while (true)
                {
                    //获得当前播放歌曲的索引
                    int index = vm.NativeMusic.IndexOf(vm.MusicPlaying);
                    while (true)
                    {
                        Thread.Sleep(delply);
                        currentTime = mc.GetCurrentTime(mc.Stream);
                        if (currentTime == mc.TotalTime)
                        {
                            progress.Report(0);
                            break;
                        }
                        if (bgWork.IsBusy == true)
                        {
                            return;
                        }
                        if (vm.MusicPlaying != vm.SelectItemNativeMusic && UpDownClick == true)
                        {
                            break;
                        }
                            //if (vm.MusicPlaying.MusicCountry != null)
                            //{
                            //    MessageBox.Show("我是本地音乐的线程，我跳出循环了，我在第3个while里面" + bgWork.IsBusy.ToString());
                            //    progress.Report(0);
                            //    break;

                            //}

                            progress.Report(currentTime);

                    }
                    if (currentTime == mc.TotalTime)
                    {
                        mc.FreeStream(mc.Stream);
                        index++;
                        if (index == vm.NativeMusic.Count)
                        {
                            index = 0;
                        }
                        mc.CreateStream(vm.NativeMusic[index].MusicFilePath);
                        vm.MusicPlaying = vm.NativeMusic[index];
                       
                        mc.GetTotalTime(mc.Stream);
                        vm.MusicTotalTime = mc.TotalTime;
                    }
                    if (vm.MusicPlaying != vm.SelectItemNativeMusic && UpDownClick == true)
                    {
                        //上一曲判断
                        //我们的歌曲使使用vm.SelectItemMusic的索引进行播放的
                        //我们点击上一曲按钮的时候，索引改变，但是当前的播放歌曲的对象不等于当前vm.SelectItemMusic对象
                        //要跳出当前的循环
                        index = vm.NativeMusic.IndexOf(vm.SelectItemNativeMusic);
                        vm.MusicPlaying = vm.NativeMusic[index];

                        mc.FreeStream(mc.Stream);
                        //MessageBox.Show("我是判断上一曲的，我在第二个while里面---跳出了循环" + "----" + ListViewChanged.ToString());
                        progress.Report(0);
                        UpDownClick = false;
                        mc.CreateStream(vm.NativeMusic[index].MusicFilePath);

                        mc.GetTotalTime(mc.Stream);
                        vm.MusicTotalTime = mc.TotalTime;
                        //break;
                    }
                    if (bgWork.IsBusy == true)
                    {
                        return;
                    }
                }
          


            }
            else
            {

               return;
                //应该专门有一个方法异步的方法去获取当前的歌曲进度
            }

        }
        /// <summary>
        /// 打开下载列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDownLoad(object sender, RoutedEventArgs e)
        {
            if (CountryBorder.Visibility == Visibility.Visible)
            {
                CountryBorder.Visibility = Visibility.Hidden;
                DownLoadBorder.Visibility = Visibility.Visible;
            }
            if (MusicInfo.Visibility == Visibility.Visible)
            {

                MusicInfo.Visibility = Visibility.Hidden;
                DownLoadBorder.Visibility = Visibility.Visible;
            }
            //DownLoadBorder.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 展示音乐和歌词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MusicShow(object sender, RoutedEventArgs e)
        {
            ShowMusic = true;
            if (MusicInfo.Visibility == Visibility.Visible)
            {
            borderMusicMian.Visibility = Visibility.Visible;
                MusicInfo.Visibility = Visibility.Hidden;
            }
            if (CountryBorder.Visibility == Visibility.Visible)
            {
                CountryBorder.Visibility = Visibility.Hidden;
                borderMusicMian.Visibility = Visibility.Visible;
            }
          
        }

        private void sliderVomule_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    mc.SetVolume(sliderVomule.Value);

            //}
          
        }

        private void sliderVomule_MouseMove(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mc.Volume =(int) sliderVomule.Value;
        }

        private void sliderVomule_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(sliderVomule.Value.ToString());
        }

        private void sliderVomule_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(sliderVomule.Value.ToString());
        }

        private void slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double value = slider.Value;
            mc.MusicPause(mc.Stream);
            //if (mc.Stream != 0)
            //{
               
            //    mc.SetMusicPosition(mc.Stream, (long)value);
            //    mc.MusicPlay(mc.Stream);
            //}
        }
        /// <summary>
        /// 关闭音乐展示窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_borderMusicMain(object sender, RoutedEventArgs e)
        {
            borderMusicMian.Visibility = Visibility.Hidden;
           
        }
        /// <summary>
        /// 国家列表点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoutryList_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if (this.vm.SlectecItem != null)
            {
                CountryBorder.Visibility = Visibility.Hidden;
                MusicInfo.Visibility = Visibility.Visible;
                vm.GetMusicInfos(vm.SlectecItem.Name);
                listView.ItemsSource = vm.MusicInfos;
                MusicCountry.Content = vm.SlectecItem.Name;
                MusicCount.Content = vm.MusicCount;
                MuiscCountryImag.Source = new BitmapImage(new Uri(vm.SlectecItem.CountryPath, UriKind.Relative));




            }
            else
            {
                MessageBox.Show("所选内容为空");
            }
        }
    }
}