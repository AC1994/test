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
            string tmplFolder = "";
            OpenFileDialog SFDialog = new OpenFileDialog();
            SFDialog.Filter = "ģ���ļ� (*.zlzc)|*.zlzc";
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

            //��ȡcpu�͵���ʱ��
            string cpuId = vs[0];

            string date = vs[1];
            string Startdate = vs[2];

            string CpuIdThis = TimeClass.GetCpuId();
            //�Ƚ�cup
            if (cpuId != CpuIdThis)
            {
                ShowMsg("��ע���벻���ڸû�������������룡��");
                return;
            }

            /* �Ƚ�ʱ�� */
            string NowDate = TimeClass.GetNowDate();
            if (DateTime.Compare(Convert.ToDateTime(Startdate), Convert.ToDateTime(NowDate)) > 0)
            {
                ShowMsg("ϵͳʱ�������޸ģ������·���ע�����̣���");
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
                    ShowMsg("�ɹ�ע�ᣡ��");
                }
                //else if (res == 1)
                //{
                //    MessageBox.Show("�����δע�ᣬ��ע�������");
                //}
                //else if (res == 2)
                //{
                //    MessageBox.Show("ע������뱾����һ��,����ϵ����Ա��");
                //}
                //else if (res == 3)
                //{
                //    MessageBox.Show("��������ѵ��ڣ�");
                //}
                else
                {
                    ShowMsg("ע��ʧ�ܣ�������ֵ��" + res);
                }


                

            }
        }

        private void ShowMsg(string msg)
        {
            HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
            {
                MessageBoxText = msg,
                Caption = "��ʾ",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.AskGeometry,
                Style = ResourceHelper.GetResource<Style>("MessageBoxCustom")
            });
        }
    }
}