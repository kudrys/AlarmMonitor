using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorLogic;

namespace JsonParser
{
    public class JsonLoader {
        public static void LoadConfigs(string fileNameSensors, string fileNameReceivers, ref JsonObjectSensors jObj, ref JsonObjectReceivers rObj) {
            //string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Configs\", fileNameSensors);
            //using (StreamReader r = new StreamReader(path)) {
            //    string json = r.ReadToEnd();
            //    var jsonItems = JsonConvert.DeserializeObject<JsonObjectSensors>(json);
            //    jObj = jsonItems;
            //}

            //path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Configs\", fileNameReceivers);
            //using (StreamReader r = new StreamReader(path)) {
            //    string json = r.ReadToEnd();
            //    var jsonItems = JsonConvert.DeserializeObject<JsonObjectReceivers>(json);
            //    rObj = jsonItems;
            //}

            LoadConfigss(fileNameSensors, ref jObj);
            LoadConfigss(fileNameReceivers, ref rObj);
        }

        public static void LoadConfigss<T>(string fileName, ref T jObj) {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Configs\", fileName);
            using (StreamReader r = new StreamReader(path)) {
                string json = r.ReadToEnd();
                var jsonItems = JsonConvert.DeserializeObject<T>(json);
                jObj = jsonItems;
            }
        }
    }

    public class JsonObjectSensors {
        [JsonProperty("Sensors")]
        public List<Sensor> sensors { get; set; }
    }

    public class JsonObjectReceivers {
        public List<Receiver> Receivers { get; set; }
    }
}
