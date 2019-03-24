using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroMat_Application.ViewModel;

namespace NeuroMat_Application.Model
{
    public partial class PressureSensorRowDataTemplate : NotifyProperyChangedBase
    {

        private IList<double> _pressureSensorData;
        private string _selectedRow;


        public PressureSensorRowDataTemplate()
        {
            _pressureSensorData = new List<double>();
            for (int i = 0; i < 8; i++)
            {
                _pressureSensorData.Add(0);
            }
        }

        public IList<double> PressureSensorData
        {
            get { return _pressureSensorData; }
            set
            {
                _pressureSensorData = value;
                OnPropertyChanged();
            }
        }

        public string SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                _selectedRow = value;
                OnPropertyChanged();
            }
        }

    }
}
