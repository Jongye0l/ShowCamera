using System.IO;
using Newtonsoft.Json;

namespace ShowCamera {
    public class Settings {
        private static readonly string SettingPath = Path.Combine(Main.ModEntry.Path, "Settings.json");
        public static Settings Instance;
        public string CameraName = Main.NoneSelect;
        public float Size = 1;
        [JsonIgnore] public string SizeString;
        public float LocateX = 0;
        [JsonIgnore] public string LocateXString;
        public float LocateY = 0;
        [JsonIgnore] public string LocateYString;
        public float Rotate = 0;
        [JsonIgnore] public string RotateString;
        public float CameraX = 0;
        [JsonIgnore] public string CameraXString;
        public float CameraY = 0;
        [JsonIgnore] public string CameraYString;
        public float CameraWidth = 1;
        [JsonIgnore] public string CameraWidthString;
        public float CameraHeight = 1;
        [JsonIgnore] public string CameraHeightString;
        public bool FlipHorizontal = false;
        public bool FlipVertical = false;
        
        public static Settings CreateInstance() {
            Instance = File.Exists(SettingPath) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingPath)) : new Settings();
            return Instance;
        }

        public void Save() {
            File.WriteAllText(SettingPath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
        }
    }
}