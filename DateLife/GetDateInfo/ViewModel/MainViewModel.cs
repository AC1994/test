using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.Win32;
using Share.Date;
using System;
using System.IO;
using System.Windows;

namespace GetDateInfo.ViewModel
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
            string  tmplFolder = "";
            SaveFileDialog SFDialog = new SaveFileDialog();
            SFDialog.Filter = "模板文件 (*.zl)|*.zl";
            SFDialog.InitialDirectory = @"D:\";
            if (SFDialog.ShowDialog() == true)
            {
                Path = SFDialog.FileName;
            }
            else
                return;
            //获取cpu和时间
            string cpuId = TimeClass.GetCpuId();

            string date = TimeClass.GetNowDate();

            string temp = $"{cpuId},{date}";
            //加密
            temp = Encryption.DesEncrypt(temp, TimeClass.EncryKey);


            //写入文件
            using (StreamWriter sw = new StreamWriter(Path))
            {
                sw.Write(temp);//直接追加文件末尾，不换行
            }

            //MessageBox.Show("信息文件生成成功", "提示");

            HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
            {
                MessageBoxText = "信息文件生成成功",
                Caption = "提示",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.AskGeometry,
                Style = ResourceHelper.GetResource<Style>("MessageBoxCustom")
            });
        }
    }
}