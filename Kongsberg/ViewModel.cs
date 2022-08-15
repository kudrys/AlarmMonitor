using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonParser;
using SimulatorLogic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace AlarmMonitor {
    public class ViewModel {

        public Dictionary<int, Sensor> sensorsDict = new Dictionary<int, Sensor>();
        public List<Receiver> receiverList = new List<Receiver>();

        public ObservableCollection<Sensor.sensValuesStruct> obsSensValues1 = new ObservableCollection<Sensor.sensValuesStruct>();
        public ObservableCollection<Sensor.sensValuesStruct> obsSensValues2 = new ObservableCollection<Sensor.sensValuesStruct>();
        public ObservableCollection<Sensor.sensValuesStruct> obsSensValues3 = new ObservableCollection<Sensor.sensValuesStruct>();

        public ViewModel() {
            // Load sensors from json
            JsonObjectSensors jObj = new JsonObjectSensors();
            JsonObjectReceivers rObj = new JsonObjectReceivers();
            JsonLoader.LoadConfigs("sensorConfig.json", "receiverConfig.json", ref jObj, ref rObj);

            receiverList = rObj.Receivers;

            foreach (var s in jObj.sensors) {
                sensorsDict.Add(s.ID, s);
            }
        }
    }
}


