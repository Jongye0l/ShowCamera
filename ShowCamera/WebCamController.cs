using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ShowCamera {
    public class WebCamController : MonoBehaviour {
        private static readonly Settings Settings = Settings.Instance;
        public static WebCamController Instance;
        private static GameObject _gameObject;
        public Canvas canvas;
        public RawImage rawImage;
        public WebCamTexture webcamTexture;
        public RectTransform panelTransform;
        public bool isNull = false;
        
        public static void Create() {
            _gameObject = new GameObject("Webcam Object");
            _gameObject.AddComponent<WebCamController>();
            DontDestroyOnLoad(_gameObject);
        }
        public static void Remove() {
            DestroyImmediate(_gameObject);
            _gameObject = null;
            Instance = null;
        }

        private void OnDestroy() {
            webcamTexture.Stop();
        }

        private static Canvas CreateCanvas(GameObject gameObject) {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1;
            CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            return canvas;
        }

        public static void LoadNewCamera() {
            Instance.webcamTexture.Stop();
            Instance.GenerateWebCam();
        }

        private void Start() {
            Instance = this;
            canvas = CreateCanvas(_gameObject);
            GameObject webCamPanel = new GameObject("WebCam Object");
            panelTransform = webCamPanel.AddComponent<RectTransform>();
            webCamPanel.transform.SetParent(canvas.transform);
            rawImage = webCamPanel.AddComponent<RawImage>();
            GenerateWebCam();
            UpdateRotate();
        }

        private void GenerateWebCam() {
            isNull = Main.NoneSelect == Settings.CameraName;
            webcamTexture = new WebCamTexture(Settings.CameraName, Screen.width, Screen.height);
            if(isNull || WebCamTexture.devices.Any(device => device.name == Settings.CameraName)) webcamTexture.Play();
            rawImage.texture = webcamTexture;
            UpdateCameraSize();
        }

        private async void Update() {
            await Play();
        }

        private Task Play() {
            return Task.Run(() => {
                if(isNull || webcamTexture.isPlaying && WebCamTexture.devices.All(device => device.name != Settings.CameraName)) return;
                webcamTexture.Play();
                UpdateCameraSize();
            });
        }

        public static void UpdateSize() {
            Instance.panelTransform.sizeDelta = new Vector2(Instance.webcamTexture.width * Settings.Size * Settings.CameraWidth, Instance.webcamTexture.height * Settings.Size * Settings.CameraHeight);
            UpdateLocate();
        }

        public static void UpdateLocate() {
            Vector2 sizeDelta = Instance.panelTransform.sizeDelta;
            Instance.panelTransform.anchoredPosition = new Vector2(Settings.LocateX * (1920 - sizeDelta.x) / 2, Settings.LocateY * (1080 - sizeDelta.y) / 2);
        }

        public static void UpdateCameraSize() {
            Instance.rawImage.uvRect = new Rect((Settings.CameraX + 1) * (1 - Settings.CameraWidth) / 2, 
                (Settings.CameraY + 1) * (1 - Settings.CameraHeight) / 2, Settings.CameraWidth, Settings.CameraHeight);
            UpdateSize();
        }

        public static void UpdateRotate() {
            Instance.panelTransform.eulerAngles = new Vector3(Settings.FlipVertical ? 180 : 0, Settings.FlipHorizontal ? 180 : 0, Settings.Rotate);
        }
    }
}