using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArduinoDriver;
using ArduinoDriver.SerialProtocol;
using ArduinoUploader.Hardware;


namespace NeuroMat_Application.Model
{
    public class PressureSensorArduinoControl
    {
        private const ArduinoModel AttachedArduino = ArduinoModel.Mega2560;
        private readonly byte[] controlPins = new byte[] { 10, 9, 8 };
        private readonly byte[] analogSensorPins = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        private int numberOfAveragedSamples;
        private ArduinoDriver.ArduinoDriver driver;

        // Make Delays adjustable

        public PressureSensorArduinoControl(int NumberOfAveragedSamples)
        {
            numberOfAveragedSamples = NumberOfAveragedSamples;
            driver = new ArduinoDriver.ArduinoDriver(AttachedArduino, "COM24", true);
            InitializeDigitialOutputPins(driver);
        }

        public double[][] ReadPressureSensorVoltages()
        {
            double[][] allSensorVoltages = new double[8][];
            using (driver)
            {
                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0: // Sensor Column A
                            DigitalValue[] controlValuesA = new DigitalValue[] { DigitalValue.Low, DigitalValue.Low, DigitalValue.Low };
                            SelectSensorColumn(driver, controlValuesA);
                            break;
                        case 1: // Sensor Column B
                            DigitalValue[] controlValuesB = new DigitalValue[] { DigitalValue.High, DigitalValue.Low, DigitalValue.Low };
                            SelectSensorColumn(driver, controlValuesB);
                            break;
                        case 2: // Sensor Column C
                            DigitalValue[] controlValuesC = new DigitalValue[] { DigitalValue.Low, DigitalValue.High, DigitalValue.Low };
                            SelectSensorColumn(driver, controlValuesC);
                            break;
                        case 3: // Sensor Column D
                            DigitalValue[] controlValuesD = new DigitalValue[] { DigitalValue.High, DigitalValue.High, DigitalValue.Low };
                            SelectSensorColumn(driver, controlValuesD);
                            break;
                        case 4: // Sensor Column E
                            DigitalValue[] controlValuesE = new DigitalValue[] { DigitalValue.Low, DigitalValue.Low, DigitalValue.High };
                            SelectSensorColumn(driver, controlValuesE);
                            break;
                        case 5: // Sensor Column F
                            DigitalValue[] controlValuesF = new DigitalValue[] { DigitalValue.High, DigitalValue.Low, DigitalValue.High };
                            SelectSensorColumn(driver, controlValuesF);
                            break;
                        case 6: // Sensor Column G
                            DigitalValue[] controlValuesG = new DigitalValue[] { DigitalValue.Low, DigitalValue.High, DigitalValue.High };
                            SelectSensorColumn(driver, controlValuesG);
                            break;
                        case 7: // Sensor Column H
                            DigitalValue[] controlValuesH = new DigitalValue[] { DigitalValue.High, DigitalValue.High, DigitalValue.High };
                            SelectSensorColumn(driver, controlValuesH);
                            break;
                    }

                    Thread.Sleep(100);

                    double[] avgSensorVoltagesInColumn = new double[analogSensorPins.Length];
                    for (int j = 0; j < numberOfAveragedSamples; j++)
                    {
                        for (int row = 0; row < analogSensorPins.Length; row++)
                        {
                            int sensorVoltage = driver.Send(new AnalogReadRequest(analogSensorPins[row])).PinValue;
                            avgSensorVoltagesInColumn[row] = avgSensorVoltagesInColumn[row] + sensorVoltage;

                            if (j == (numberOfAveragedSamples - 1))
                            {
                                avgSensorVoltagesInColumn[row] = (avgSensorVoltagesInColumn[row] / numberOfAveragedSamples) * 0.0049;
                            }
                        }
                        Thread.Sleep(10);
                    }
                    allSensorVoltages[i] = avgSensorVoltagesInColumn;
                }
            }
                return allSensorVoltages;
        }

        private void InitializeDigitialOutputPins(ArduinoDriver.ArduinoDriver driver)
        {
            for (int i = 0; i < controlPins.Length; i++)
            {
                driver.Send(new PinModeRequest(controlPins[i], PinMode.Output));
            }
        }

        private void SelectSensorColumn(ArduinoDriver.ArduinoDriver arduinoDriver, DigitalValue[] controlValues)
        {
            for (int i = 0; i < controlValues.Length; i++)
            {
                arduinoDriver.Send(new DigitalWriteRequest(controlPins[i], controlValues[i]));
            }
        }

    }
}
