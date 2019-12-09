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
        /// Path ���Ը���֪ͨ
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
            SFDialog.Filter = "ģ���ļ� (*.zl)|*.zl";
            SFDialog.InitialDirectory = @"D:\";
            if (SFDialog.ShowDialog() == true)
            {
                Path = SFDialog.FileName;
            }
            else
                return;
            //��ȡcpu��ʱ��
            string cpuId = TimeClass.GetCpuId();

            string date = TimeClass.GetNowDate();

            string temp = $"{cpuId},{date}";
            //����
            temp = Encryption.DesEncrypt(temp, TimeClass.EncryKey);


            //д���ļ�
            using (StreamWriter sw = new StreamWriter(Path))
            {
                sw.Write(temp);//ֱ��׷���ļ�ĩβ��������
            }

            //MessageBox.Show("��Ϣ�ļ����ɳɹ�", "��ʾ");

            HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
            {
                MessageBoxText = "��Ϣ�ļ����ɳɹ�",
                Caption = "��ʾ",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.AskGeometry,
                Style = ResourceHelper.GetResource<Style>("MessageBoxCustom")
            });
        }
    }
}