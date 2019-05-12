using System;
using System.IO;
using System.Linq;

namespace MainGUI.RealTimeMeasurement
{
    public struct MeasurementData
    {
        // temperatures of medium
        internal double Tc1;
        internal double Tc2;
        internal double Tc3;
        internal double Tc4;

        // air temperature
        internal double TinA;
        internal double ToutA;

        // water temperature
        internal double TinW;
        internal double ToutW;

        // water volume flow
        internal double v;

        // temperature of water in barrel
        internal double Tvn;

        // current and voltage
        internal double I;
        internal double U;

        internal double COP;
    }

    public class RealTimeMeasurementData
    {
        private MeasurementData _measurementData = new MeasurementData();
        private RealTimeMeasurementWindow realTimeMeasurementWindow;

        public RealTimeMeasurementData(RealTimeMeasurementWindow realTimeMeasurementWindow)
        {
            this.realTimeMeasurementWindow = realTimeMeasurementWindow;
        }

        /// <summary>
        /// Read data from file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>
        /// <see cref="Tuple{T1,T2}"/>containing:
        /// <see cref="Tuple{T1,T2}.Item1"/>: <see cref="bool"/> if the read was successful.
        /// <see cref="Tuple{T1,T2}.Item2"/>: struct of data.
        /// </returns>
        public Tuple<bool, MeasurementData> ReadData(string fileName)
        {
            string lastEntry = File.ReadLines(fileName).Last();
            string[] data = lastEntry.Split(';');

            if (data.Length < 13)
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("Wrong data format in: " + fileName);
                return new Tuple<bool, MeasurementData>(false, _measurementData);
            }

            bool readSuccessful = true;

            #region Data to struct

            // temperatures of medium
            int i = 0;
            if (!double.TryParse(data[i++], out _measurementData.Tc1))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse Tc1. " + data[i]);
                readSuccessful = false;
            }
            if (!double.TryParse(data[i++], out _measurementData.Tc2))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse Tc2. " + data[i]);
                readSuccessful = false;
            }
            if (!double.TryParse(data[i++], out _measurementData.Tc3))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse Tc3. " + data[i]);
                readSuccessful = false;
            }
            if (!double.TryParse(data[i++], out _measurementData.Tc4))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse Tc4. " + data[i]);
                readSuccessful = false;
            }

            // air temperature
            if (!double.TryParse(data[i++], out _measurementData.TinA))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse TinA. " + data[i]);
                readSuccessful = false;
            }
            if (!double.TryParse(data[i++], out _measurementData.ToutA))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse ToutA. " + data[i]);
                readSuccessful = false;
            }

            // water temperature
            if (!double.TryParse(data[i++], out _measurementData.TinW))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse TinW. " + data[i]);
                readSuccessful = false;
            }
            if (!double.TryParse(data[i++], out _measurementData.ToutW))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse ToutW. " + data[i]);
                readSuccessful = false;
            }

            // water flow
            if (!double.TryParse(data[i++], out _measurementData.v))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse v. " + data[i]);
                readSuccessful = false;
            }

            // temperature of water in barrel
            if (!double.TryParse(data[i++], out _measurementData.Tvn))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse Tan. " + data[i]);
                readSuccessful = false;
            }

            // I and U
            if (!double.TryParse(data[i++], out _measurementData.I))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse I. " + data[i]);
                readSuccessful = false;
            }
            if (!double.TryParse(data[i++], out _measurementData.U))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("couldn't parse U. " + data[i]);
                readSuccessful = false;
            }

            if (!double.TryParse(data[i++], out _measurementData.COP))
            {
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("Couldn't parse COP. " + data[i]);
                readSuccessful = false;
            }

            #endregion

            if (readSuccessful)
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage(
                    "Reading data from: " + fileName + " successful!");
            else
                realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage(
                    "Reading data from: " + fileName + " unsuccessful!");

            #region Message of loaded data

            string message = "";
            message += nameof(_measurementData.Tc1) + "=" + _measurementData.Tc1 + ";";
            message += nameof(_measurementData.Tc2) + "=" + _measurementData.Tc2 + ";";
            message += nameof(_measurementData.Tc3) + "=" + _measurementData.Tc3 + ";";
            message += nameof(_measurementData.Tc4) + "=" + _measurementData.Tc4 + ";";

            message += nameof(_measurementData.TinA) + "=" + _measurementData.TinA + ";";
            message += nameof(_measurementData.ToutA) + "=" + _measurementData.ToutA + ";";

            message += nameof(_measurementData.TinW) + "=" + _measurementData.TinW + ";";
            message += nameof(_measurementData.ToutW) + "=" + _measurementData.ToutW + ";";

            message += nameof(_measurementData.v) + "=" + _measurementData.v + ";";
            message += nameof(_measurementData.Tvn) + "=" + _measurementData.Tvn + ";";

            message += nameof(_measurementData.I) + "=" + _measurementData.I + ";";
            message += nameof(_measurementData.U) + "=" + _measurementData.U + ";";

            message += nameof(_measurementData.COP) + "=" + _measurementData.COP + ";";
            #endregion

            realTimeMeasurementWindow.ConsoleManager.ConsoleWriteMessage("Data: " + message);

            return new Tuple<bool, MeasurementData>(readSuccessful, _measurementData);
        }
    }
}