using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Share;

namespace DateLife.Data
{
    public class CustomerInfo:ObservableObject
    {
        
        private string _Code;
        /// <summary>
        /// 机器码 属性更改通知
        /// </summary>
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        private string _Name;
        /// <summary>
        /// 客户名 属性更改通知
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private RegisterInfo _CurrentRegister;
        /// <summary>
        /// 当前注册信息 属性更改通知
        /// </summary>
        public RegisterInfo CurrentRegister
        {
            get
            {
                return _CurrentRegister;
            }
            set
            {
                _CurrentRegister = value;
                RaisePropertyChanged(() => CurrentRegister);
            }
        }


        private ObservableCollection<RegisterInfo> _Registers;
        /// <summary>
        /// MyProperty 属性更改通知
        /// </summary>
        public ObservableCollection<RegisterInfo> Registers
        {
            get
            {
                return _Registers;
            }
            set
            {
                _Registers = value;
                RaisePropertyChanged(() => Registers);
            }
        }


    }

    public class RegisterInfo: ObservableObject
    {

        private string _EndDate;
        /// <summary>
        /// 到期时间 属性更改通知
        /// </summary>
        public string EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
                RaisePropertyChanged(() => EndDate);
            }
        }

        private string _StartDate;
        /// <summary>
        /// 开始时间 属性更改通知
        /// </summary>
        public string StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
                RaisePropertyChanged(() => StartDate);
            }
        }


        private int _DayCount;
        /// <summary>
        /// DayCount 属性更改通知
        /// </summary>
        public int DayCount
        {
            get
            {
                return _DayCount;
            }
            set
            {
                _DayCount = value;
                EndDate = System.Convert.ToDateTime(_StartDate).AddDays(_DayCount).ToString("yyyy-MM-dd HH:mm:ss");
                RaisePropertyChanged(() => DayCount);
            }
        }

        
    }
}
