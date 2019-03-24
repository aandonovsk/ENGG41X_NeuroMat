using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroMat_Application.Model
{
    public static class ReadCalibrationDataFromFile
    {
        public static double[][] ObtainCalibrationDataFromTextFile(string fileName)
        {
            double[][] calibrationData = new double[8][];
            string executablePath = String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            string[] rawCalibrationData = System.IO.File.ReadAllLines(executablePath);

            for (int i = 0; i < 8; i++)
            {
                string[] rowData = rawCalibrationData[i].Split(';');
                double[] rowDataAsDouble = new double[13];

                for (int j = 0; j < 13; j++)
                {
                    rowDataAsDouble[j] = double.Parse(rowData[j]);
                }
                calibrationData[i] = rowDataAsDouble;
            }
            return calibrationData;
        }
    }
}
