using System;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace ShowCamera {
    public class Main {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        public static bool Enabled;
        public static Settings Settings;
        public static string[] CameraList;
        public static bool UnknownSelect;
        internal const string NoneSelect = "None";
        private static int _selectedCamera = -1;

        public static void Setup(UnityModManager.ModEntry modEntry) {
            Logger = modEntry.Logger;
            ModEntry = modEntry;
            Settings = Settings.CreateInstance();
            modEntry.OnToggle = OnToggle;
            modEntry.OnShowGUI = OnShowGUI;
            modEntry.OnGUI = OnGUI;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            Enabled = value;
            if(value) WebCamController.Create();
            else WebCamController.Remove();
            return true;
        }

        private static void OnShowGUI(UnityModManager.ModEntry modEntry) {
            LoadCameraList();
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry) {
            Values values = GetValues();
            Camera(values.Camera);
            if(Slider(ref Settings.Size, 1, ref Settings.SizeString, 0, 5, values.Size)) WebCamController.UpdateSize();
            if(Slider(ref Settings.LocateX, 0, ref Settings.LocateXString, -1, 1, "X")) WebCamController.UpdateLocate();
            if(Slider(ref Settings.LocateY, 0, ref Settings.LocateYString, -1, 1, "Y")) WebCamController.UpdateLocate();
            if(Slider(ref Settings.Rotate, 0, ref Settings.RotateString, -180, 180, values.Rotate)) WebCamController.UpdateRotate();
            if(Slider(ref Settings.CameraX, 0, ref Settings.CameraXString, -1, 1, $"{values.Camera} X")) WebCamController.UpdateCameraSize();
            if(Slider(ref Settings.CameraY, 0, ref Settings.CameraYString, -1, 1, $"{values.Camera} Y")) WebCamController.UpdateCameraSize();
            if(Slider(ref Settings.CameraWidth, 0, ref Settings.CameraWidthString, 0, 1, $"{values.Camera} {values.Width}")) WebCamController.UpdateCameraSize();
            if(Slider(ref Settings.CameraHeight, 0, ref Settings.CameraHeightString, 0, 1, $"{values.Camera} {values.Height}")) WebCamController.UpdateCameraSize();
            if(AddSettingToggle(ref Settings.FlipHorizontal, values.FlipHorizontal)) WebCamController.UpdateRotate();
            if(AddSettingToggle(ref Settings.FlipVertical, values.FlipVertical)) WebCamController.UpdateRotate();
        }

        private static void LoadCameraList() {
            bool needChange = false;
            WebCamDevice[] devices = WebCamTexture.devices;
            if(CameraList != null) {
                if(CameraList.Length + (UnknownSelect ? 2 : 1) != devices.Length) needChange = true;
                else {
                    for(int i = UnknownSelect ? 2 : 1; i < CameraList.Length; i++) {
                        if(CameraList[i] == devices[i - (UnknownSelect ? 2 : 1)].name) continue;
                        needChange = true;
                        break;
                    }
                }
            } else needChange = true;
            if(!needChange) return;
            UnknownSelect = Settings.CameraName != NoneSelect && devices.All(device => device.name != Settings.CameraName);
            CameraList = new string[devices.Length + (UnknownSelect ? 2 : 1)];
            _selectedCamera = 0;
            if(UnknownSelect) {
                CameraList[0] = Settings.CameraName;
                CameraList[1] = NoneSelect;
            } else CameraList[0] = NoneSelect;
            for(int i = UnknownSelect ? 2 : 1; i < CameraList.Length; i++) 
                if((CameraList[i] = devices[i - (UnknownSelect ? 2 : 1)].name) == Settings.CameraName) _selectedCamera = i;
        }

        private static void Camera(string text) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text);
            GUILayout.Space(4f);
            if(UnityModManager.UI.PopupToggleGroup(ref _selectedCamera, CameraList, "카메라를 선택해주세요")) {
                if(Settings.CameraName != CameraList[_selectedCamera]) {
                    Settings.CameraName = CameraList[_selectedCamera];
                    Settings.Save();
                    WebCamController.LoadNewCamera();
                }
                LoadCameraList();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static bool Slider(ref float value, float defaultValue, ref string valueString, float min, float max, string text) {
            bool changed = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label(text);
            GUILayout.Space(4f);
            float result = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(300));
            if(result != value) {
                value = result;
                valueString = $"{value:0.00}";
                changed = true;
                Settings.Save();
            }
            if(valueString == null) valueString = $"{value:0.00}";
            valueString = GUILayout.TextField(valueString, GUILayout.Width(50));
            try {
                result = valueString.IsNullOrEmpty() ? defaultValue : float.Parse(valueString);
            } catch (FormatException) {
                result = defaultValue;
                valueString = $"{result:0.00}";
            }
            if(result != value) {
                value = result;
                changed = true;
                Settings.Save();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return changed;
        }

        private static bool AddSettingToggle(ref bool value, string text) {
            bool changed = false;
            if(GUILayout.Toggle(value, text)) {
                if(!value) {
                    value = true;
                    changed = true;
                    Settings.Save();
                }
            } else if(value) {
                value = false;
                changed = true;
                Settings.Save();
            }
            return changed;
        }

        public static Values GetValues() {
            return RDString.language == SystemLanguage.Korean ? Values.Korean : Values.English;
        }
    }
}