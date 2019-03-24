using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroMat_Application.Commands;
using NeuroMat_Application.Model;

namespace NeuroMat_Application.ViewModel
{
    public class MainViewModel : NotifyProperyChangedBase
    {
        public Command TestArduinoCode { get; private set; }
        public Command TestDummyArduinoCode { get; private set; }
        public PressureSensorArduinoControl PressureSensorArduinoControl;

        public double[][] PressureSensorCalibrationSlopes { get; private set; }
        public double[][] PressureSensorCalibrationBias { get; private set; }
        public ObservableCollection<PressureSensorRowDataTemplate> FilteredPressureSensorVoltageValues { get; private set; }

        private int NumberOfAveragedPressureSensorSamples = 2;
        private int PressSensVoltDividerResistance = 47000;
        private int PressureSensorInputVoltage = 5;


        public MainViewModel() // Constructor
        {
            TestArduinoCode = new Command(TestArduinoCodeMethod, () => true);
            TestDummyArduinoCode = new Command(TestDummyArduinoCodeMethod, () => true);
            FilteredPressureSensorVoltageValues = new ObservableCollection<PressureSensorRowDataTemplate>();
            GenerateDefaultPressureSensorValues();
            ReadPressureSensorCalibrationDataFromTextFiles();
        }

        private void TestArduinoCodeMethod(object parameter)
        {
            PressureSensorArduinoControl = new PressureSensorArduinoControl(NumberOfAveragedPressureSensorSamples);
            double[][] pressureSensorVoltageValues = PressureSensorArduinoControl.ReadPressureSensorVoltages();
            double[][] pressureSensorForceValues = ConvertPressureSensorVoltageToEquivalentForce(pressureSensorVoltageValues);
            double[][] filteredPressureSensorForceValues = NormFilterSensorForceValues(pressureSensorForceValues);
            //GeneratePressureSensorObject(filteredPressureSensorForceValues);
            GeneratePressureSensorObject(pressureSensorForceValues);
        }

        private void TestDummyArduinoCodeMethod(object parameter)
        {
            double[][] pressureSensorVoltageValues = ReadCalibrationDataFromFile.ObtainCalibrationDataFromTextFile("NeuroMatDummyRun.txt");
            double[][] pressureSensorForceValues = ConvertPressureSensorVoltageToEquivalentForce(pressureSensorVoltageValues);
            double[][] filteredPressureSensorForceValues = NormFilterSensorForceValues(pressureSensorForceValues);
            GeneratePressureSensorObject(filteredPressureSensorForceValues);
            //GeneratePressureSensorObject(pressureSensorForceValues);
        }

        private void GenerateDefaultPressureSensorValues()
        {
            for (int i = 0; i < 13; i++)
            {
                PressureSensorRowDataTemplate pressureSensorRowDefaultData = new PressureSensorRowDataTemplate { SelectedRow = String.Format("Row {0}", i + 1) };
                FilteredPressureSensorVoltageValues.Add(pressureSensorRowDefaultData);
            }
        }

        private void ReadPressureSensorCalibrationDataFromTextFiles()
        {
            PressureSensorCalibrationSlopes = ReadCalibrationDataFromFile.ObtainCalibrationDataFromTextFile("NeuroMatCalibrationSlopes.txt");
            PressureSensorCalibrationBias = ReadCalibrationDataFromFile.ObtainCalibrationDataFromTextFile("NeuroMatCalibrationBias.txt");
        }

        private void GeneratePressureSensorObject(double[][] pressureSensorVoltageValues)
        {
            FilteredPressureSensorVoltageValues.Clear();

            for (int i = 0; i < 13; i++)
            {
                IList<double> rowSensorValues = new List<double>();
                for (int j = 0; j < 8; j++)
                {
                    rowSensorValues.Add(pressureSensorVoltageValues[j][i]);
                }
                FilteredPressureSensorVoltageValues.Add(new PressureSensorRowDataTemplate { SelectedRow = String.Format("Row {0}", i+1), PressureSensorData = rowSensorValues });
            }
        }

        private double[][] ConvertPressureSensorVoltageToEquivalentForce(double[][] pressureSensorVoltageValues)
        {
            double[][] pressureSensorForceValues = new double[8][];
            for (int i = 0; i < 8; i++)
            {
                double[] pressureSensorResitanceInRow = new double[13];
                double[] pressureSensorForceInRows = new double[13];
                for (int j = 0; j < 13; j++)
                {
                    pressureSensorResitanceInRow[j] = Math.Round((pressureSensorVoltageValues[i][j] * PressSensVoltDividerResistance) / (PressureSensorInputVoltage - pressureSensorVoltageValues[i][j]),2);
                    pressureSensorForceInRows[j] = Math.Round((pressureSensorResitanceInRow[j] - PressureSensorCalibrationBias[i][j]) / PressureSensorCalibrationSlopes[i][j],2);
                    pressureSensorForceInRows[j] = pressureSensorForceInRows[j] < 0 ? 0 : pressureSensorForceInRows[j];
                    Debug.Write(pressureSensorForceInRows[j].ToString());
                    Debug.Write(";");
                }
                Debug.WriteLine("");
                pressureSensorForceValues[i] = pressureSensorForceInRows; 
            }

            //Correction for two "bad" sensors
            pressureSensorForceValues[2][4] = (pressureSensorForceValues[1][4] + pressureSensorForceValues[3][4]) / 2;
            pressureSensorForceValues[2][7] = (pressureSensorForceValues[1][7] + pressureSensorForceValues[3][7]) / 2;
            return pressureSensorForceValues;
        }

        private double[][] NormFilterSensorForceValues(double[][] pressureSensorForceValues)
        {
            double[][] zeroPaddedSensorForceValues = new double[10][];
            double[] zeroPad = new double[15];
            zeroPaddedSensorForceValues[0] = zeroPad;
            zeroPaddedSensorForceValues[9] = zeroPad;

            for (int i = 0; i < 8; i++)
            {
                zeroPad = new double[15];
                for (int j = 0; j < 13; j++)
                {
                    zeroPad[j + 1] = pressureSensorForceValues[i][j];
                }
                zeroPaddedSensorForceValues[i + 1] = zeroPad;
            }

            double[][] normFilteredSensorForceValues = new double[8][];

            for (int i = 1; i < 9; i++)
            {
                double[] normFilteredSensorForceInRow = new double[13];
                for (int j = 1; j < 14; j++)
                {
                    normFilteredSensorForceInRow[j - 1] = Math.Round((zeroPaddedSensorForceValues[i - 1][j - 1] + zeroPaddedSensorForceValues[i][j - 1] + zeroPaddedSensorForceValues[i + 1][j - 1] + zeroPaddedSensorForceValues[i - 1][j] + zeroPaddedSensorForceValues[i][j] + zeroPaddedSensorForceValues[i + 1][j] + zeroPaddedSensorForceValues[i - 1][j + 1] + zeroPaddedSensorForceValues[i][j + 1] + zeroPaddedSensorForceValues[i + 1][j + 1]) / 9, 2);
                }
                normFilteredSensorForceValues[i - 1] = normFilteredSensorForceInRow;
            }
            return normFilteredSensorForceValues;
        }
    }
}
