using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroMat_Application.Commands;
using NeuroMat_Application.Model;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace NeuroMat_Application.ViewModel
{
    public class MainViewModel : NotifyProperyChangedBase
    {
        Stopwatch testingWatch = new Stopwatch();
        // Patient Info Init
        //// Commands 
        public Command OpenPrompt { get; private set; }
        public Command Newpat { get; private set; }
        public Command Loadpat { get; private set; }
        public Command Clearpat { get; private set; }
        public Command Engresults { get; private set; }
        public string STname { get; set; }
        public string STbirth { get; set; }
        public string STheight { get; set; }
        Random generator = new Random();

        //// Else
        public ObservableCollection<PatientDataTemplate> PatientDataList { get; private set; }
        public ObservableCollection<PatientDataTemplate> PatientDataList_1 { get; private set; }
        public ObservableCollection<PatientDataTemplate> SelectedPatientLoaded { get; private set; }

        public ObservableCollection<NCVTestResults> LeftNCVTestResults { get; private set; }
        public ObservableCollection<NCVTestResults> RightNCVTestResults { get; private set; }

        private PatientDataTemplate _selectedPatient = new PatientDataTemplate();
        public PatientDataTemplate SelectedPatient
        {
            get { return _selectedPatient; }
            set
            {
                _selectedPatient = value;
                SelectedPatientLoaded.Clear();
                SelectedPatientLoaded.Add(_selectedPatient);
                OnPropertyChanged();
            }
        }
    
        // Test Results Init
        //// Commands
        public PressureSensorArduinoControl PressureSensorArduinoControl;
        public Command StartPressureMapping { get; private set; }
        public Command LoadSamplePressureSensorData { get; private set; }
        public Command CalibratePressureSensors { get; private set; }
        public Command TurnOnDynamicPressureSensing { get; private set; }
        public Command TurnOffDynamicPressureSensing { get; private set; }
        public Command PerformNCVSampleTest { get; private set; }

        //// Misc
        public double[][] PressureSensorCalibrationSlopes { get; private set; }

        public double[][] PressureSensorCalibrationBias2 { get; private set; }

        public double[][] PressureSensorCalibrationBias { get; private set; }
        public ObservableCollection<PressureSensorRowDataTemplate> FilteredPressureSensorVoltageValues { get; private set; }
        public EnableUiParameters EnableUiParameters { get; set; }
        private static System.Timers.Timer DynamicTestingTimer;

        // Constants
        private int NumberOfAveragedPressureSensorSamples = 4;
        private int PressSensVoltDividerResistance = 47000;
        private int PressureSensorInputVoltage = 5;

        public MainViewModel() 
        {
            // Misc
            EnableUiParameters = new EnableUiParameters();
            EnableUiParameters.IsRealTimeSensingEnabled = false;
            EnableUiParameters.StatusUpdate = "Status Update:";

            // Patient Info
            PatientDataList = new ObservableCollection<PatientDataTemplate>();
            PatientDataList_1 = new ObservableCollection<PatientDataTemplate>();
            SelectedPatientLoaded = new ObservableCollection<PatientDataTemplate>();
            SelectedPatient = new PatientDataTemplate();
            SelectedPatientLoaded.Clear();
            LeftNCVTestResults = new ObservableCollection<NCVTestResults>();
            RightNCVTestResults = new ObservableCollection<NCVTestResults>();

            OpenPrompt = new Command(RaisePrompt, () => true);
            Newpat = new Command(NewpatMethod, () => true);
            Loadpat = new Command(LoadpatMethod, () => true);
            Clearpat = new Command(ClearPatMethod, () => true);

            // Testing 
            StartPressureMapping = new Command(parameter => PerformPressureMapping(parameter, NumberOfAveragedPressureSensorSamples), () => true);
            LoadSamplePressureSensorData = new Command(LoadSamplePressureData, () => true);
            CalibratePressureSensors = new Command(UpdatePressureSensorCalibrationBias, () => true);
            TurnOnDynamicPressureSensing = new Command(EnableDynamicPressureSensing, () => !EnableUiParameters.IsRealTimeSensingEnabled);
            TurnOffDynamicPressureSensing = new Command(DisableDynamicPressureSensing, () => EnableUiParameters.IsRealTimeSensingEnabled);
            PerformNCVSampleTest = new Command(PerformNCVDemo, () => true);

            FilteredPressureSensorVoltageValues = new ObservableCollection<PressureSensorRowDataTemplate>();
            GenerateDefaultPressureSensorValues();
            ReadPressureSensorCalibrationDataFromTextFiles();
            CreatePatientResults();


        }

        private void LoadpatMethod(object parameter) //Method load in all patient data from text file
        {
            //Read from textfile
            {   // Open the text file using a stream reader.
                string docPath = AppDomain.CurrentDomain.BaseDirectory;

                int lineCount = File.ReadLines(Path.Combine(docPath, "41X Patient Data.txt")).Count();
                PatientDataList.Clear();
                PatientDataList_1.Clear();
                using (StreamReader sr = new StreamReader(Path.Combine(docPath, "41X Patient Data.txt"), true))
                {
                    // Read the stream to a string, and write the string to the console.
                    for (int i = 0; i < lineCount; i++)
                    {
                        String line = sr.ReadLine();
                        string[] words = line.Split(';');
                        PatientDataTemplate patientDataTemplate = new PatientDataTemplate() { Name = words[0], ID = words[1], Birth = words[2], Age = words[3], Height = words[4] };
                        PatientDataList.Add(patientDataTemplate);
                    }
                }
                if (PatientDataList.Any())
                {
                    PatientDataList_1.Add(PatientDataList.First());
                }

                else
                {
                    PatientDataTemplate patientDataTemplate = new PatientDataTemplate();
                    PatientDataList_1.Add(patientDataTemplate);
                }
            }
        }

        private void NewpatMethod(object parameter) //Method creates a new patient
        {
            var result = MessageBox.Show("Confirm create patient action?", "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Cancel)
            {
                String r = generator.Next(0, 999999).ToString("D6");

                // Set a variable to the Documents path.
                string docPath = AppDomain.CurrentDomain.BaseDirectory;

                DateTime dob = Convert.ToDateTime(STbirth);
                int STage = CalculateAge(dob);

                // Append text to an existing file named "WriteLines.txt".
                using (TextWriter outputFile = new StreamWriter(Path.Combine(docPath, "41X Patient Data.txt"), true))
                {
                    string lines = STname + ";" + r + ";" + STbirth + ";" + STage + ";" + STheight;
                    outputFile.WriteLine(lines);
                }
                LoadpatMethod(null);
            }
        }

        private void ClearPatMethod(object parameter)
        {
            var result = MessageBox.Show("Confirm clear patient data action?", "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Cancel)
            {
                string docPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "41X Patient Data.txt");
                File.WriteAllText(docPath, "");
                LoadpatMethod(null);
            }
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }

        private void PerformPressureMapping(object parameter, int numberOfAveragedPressureSensorSamples)
        {
            var result = MessageBox.Show("Confirm pressure mapping action?", "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Cancel)
            {
                PressureSensorArduinoControl = new PressureSensorArduinoControl();
                testingWatch.Start();
                double[][] pressureSensorVoltageValues = PressureSensorArduinoControl.ReadPressureSensorVoltages(numberOfAveragedPressureSensorSamples);
                //double[][] pressureSensorVoltageValues = PressureSensorArduinoControl.ReadPressureSensorVoltages(2);
                double[][] pressureSensorForceValues = ConvertPressureSensorVoltageToEquivalentForce(pressureSensorVoltageValues);
                double[][] filteredPressureSensorForceValues = NormFilterSensorForceValues(pressureSensorForceValues);
                GeneratePressureSensorObject(filteredPressureSensorForceValues);
                testingWatch.Stop();
                string length = testingWatch.ElapsedMilliseconds.ToString();

                Debug.WriteLine("Time =");
                Debug.WriteLine(length);
            }
        }

        private void PerformPressureMapping(object parameter, int numberOfAveragedPressureSensorSamples, bool isRealTime)
        {
            PressureSensorArduinoControl = new PressureSensorArduinoControl();
            testingWatch.Start();
            double[][] pressureSensorVoltageValues = PressureSensorArduinoControl.ReadPressureSensorVoltages(numberOfAveragedPressureSensorSamples);
            //double[][] pressureSensorVoltageValues = PressureSensorArduinoControl.ReadPressureSensorVoltages(2);
            double[][] pressureSensorForceValues = ConvertPressureSensorVoltageToEquivalentForce(pressureSensorVoltageValues);
            double[][] filteredPressureSensorForceValues = NormFilterSensorForceValues(pressureSensorForceValues);
            GeneratePressureSensorObject(filteredPressureSensorForceValues);
            testingWatch.Stop();
            string length = testingWatch.ElapsedMilliseconds.ToString();

            Debug.WriteLine("Time =");
            Debug.WriteLine(length);
        }



        private void LoadSamplePressureData(object parameter)
        {
                double[][] pressureSensorVoltageValues = ReadCalibrationDataFromFile.ObtainCalibrationDataFromTextFile("NeuroMatDummyRun.txt");
                double[][] pressureSensorForceValues = ConvertPressureSensorVoltageToEquivalentForce(pressureSensorVoltageValues, true);
                double[][] filteredPressureSensorForceValues = NormFilterSensorForceValues(pressureSensorForceValues);
                GeneratePressureSensorObject(filteredPressureSensorForceValues);
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
            PressureSensorCalibrationBias2 = PressureSensorCalibrationBias;
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
                FilteredPressureSensorVoltageValues.Add(new PressureSensorRowDataTemplate { SelectedRow = String.Format("Row {0}", i + 1), PressureSensorData = rowSensorValues });
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
                    pressureSensorResitanceInRow[j] = ConvertVoltageSensorReadingToResistance(pressureSensorVoltageValues[i][j]);
                    pressureSensorForceInRows[j] = Math.Round((pressureSensorResitanceInRow[j] - PressureSensorCalibrationBias[i][j]) / PressureSensorCalibrationSlopes[i][j], 2);
                    pressureSensorForceInRows[j] = pressureSensorForceInRows[j] < 0 ? 0 : pressureSensorForceInRows[j];
                    Debug.Write(pressureSensorForceInRows[j].ToString());
                    Debug.Write(";");
                }
                Debug.WriteLine("");
                pressureSensorForceValues[i] = pressureSensorForceInRows;
            }

            //Correction for two "bad" sensors
            pressureSensorForceValues[2][3] = (pressureSensorForceValues[1][3] + pressureSensorForceValues[3][3]) / 2;
            pressureSensorForceValues[2][4] = (pressureSensorForceValues[1][4] + pressureSensorForceValues[3][4]) / 2;
            pressureSensorForceValues[2][7] = (pressureSensorForceValues[1][7] + pressureSensorForceValues[3][7]) / 2;
            pressureSensorForceValues[6][5] = (pressureSensorForceValues[1][7] + pressureSensorForceValues[3][7]) / 2;
            return pressureSensorForceValues;
        }

        private double[][] ConvertPressureSensorVoltageToEquivalentForce(double[][] pressureSensorVoltageValues, bool isCalibration)
        {
            double[][] pressureSensorForceValues = new double[8][];
            for (int i = 0; i < 8; i++)
            {
                double[] pressureSensorResitanceInRow = new double[13];
                double[] pressureSensorForceInRows = new double[13];
                for (int j = 0; j < 13; j++)
                {
                    pressureSensorResitanceInRow[j] = ConvertVoltageSensorReadingToResistance(pressureSensorVoltageValues[i][j]);
                    pressureSensorForceInRows[j] = Math.Round((pressureSensorResitanceInRow[j] - PressureSensorCalibrationBias2[i][j]) / PressureSensorCalibrationSlopes[i][j], 2);
                    pressureSensorForceInRows[j] = pressureSensorForceInRows[j] < 0 ? 0 : pressureSensorForceInRows[j];
                    Debug.Write(pressureSensorForceInRows[j].ToString());
                    Debug.Write(";");
                }
                Debug.WriteLine("");
                pressureSensorForceValues[i] = pressureSensorForceInRows;
            }

            //Correction for two "bad" sensors
            pressureSensorForceValues[2][3] = (pressureSensorForceValues[1][3] + pressureSensorForceValues[3][3]) / 2;
            pressureSensorForceValues[2][4] = (pressureSensorForceValues[1][4] + pressureSensorForceValues[3][4]) / 2;
            pressureSensorForceValues[2][7] = (pressureSensorForceValues[1][7] + pressureSensorForceValues[3][7]) / 2;
            pressureSensorForceValues[6][5] = (pressureSensorForceValues[1][7] + pressureSensorForceValues[3][7]) / 2;
            return pressureSensorForceValues;
        }

        private double ConvertVoltageSensorReadingToResistance(double pressureSensorVoltageReading)
        {
            double pressureSensorResistance = Math.Round((pressureSensorVoltageReading * PressSensVoltDividerResistance) / (PressureSensorInputVoltage - pressureSensorVoltageReading), 2);
            return pressureSensorResistance;
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

        private void UpdatePressureSensorCalibrationBias(object parameter)
        {
            var result = MessageBox.Show("Confirm calibrate action?", "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Cancel)
            {
                PressureSensorArduinoControl = new PressureSensorArduinoControl();
                double[][] pressureSensorVoltageValues = PressureSensorArduinoControl.ReadPressureSensorVoltages(NumberOfAveragedPressureSensorSamples);
                //double[][] pressureSensorVoltageValues = ReadCalibrationDataFromFile.ObtainCalibrationDataFromTextFile("NeuroMatDummyRun.txt"); Used to test
                double[][] pressureSensorResistanceValuesFromCal = new double[8][];

                for (int i = 0; i < 8; i++)
                {
                    double[] columnPressureSensorResistanceValues = new double[13];
                    for (int j = 0; j < 13; j++)
                    {
                        columnPressureSensorResistanceValues[j] = ConvertVoltageSensorReadingToResistance(pressureSensorVoltageValues[i][j]);
                    }
                    pressureSensorResistanceValuesFromCal[i] = columnPressureSensorResistanceValues;
                }
                PressureSensorCalibrationBias = pressureSensorResistanceValuesFromCal;
                double[][] pressureSensorForceValues = ConvertPressureSensorVoltageToEquivalentForce(pressureSensorVoltageValues);
                double[][] filteredPressureSensorForceValues = NormFilterSensorForceValues(pressureSensorForceValues);
                GeneratePressureSensorObject(filteredPressureSensorForceValues);
            }

        }

        private void EnableDynamicPressureSensing(object parameter)
        {
            int pressureSensingTimeInterval = 20;
            EnableUiParameters.IsRealTimeSensingEnabled = true;
            PerformPressureMapping(null, 2);
            DynamicTestingTimer = new System.Timers.Timer((pressureSensingTimeInterval) * 1000);
            DynamicTestingTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            DynamicTestingTimer.AutoReset = true;
            DynamicTestingTimer.Enabled = true;
            TurnOnDynamicPressureSensing.RaiseCanExecuteChanged();
            TurnOffDynamicPressureSensing.RaiseCanExecuteChanged();
        }

        private void DisableDynamicPressureSensing(object parameter)
        {
            PerformPressureMapping(null, 2);
            EnableUiParameters.IsRealTimeSensingEnabled = false;
            DynamicTestingTimer.Enabled = false;
            TurnOnDynamicPressureSensing.RaiseCanExecuteChanged();
            TurnOffDynamicPressureSensing.RaiseCanExecuteChanged();
        }

        private void OnTimedEvent(object parameter, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                PerformPressureMapping(null, 1, true);
            });
        }

        private void PerformNCVDemo(object parameter)
        {
            var result = MessageBox.Show("Confirm NCV test action?", "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Cancel)
            {
                PressureSensorArduinoControl = new PressureSensorArduinoControl();
                PressureSensorArduinoControl.RunNcvLedSequence(0);
                Thread.Sleep(500);
                PressureSensorArduinoControl = new PressureSensorArduinoControl();
                PressureSensorArduinoControl.RunNcvLedSequence(1);

                EnableUiParameters.LeftNCVTestResults = "32";
                EnableUiParameters.RightNCVTestResults = "63";
                LeftNCVTestResults.First(o => o.Name == "T5").NcvValue = EnableUiParameters.LeftNCVTestResults;
                RightNCVTestResults.First(o => o.Name == "T5").NcvValue = EnableUiParameters.RightNCVTestResults;
            }
        }

        private void CreatePatientResults()
        {
            List<string> vertebrate = new List<string>();
            for (int i = 0; i < 7; i++)
            {
                vertebrate.Add(String.Format("C{0}", i + 1));
            }
            for (int i = 0; i < 12; i++)
            {
                vertebrate.Add(String.Format("T{0}", i + 1));
            }

            for (int i = 0; i < 5; i++)
            {
                vertebrate.Add(String.Format("L{0}", i + 1));
            }

            for (int i = 0; i < 5; i++)
            {
                vertebrate.Add(String.Format("S{0}", i + 1));
            }
            for (int i = 0; i < vertebrate.Count; i++)
            {
                LeftNCVTestResults.Add(new NCVTestResults { Name = vertebrate[i], NcvValue = "0" });
                RightNCVTestResults.Add(new NCVTestResults { Name = vertebrate[i], NcvValue = "0" });
            }
        }

        private void RaisePrompt(object parameter)
        {
            var result = MessageBox.Show("Confirm action?", "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        }
    }
}

