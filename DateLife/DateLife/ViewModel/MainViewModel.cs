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

        #region 属性

        private ObservableCollection<CustomerInfo> _CustomerInfos;
        /// <summary>
        /// MyProperty 属性更改通知
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
        /// SelectItem 属性更改通知
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
        /// 正在编辑的信息 属性更改通知
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

        #region 命令

        public RelayCommand CreateCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(Create)).Value;
        public RelayCommand ChooseCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(Choose)).Value;
        /// <summary>
        /// 选择lz文件
        /// </summary>
        private void Choose()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "加密文件(*.zl)|*.zl";
            openFile.InitialDirectory = "D:";
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    //加载模板
                    Load(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// 生成 
        /// </summary>
        private void Create()
        {
            if (EditInfo.Name == "" || EditInfo.Name ==null)
            {
                ShowMsg("用户不能为null");
                return;
            }

            //if (EditInfo.CurrentRegister.DayCount == 0)
            //{
            //    ShowMsg("使用天数不能为0");
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
            //记录cpu和到期日期
            string tempE = $"{EditInfo.Code},{EditInfo.CurrentRegister.EndDate},{EditInfo.CurrentRegister.StartDate}";
            //加密
            tempE = Encryption.DesEncrypt(tempE, TimeClass.EncryKey);
            //File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}RegistrationCode.zlzc", tempE);
            string path = $@"D:\RegistrationCode.zlzc";
            File.WriteAllText(path, tempE);
            ShowMsg($"生成成功，{path}");
        }



        #endregion

        #region 方法
        string Path = $"{AppDomain.CurrentDomain.BaseDirectory}AppConfig.json";
        /// <summary>
        /// 保存
        /// </summary>
        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(CustomerInfos);
            json = json.FormatJsonStr();
            File.WriteAllText(path, json);
            
        }
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="file"></param>
        private void Load(string file)
        {
            //var json = File.ReadAllText(file).JsonStrToObj<>();

            var lzStr = File.ReadAllText(file);
            lzStr = Encryption.DesDecrypt(lzStr, TimeClass.EncryKey);
            string[] vs = lzStr.Split(',');

            //获取cpu和时间
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
                Caption = "提示",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.AskGeometry,
                Style = ResourceHelper.GetResource<Style>("MessageBoxCustom")
            });
        }

        /// <summary>
        /// 初始化
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
            //选中第一个
            if (customerInfos.Count > 0)
            {
                SelectItem = customerInfos[0];
            }
        }
        #endregion

    }
}