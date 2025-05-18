using System;

namespace GAG.EasyWebCam
{
    public class EasyWebcamEvents
    {
        public static event Action OnCameraInitialized;
        public static void RaiseOnCameraInitialized()
        {
            OnCameraInitialized?.Invoke();
        }

        public static event Action<int> OnCameraStarted;
        public static void RaiseOnCameraStarted(int cameraIndex)
        {
            OnCameraStarted?.Invoke(cameraIndex);
        }

        public static event Action OnCameraCaptuerd;
        public static void RaiseOnCameraCaptured()
        {
            OnCameraCaptuerd?.Invoke();
        }

        public static event Action OnPreviewUICreated;
        public static void RaiseOnPreviewUICreated()
        {
            OnPreviewUICreated?.Invoke();
        }

        public static event Action OnPreviewCapturedAndSaved;
        public static void RaiseOnPreviewCapturedAndSaved()
        {
            OnPreviewCapturedAndSaved?.Invoke();
        }

        public static event Action<string> OnSavePhotoPathCreated;
        public static void RaiseOnSavePhotoPathCreated(string path)
        {
            OnSavePhotoPathCreated?.Invoke(path);
        }
    }
}
