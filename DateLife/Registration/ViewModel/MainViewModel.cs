using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.Win32;
using Share.Date;
using System;
using System.IO;
using System.Windows;

namespace Registration.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private string _Path;
        /// <summary>
        /// Path 属性更改通知
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public RelayCommand GetInfoCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(GetInfo)).Value;

        private void GetInfo()
        {
            string tmplFolder = "";
            OpenFileDialog SFDialog = new OpenFileDialog();
            SFDialog.Filter = "模板文件 (*.zlzc)|*.zlzc";
            SFDialog.InitialDirectory = @"D:\";
            if (SFDialog.ShowDialog() == true)
            {
                Path = SFDialog.FileName;
            }
            else
                return;
                    
            var lzStr = File.ReadAllText(Path);
            lzStr = Encryption.DesDecrypt(lzStr, TimeClass.EncryKey);
            string[] vs = lzStr.Split(',');

            //获取cpu和到期时间
            string cpuId = vs[0];

            string date = vs[1];
            string Startdate = vs[2];

            string CpuIdThis = TimeClass.GetCpuId();
            //比较cup
            if (cpuId != CpuIdThis)
            {
                ShowMsg("该注册码不属于该机器，请检查机器码！！");
                return;
            }

            /* 比较时间 */
            string NowDate = TimeClass.GetNowDate();
            if (DateTime.Compare(Convert.ToDateTime(Startdate), Convert.ToDateTime(NowDate)) > 0)
            {
                ShowMsg("系统时间已做修改，请重新发起注册流程！！");
                return;
            }
            else
            {
                Startdate = NowDate;
                string temp = $"{cpuId},{date},{Startdate}";
                TimeClass.Add(temp);

                int res = TimeClass.InitRegedit();
                if (res == 0)
                {
                    ShowMsg("成功注册！！");
                }
                //else if (res == 1)
                //{
                //    MessageBox.Show("软件尚未注册，请注册软件！");
                //}
                //else if (res == 2)
                //{
                //    MessageBox.Show("注册机器与本机不一致,请联系管理员！");
                //}
                //else if (res == 3)
                //{
                //    MessageBox.Show("软件试用已到期！");
                //}
                else
                {
                    ShowMsg("注册失败！！返回值：" + res);
                }


                

            }
        }

        private void ShowMsg(string msg)
        {
            HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
            {
                MessageBoxText = msg,
                Caption = "提示",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.AskGeometry,
                Style = ResourceHelper.GetResource<Style>("MessageBoxCustom")
            });
        }
    }
}