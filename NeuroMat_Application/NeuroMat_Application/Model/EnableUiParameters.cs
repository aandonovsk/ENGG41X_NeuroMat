using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroMat_Application;
using NeuroMat_Application.ViewModel;

namespace NeuroMat_Application.Model
{
    public class EnableUiParameters : NotifyProperyChangedBase
    {
        private bool _isRealTimeSensingEnabled;
        public bool IsRealTimeSensingEnabled
        {
            get
            {
                return _isRealTimeSensingEnabled;
            }
            set
            {
                _isRealTimeSensingEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _statusUpdate;
        public string StatusUpdate
        {
            get
            {
                return _statusUpdate;
            }
            set
            {
                _statusUpdate = value;
                OnPropertyChanged();
            }
        }

        private string _leftNCVTestResults = "0";
        public string LeftNCVTestResults
        {
            get
            {
                return _leftNCVTestResults;
            }
            set
            {
                _leftNCVTestResults = value;
                OnPropertyChanged();
            }
        }

        private string _rightNCVTestResults = "0";
        public string RightNCVTestResults
        {
            get
            {
                return _rightNCVTestResults;
            }
            set
            {
                _rightNCVTestResults = value;
                OnPropertyChanged();
            }
        }
    }
}
