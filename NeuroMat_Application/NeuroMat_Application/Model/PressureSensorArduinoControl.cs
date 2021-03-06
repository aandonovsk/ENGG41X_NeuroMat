﻿using System;
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
        private readonly byte[] ledPins = new byte[] { 12, 2, 3, 4, 5, 6 };
        private readonly byte[] controlPins = new byte[] { 10, 9, 8 };
        private readonly byte[] analogSensorPins = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        private ArduinoDriver.ArduinoDriver driver;

        // Make Delays adjustable

        public PressureSensorArduinoControl()
        {
            driver = new ArduinoDriver.ArduinoDriver(AttachedArduino, "COM24", true);
            InitializeDigitialOutputPins(driver);
        }

        public double[][] ReadPressureSensorVoltages(int numberOfAveragedSamples)
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
                            SelectPressureSensorColumn(driver, controlValuesA);
                            break;
                        case 1: // Sensor Column B
                            DigitalValue[] controlValuesB = new DigitalValue[] { DigitalValue.High, DigitalValue.Low, DigitalValue.Low };
                            SelectPressureSensorColumn(driver, controlValuesB);
                            break;
                        case 2: // Sensor Column C
                            DigitalValue[] controlValuesC = new DigitalValue[] { DigitalValue.Low, DigitalValue.High, DigitalValue.Low };
                            SelectPressureSensorColumn(driver, controlValuesC);
                            break;
                        case 3: // Sensor Column D
                            DigitalValue[] controlValuesD = new DigitalValue[] { DigitalValue.High, DigitalValue.High, DigitalValue.Low };
                            SelectPressureSensorColumn(driver, controlValuesD);
                            break;
                        case 4: // Sensor Column E
                            DigitalValue[] controlValuesE = new DigitalValue[] { DigitalValue.Low, DigitalValue.Low, DigitalValue.High };
                            SelectPressureSensorColumn(driver, controlValuesE);
                            break;
                        case 5: // Sensor Column F
                            DigitalValue[] controlValuesF = new DigitalValue[] { DigitalValue.High, DigitalValue.Low, DigitalValue.High };
                            SelectPressureSensorColumn(driver, controlValuesF);
                            break;
                        case 6: // Sensor Column G
                            DigitalValue[] controlValuesG = new DigitalValue[] { DigitalValue.Low, DigitalValue.High, DigitalValue.High };
                            SelectPressureSensorColumn(driver, controlValuesG);
                            break;
                        case 7: // Sensor Column H
                            DigitalValue[] controlValuesH = new DigitalValue[] { DigitalValue.High, DigitalValue.High, DigitalValue.High };
                            SelectPressureSensorColumn(driver, controlValuesH);
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

        private void SelectPressureSensorColumn(ArduinoDriver.ArduinoDriver arduinoDriver, DigitalValue[] controlValues)
        {
            for (int i = 0; i < controlValues.Length; i++)
            {
                arduinoDriver.Send(new DigitalWriteRequest(controlPins[i], controlValues[i]));
            }
        }


        public void RunNcvLedSequence(int side)
        {
            using (driver)
            {
                if (side == 0)
                {
                    driver.Send(new DigitalWriteRequest(ledPins[2], DigitalValue.High));
                    Thread.Sleep(200);
                    driver.Send(new DigitalWriteRequest(ledPins[2], DigitalValue.Low));
                    Thread.Sleep(300);
                    driver.Send(new DigitalWriteRequest(ledPins[1], DigitalValue.High));
                    Thread.Sleep(200);
                    driver.Send(new DigitalWriteRequest(ledPins[1], DigitalValue.Low));
                    Thread.Sleep(300);
                    driver.Send(new DigitalWriteRequest(ledPins[0], DigitalValue.High));
                    Thread.Sleep(200);
                    driver.Send(new DigitalWriteRequest(ledPins[0], DigitalValue.Low));
                }

                else if (side == 1)
                {
                    driver.Send(new DigitalWriteRequest(ledPins[3], DigitalValue.High));
                    Thread.Sleep(200);
                    driver.Send(new DigitalWriteRequest(ledPins[3], DigitalValue.Low));
                    Thread.Sleep(1500);
                    driver.Send(new DigitalWriteRequest(ledPins[4], DigitalValue.High));
                    Thread.Sleep(200);
                    driver.Send(new DigitalWriteRequest(ledPins[4], DigitalValue.Low));
                    Thread.Sleep(1500);
                    driver.Send(new DigitalWriteRequest(ledPins[5], DigitalValue.High));
                    Thread.Sleep(200);
                    driver.Send(new DigitalWriteRequest(ledPins[5], DigitalValue.Low));
                }
            }

        }

        private void InitializeDigitialOutputPins(ArduinoDriver.ArduinoDriver driver)
        {
            for (int i = 0; i < controlPins.Length; i++)
            {
                driver.Send(new PinModeRequest(controlPins[i], PinMode.Output));
            }

            for (int i = 0; i < 6; i++)
            {
                driver.Send(new PinModeRequest(ledPins[i], PinMode.Output));
            }
        }



    }
}
