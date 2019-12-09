using DateLife.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.Win32;
using Newtonsoft.Json;
using Share;
using Share.Date;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace DateLife.ViewModel
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
            Init();

        }

        #region ����

        private ObservableCollection<CustomerInfo> _CustomerInfos;
        /// <summary>
        /// MyProperty ���Ը���֪ͨ
        /// </summary>
        public ObservableCollection<CustomerInfo> CustomerInfos
        {
            get
            {
                return _CustomerInfos;
            }
            set
            {
                _CustomerInfos = value;
                RaisePropertyChanged(() => CustomerInfos);
            }
        }


        private CustomerInfo _selectItem;
        /// <summary>
        /// SelectItem ���Ը���֪ͨ
        /// </summary>
        public CustomerInfo SelectItem
        {
            get
            {
                return _selectItem;
            }
            set
            {
                _selectItem = value;
                RaisePropertyChanged(() => SelectItem);
            }
        }

        
        private CustomerInfo _EditInfo;
        /// <summary>
        /// ���ڱ༭����Ϣ ���Ը���֪ͨ
        /// </summary>
        public CustomerInfo EditInfo
        {
            get
            {
                return _EditInfo;
            }
            set
            {
                _EditInfo = value;
                RaisePropertyChanged(() => EditInfo);
            }
        }



        #endregion

        #region ����

        public RelayCommand CreateCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(Create)).Value;
        public RelayCommand ChooseCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(Choose)).Value;
        /// <summary>
        /// ѡ��lz�ļ�
        /// </summary>
        private void Choose()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "�����ļ�(*.zl)|*.zl";
            openFile.InitialDirectory = "D:";
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    //����ģ��
                    Load(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// ���� 
        /// </summary>
        private void Create()
        {
            if (EditInfo.Name == "" || EditInfo.Name ==null)
            {
                ShowMsg("�û�����Ϊnull");
                return;
            }

            //if (EditInfo.CurrentRegister.DayCount == 0)
            //{
            //    ShowMsg("ʹ����������Ϊ0");
            //    return;
            //}

            try
            {
                var temp = CustomerInfos.Where(x => x.Code == EditInfo.Code).First();
                temp.Registers.Add(EditInfo.CurrentRegister);
                temp.CurrentRegister = EditInfo.CurrentRegister;
            }
            catch (Exception ex)
            {
                CustomerInfos.Add(EditInfo);
            }

            Save(Path);
            //��¼cpu�͵�������
            string tempE = $"{EditInfo.Code},{EditInfo.CurrentRegister.EndDate},{EditInfo.CurrentRegister.StartDate}";
            //����
            tempE = Encryption.DesEncrypt(tempE, TimeClass.EncryKey);
            //File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}RegistrationCode.zlzc", tempE);
            string path = $@"D:\RegistrationCode.zlzc";
            File.WriteAllText(path, tempE);
            ShowMsg($"���ɳɹ���{path}");
        }



        #endregion

        #region ����
        string Path = $"{AppDomain.CurrentDomain.BaseDirectory}AppConfig.json";
        /// <summary>
        /// ����
        /// </summary>
        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(CustomerInfos);
            json = json.FormatJsonStr();
            File.WriteAllText(path, json);
            
        }
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="file"></param>
        private void Load(string file)
        {
            //var json = File.ReadAllText(file).JsonStrToObj<>();

            var lzStr = File.ReadAllText(file);
            lzStr = Encryption.DesDecrypt(lzStr, TimeClass.EncryKey);
            string[] vs = lzStr.Split(',');

            //��ȡcpu��ʱ��
            string cpuId = vs[0];

            string date = vs[1];

            CustomerInfo customerInfo = new CustomerInfo();
            customerInfo.Code = cpuId;
            customerInfo.Registers = new ObservableCollection<RegisterInfo>();

            RegisterInfo registerInfo = new RegisterInfo();
            registerInfo.StartDate = date;
            registerInfo.EndDate = date;

            customerInfo.Registers.Add(registerInfo);
            customerInfo.CurrentRegister = registerInfo;
            EditInfo = customerInfo;


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

        /// <summary>
        /// ��ʼ��
        /// </summary>
        private void Init()
        {
            ObservableCollection<CustomerInfo> customerInfos;

            
            if (File.Exists(Path))
            {
                try
                {
                    var json = File.ReadAllText(Path);
                    customerInfos = JsonConvert.DeserializeObject<ObservableCollection<CustomerInfo>>(json);
                }
                catch
                {
                    customerInfos = new ObservableCollection<CustomerInfo>();
                }
            }
            else
            {
                customerInfos = new ObservableCollection<CustomerInfo>();
            }

            CustomerInfos = customerInfos;
            //ѡ�е�һ��
            if (customerInfos.Count > 0)
            {
                SelectItem = customerInfos[0];
            }
        }
        #endregion

    }
}