using NeuroMat_Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroMat_Application.Model
{
    public class NCVTestResults : NotifyProperyChangedBase
    {
        private string _name;
        private string _ncvValue;

        public NCVTestResults()
        {

        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string NcvValue
        {
            get { return _ncvValue; }
            set
            {
                _ncvValue = value;
                OnPropertyChanged();
            }
        }
    }
}
