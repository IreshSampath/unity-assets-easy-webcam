using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GAG.EasyWebCam
{
    public class EasyCamHandler : MonoBehaviour
    {
        [SerializeField] bool _isAutoProcessing = false;
        [SerializeField] bool _isCountdownEnable = false;
        [SerializeField] int _cameraCount = 0;

        [SerializeField] List<RawImage> _cameraViews;
        [SerializeField] List<RawImage> _previews;
        [SerializeField] RectTransform _capturePanel;
        [SerializeField] TMP_Text _countdownText;

        WebCamDevice[] _cameras;
        List<WebCamTexture> _webCamTextures = new List<WebCamTexture>();
        Queue<Texture2D> _capturedPhotos = new Queue<Texture2D>();

        int _selectedCameraIndex = 0;

        private void OnEnable()
        {
            EasyCamEvents.OnCameraInitialized += InitializeCamera;
            EasyCamEvents.OnCameraStarted += SwitchOnCamera;
            EasyCamEvents.OnCameraCaptuerd += TakePhoto;
            EasyCamEvents.OnPreviewUICreated += CreatePreviewUI;
            EasyCamEvents.OnPreviewCapturedAndSaved += CaptureAndSaveUI;
        }

        private void OnDisable()
        {
            EasyCamEvents.OnCameraInitialized -= InitializeCamera;
            EasyCamEvents.OnCameraStarted -= SwitchOnCamera;
            EasyCamEvents.OnCameraCaptuerd -= TakePhoto;
            EasyCamEvents.OnPreviewUICreated -= CreatePreviewUI;
            EasyCamEvents.OnPreviewCapturedAndSaved -= CaptureAndSaveUI;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //InitializeCamera();
        }

        // Update is called once per frame
        void Update()
        {
            //for (int i = 0; i < _cameraCount; i++)
            //{
            //    if (_webCamTextures[i].didUpdateThisFrame)
            //    {
            //        _cameraViews[i].texture = _webCamTextures[i];
            //    }
            //}
        }

        void SetFrameRateForWebcams(bool webcamsActive)
        {
            Application.targetFrameRate = webcamsActive ? 30 : 60;
        }

        void InitializeCamera()
        {
            _cameras = WebCamTexture.devices;
            print(_cameras.Count());
            foreach (var cam in _cameras)
            {
                print(cam.name);
            }

            if (_cameras.Length == 0)
            {
                Debug.Log("No camera detected");
                return;
            }
            else if (_cameras.Length < _cameraCount)
            {
                Debug.Log("Not enough cameras detected");
                return;
            }

            for (int i = 0; i < _cameraCount; i++)
            {
                if (_cameraViews[i] == null)
                {
                    Debug.Log("Camera view is not assigned");
                    return;
                }
                else
                {
                    print($"Camera view {i} is assigned");
                    WebCamTexture webCamTexture = new WebCamTexture(_cameras[i].name, Screen.width, Screen.height);
                    //WebCamTexture tex = new WebCamTexture(_cameras[i].name, 640, 480, 15);
                    //WebCamTexture webCamTexture = new WebCamTexture(_cameras[i].name, 320, 240, 10);
                    _webCamTextures.Add(webCamTexture);
                    _cameraViews[i].texture = _webCamTextures[i];
                }
            }
        }

        void SwitchOnCamera(int camIndex)
        {
            if (_webCamTextures == null)
            {
                Debug.Log("WebCamTextures is not initialized");
                return;
            }
            else
            {
                StartCoroutine(ActivateCameras(camIndex));
            }
        }

        IEnumerator ActivateCameras(int selectedCam)
        {
            SetFrameRateForWebcams(true);
            _selectedCameraIndex = selectedCam;

            if (_isAutoProcessing)
            {
                for (int i = 0; i < _cameraCount; i++)
                {
                    _webCamTextures[i].Play();
                    yield return new WaitForSeconds(0.1f); // short delay to spread out load
                }
            }
            else
            {
                _webCamTextures[_selectedCameraIndex].Play();
            }
        }

        void TakePhoto()
        {
            StartCoroutine(CaptureCamera());
        }

        IEnumerator ActivateCountdown()
        {
            int countDown = 3;

            while (countDown > 0)
            {
                _countdownText.text = countDown.ToString(); 
                Debug.Log(countDown);
                yield return new WaitForSeconds(1f); 
                countDown--;
            }

            _countdownText.text = ""; 
        }

        IEnumerator CaptureCamera()
        {
            if (_isAutoProcessing)
            {
                for (int i = 0; i < _cameraCount; i++)
                {
                    if (_isCountdownEnable)
                    {
                        yield return StartCoroutine(ActivateCountdown());
                    }
                    Texture2D photo = new Texture2D(_webCamTextures[i].width, _webCamTextures[i].height);
                    photo.SetPixels(_webCamTextures[i].GetPixels());
                    photo.Apply();

                    _capturedPhotos.Enqueue(photo);
                    if (_capturedPhotos.Count > _cameraCount)
                    {
                        Destroy(_capturedPhotos.Dequeue());
                    }
                    _webCamTextures[i].Stop();
                }
            }
            else
            {
                if (_isCountdownEnable)
                {
                    yield return StartCoroutine(ActivateCountdown());
                }

                Texture2D photo = new Texture2D(_webCamTextures[_selectedCameraIndex].width, _webCamTextures[_selectedCameraIndex].height);
                photo.SetPixels(_webCamTextures[_selectedCameraIndex].GetPixels());
                photo.Apply();
                _capturedPhotos.Enqueue(photo);
                if (_capturedPhotos.Count > _cameraCount)
                {
                    Destroy(_capturedPhotos.Dequeue());
                }
                _webCamTextures[_selectedCameraIndex].Stop();
            }
            SetFrameRateForWebcams(false);
        }

        void CreatePreviewUI()
        {
            Texture2D[] photos = _capturedPhotos.ToArray();
            for (int i = 0; i < _previews.Count; i++)
            {
                if (i < photos.Length)
                {
                    _previews[i].texture = photos[i];
                }
                else
                {
                    _previews[i].texture = null;
                }
            }
        }

        void CaptureAndSaveUI()
        {
            StartCoroutine(CaptureCoroutine());
        }

        IEnumerator CaptureCoroutine()
        {
            yield return new WaitForEndOfFrame(); 

            // Get screen-space rect of the panel
            Rect rect = RectTransformToScreenSpace(_capturePanel);

            // Capture entire screen as Texture
            Texture2D screenTex = ScreenCapture.CaptureScreenshotAsTexture();

            // Crop to panel area
            Texture2D cropped = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            cropped.SetPixels(screenTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
            cropped.Apply();
            Destroy(screenTex); // clean up

            // Save to file
            byte[] bytes = cropped.EncodeToPNG();
            string path = Path.Combine(Application.streamingAssetsPath, System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
            File.WriteAllBytes(path, bytes);

            Debug.Log("Saved UI capture to: " + path);
            EasyCamEvents.RaiseOnSavePhotoPathCreated(path);
        }

        // Helper to convert RectTransform to screen pixel Rect
        Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector3[] corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector2 topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

            return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        }
    }
}

