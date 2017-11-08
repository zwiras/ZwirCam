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
using Android.Graphics;
using Android.Gms.Vision;
using Android.Util;

namespace ZwirCam
{
    public class GraphicOverlay : View
    {
        object isBlocked = new object();
        int pixelsX = 0, pixelsY = 0;
        float scalingFactorX = 1.0f, scalingFactorY = 1.0f;
        CameraFacing cameraFacing = CameraFacing.Back;
        List<FrameGraphic> frames = new List<FrameGraphic>();

        /**
         * Class responsible for graphical overlay operations
         * */
        public abstract class FrameGraphic
        {
            private GraphicOverlay graphicOverlay;

            public FrameGraphic(GraphicOverlay graphicOverlay)
            {
                this.graphicOverlay = graphicOverlay;
            }

            /**
             * The method responsible for drawing the frame on the screen
             */
            public abstract void Draw(Canvas canvas);

            /**
             * Scaling of frame drawing in X axis
             */
            public float ScalingX(float horizontally)
            {
                return horizontally * graphicOverlay.scalingFactorX;
            }

            /**
             * Scaling of frame drawing in Y axis
             */
            public float ScalingY(float vertically)
            {
                return vertically * graphicOverlay.scalingFactorY;
            }

            /**
             * Transform the X coordinate
             */
            public float TransformX(float x)
            {
                if (graphicOverlay.cameraFacing == CameraFacing.Front)
                {
                    return graphicOverlay.Width - ScalingX(x);
                }
                else
                {
                    return ScalingX(x);
                }
            }

            /**
             * Transform the Y coordinate
             */
            public float TransformY(float y)
            {
                return ScalingY(y);
            }

            // The method responsible for redrawing the screen
            public void PostInvalidate()
            {
                graphicOverlay.PostInvalidate();
            }
        }

        public GraphicOverlay(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        /**
         * Removes all drawn frames
         */
        public void Clean()
        {
            lock (isBlocked)
            {
                frames.Clear();
            }
            PostInvalidate();
        }

        /**
         * Add a frame that you want to display
         */
        public void Add(FrameGraphic frameGraphic)
        {
            lock (isBlocked)
            {
                frames.Add(frameGraphic);
            }
            PostInvalidate();
        }

        /**
         * Remove frame
         */
        public void Remove(FrameGraphic frameGraphic)
        {
            lock (isBlocked)
            {
                frames.Remove(frameGraphic);
            }
            PostInvalidate();
        }

        /**
         * Sets the height and width of the camera
         */
        public void SetInformationFromCamera(int previewWidth, int previewHeight, CameraFacing cameraFacing)
        {
            lock (isBlocked)
            {
                pixelsX = previewWidth;
                pixelsY = previewHeight;
                this.cameraFacing = cameraFacing;
            }
            PostInvalidate();
        }

        /**
         * Drawing frames
         */
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            lock (isBlocked)
            {
                if ((pixelsX != 0) && (pixelsY != 0))
                {
                    scalingFactorX = (float)canvas.Width / (float)pixelsX;
                    scalingFactorY = (float)canvas.Height / (float)pixelsY;
                }

                foreach (FrameGraphic frameGraphic in frames)
                    frameGraphic.Draw(canvas);
            }
        }
    }
}