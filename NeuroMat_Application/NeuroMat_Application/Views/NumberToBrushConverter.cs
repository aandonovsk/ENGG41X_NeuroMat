using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NeuroMat_Application.Views
{
    public class NumberToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bc = new BrushConverter();

            double upperForceLimit = 20;

            double input = Math.Abs(double.Parse(value.ToString()));
            int equivalentBrushValue = (int)Math.Round(((Math.Min(1, input / upperForceLimit)) * 255), 0);
            string equivalentBrushValueInHex = equivalentBrushValue.ToString("X");
            string brushColorInHex;
            if (equivalentBrushValue < 16)
            {
                brushColorInHex = string.Format("#0{0}0{0}00", equivalentBrushValueInHex);
            }
            else
            {
                brushColorInHex = string.Format("#{0}{0}00", equivalentBrushValueInHex);
            }
            SolidColorBrush background = (SolidColorBrush)bc.ConvertFrom(brushColorInHex);
            return background;                   
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
