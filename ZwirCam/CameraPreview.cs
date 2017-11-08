using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Vision;
using Android.Util;

namespace ZwirCam
{
    public class CameraPreview : ViewGroup, ISurfaceHolderCallback
    {
        const string TAG = "CameraPreview";

        Context context;
        SurfaceView surfaceView;
        CameraSource cameraSource;
        GraphicOverlay graphicOverlay;
        bool mStart;
        protected bool isAvailable { get; set; }
        

        public CameraPreview(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.context = context;
            mStart = false;
            isAvailable = false;

            surfaceView = new SurfaceView(context);
            surfaceView.Holder.AddCallback(this);
            AddView(surfaceView);
        }

        public void Start(CameraSource cameraSource)
        {
            if (cameraSource == null)
                Stop();

            this.cameraSource = cameraSource;

            if (cameraSource != null)
            {
                mStart = true;
                StartIfReady();
            }
        }

        public void Start(CameraSource cameraSource, GraphicOverlay graphicOverlay)
        {
            this.graphicOverlay = graphicOverlay;
            Start(cameraSource);
        }

        public void Stop()
        {
            if (cameraSource != null)
                cameraSource.Stop();
        }

        public void Release()
        {
            if (cameraSource != null)
            {
                cameraSource.Release();
                cameraSource = null;
            }
        }

        private void StartIfReady()
        {
            if (mStart && isAvailable)
            {
                cameraSource.Start(surfaceView.Holder);
                if (graphicOverlay != null)
                {
                    var size = cameraSource.PreviewSize;
                    int min = Math.Min(size.Width, size.Height);
                    int max = Math.Max(size.Width, size.Height);
                    // if portraint mode that rotate camera
                    if (IsPortraitMode())
                        graphicOverlay.SetInformationFromCamera(min, max, cameraSource.CameraFacing);
                    else
                        graphicOverlay.SetInformationFromCamera(max, min, cameraSource.CameraFacing);
                    graphicOverlay.Clean();
                }
                mStart = false;
            }
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            int width = 320;
            int height = 240;
            if (cameraSource != null)
            {
                var size = cameraSource.PreviewSize;
                if (size != null)
                {
                    width = size.Width;
                    height = size.Height;
                }
            }

            // if portrait mode that change height with width
            if (IsPortraitMode())
            {
                int tmp = width;
                width = height;
                height = tmp;
            }

            int layoutWidth = right - left;
            int layoutHeight = bottom - top;

            // Calculates the width and height and matches
            int viewWidth = layoutWidth;
            int viewHeight = (int)(((float)layoutWidth / (float)width) * height);

            if (viewHeight > layoutHeight)
            {
                viewHeight = layoutHeight;
                viewWidth = (int)(((float)layoutHeight / (float)height) * width);
            }

            for (int i = 0; i < ChildCount; ++i)
                GetChildAt(i).Layout(0, 0, viewWidth, viewHeight);

            try
            {
                StartIfReady();
            }
            catch (Exception ex)
            {
              //  Android.Util.Log.Error(TAG, "Nie uda³o siê uruchomiæ kamery.", ex);
            }
        }

        bool IsPortraitMode()
        {
            var screenOrientation = context.Resources.Configuration.Orientation;
            if (screenOrientation == Android.Content.Res.Orientation.Landscape)
                return false;
            else if (screenOrientation == Android.Content.Res.Orientation.Portrait)
                return true;

            return false;
        }

        #region implemented interface ISurfaceHolderCallback
        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
            
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            this.isAvailable = true;
            try
            {
                this.StartIfReady();
            }
            catch (Exception ex)
            {

            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            this.isAvailable = false;
        }
        #endregion
    }
}