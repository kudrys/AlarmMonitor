using System;
using System.Collections.Generic;
using System.Timers;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace SimulatorLogic {

    public interface IReceiver {
        void Update(ISensor subject);
    }

    public interface ISensor {
        void Attach(IReceiver receiver);
        void Detach(IReceiver receiver);
        void Notify();
    }

    /// <summary>
    /// Sensor class with internal logic
    /// </summary>
    public class Sensor : ISensor {

        private CustomTimer timer;
        private List<IReceiver> m_receivers = new List<IReceiver>();
        private readonly object resourceLock = new object();
        private const string
            alarm = "ALARM",
            warning = "WARNING",
            normal = "NORMAL";

        private sensValuesStruct tempstructValues;

        // Init parameters
        public int ID { get; set; }
        public string Type { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string EncoderType { get; set; }
        public int Frequency { get; set; }

        // Sensed parameters
        public int Value { get; set; } = 0;
        public string Quality { get; set; }


        /// <summary>
        ///     Sensed values:
        ///     int sensorID
        ///     string type
        ///     int value
        ///     string quality
        /// </summary>
        public struct sensValuesStruct {
            public sensValuesStruct(int id, string type, int value, string quality) {
                ID = id;
                Type = type;
                Value = value;
                Quality = quality;
            }
            public int ID { get; set; }
            public string Type { get; set; }
            public int Value { get; set; }
            public string Quality { get; set; }
        }
        public List<sensValuesStruct> sensedValues = new List<sensValuesStruct>();
        //public ObservableCollection<Sensor.sensValuesStruct> obsSensValues;

        public void Attach(IReceiver receiver) {
            this.m_receivers.Add(receiver);
        }

        public void Detach(IReceiver receiver) {
            this.m_receivers.Remove(receiver);
        }

        private void DrawValue(int minVal, int maxVal)
        {
            Random r = new Random();
            int rInt = r.Next(minVal, maxVal);
            Value = rInt;
        }

        private void QualifyValue(int val) {

            int totalRangeValues = Math.Abs(MaxValue - MinValue);
            int percentQuality = 0;

                
            if (MinValue < 0 && val < 0)
                percentQuality = (int)Math.Round((double)(100 * Math.Abs(MinValue - val)) / totalRangeValues);
            else 
                percentQuality = (int)Math.Round((double)(100 * val) / totalRangeValues);

            switch (percentQuality) {
                case var n when (n <= 10 || n>=90):
                    Console.WriteLine($"Sensor({this.ID}): 10% or 90% ALARM with value: {n}");
                    this.Quality = alarm;
                    break;

                case var n when ((n > 10 && n <=25) || (n >= 75 && n < 90)):
                    Console.WriteLine($"Sensor({this.ID}): 25% or 75% WARNING with value: {n}");
                    this.Quality = warning;
                    break;

                case var n when (n > 25 && n < 75):
                    Console.WriteLine($"Sensor({this.ID}): 50% NORMAL with value: {n}");
                    this.Quality = normal;
                    break;
            }
        }

        private void GetMeasurement(Object source, ElapsedEventArgs e) {
            lock (resourceLock) {
                DrawValue(MinValue, MaxValue);
                QualifyValue(this.Value);
                var strct = new sensValuesStruct(this.ID, this.Type, this.Value, this.Quality);
                sensedValues.Add(strct);
                System.Windows.Application.Current.Dispatcher.Invoke(() => ((CustomTimer)source).refObsColl.Add(strct));
                Notify();
            }
        }

        // Run an update in each receiver.
        public void Notify() {
            Console.WriteLine($"Sensor({this.ID}): Notifying receivers...");

            while (sensedValues.Count != 0) {
                foreach (var receiver in m_receivers) {
                    if ((receiver as Receiver).Active)
                        receiver.Update(this);
                }
                sensedValues.RemoveAt(sensedValues.Count - 1);
            }
        }

        public void RunSensor(ref ObservableCollection<Sensor.sensValuesStruct> obsColl) {
            // Create a timer with interval.
            //timer = new CustomTimer(this.Frequency * 1000, refObsColl);

            timer = new CustomTimer {
                Interval = this.Frequency * 1000,
                refObsColl = obsColl
            };
            timer.Elapsed += GetMeasurement;
            timer.Start();
        }

        public void StopSensor() {
            timer.Stop();
        }
    }

    class CustomTimer : System.Timers.Timer {
        public ObservableCollection<Sensor.sensValuesStruct> refObsColl;
    }

    /// <summary>
    /// Receiver class with internal logic
    /// </summary>
    public class Receiver : IReceiver {

        private readonly object logLock = new object();

        // Receiver parameters
        public int ID { get; set; }
        public bool Active { get; set; }
        public List<int> Sensors { get; set; }

        public void Update(ISensor sensor) {

            lock (logLock) {
                var lastSensed = (sensor as Sensor).sensedValues.Last();
                LogToFile(lastSensed.ID, lastSensed.Type, lastSensed.Value, lastSensed.Quality);
            }
        }

        private void LogToFile(int sensorID, string sensType, int sensedValue, string sensedQuality) {

            string logFileName = $"ReceiverLog-{ID}.txt";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Logs\", logFileName);

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ff") + " - [" +sensedQuality+"] - (Sensor ID: "+sensorID+", Sensor Type:"+ sensType+ ", Value: "+ sensedValue+")");
            File.AppendAllText(path, sb.ToString() + Environment.NewLine);
            sb.Clear();
        }
    }
}
