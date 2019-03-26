using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroMat_Application.ViewModel;

namespace NeuroMat_Application.Model
{
    public class PatientDataTemplate : NotifyProperyChangedBase
    {
        private string _name;
        private string _height;
        private string _birth;
        private string _id;
        private string _age;

        public PatientDataTemplate()
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

        public string Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }
        public string Birth
        {
            get { return _birth; }
            set
            {
                _birth = value;
                OnPropertyChanged();
            }
        }
        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public string Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }
    }
}
