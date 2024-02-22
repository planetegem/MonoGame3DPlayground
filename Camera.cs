using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame3DPlayground
{
    internal class Camera
    {
        // Positioning fields
        private float cameraAngle;
        private float cameraMagnification;
        private float cameraBaseDistance;

        // Comparison fields
        private int startX;
        private float previousAngle;
        private MouseState oldState;
        private MouseState newState;

        // Final matrix
        private Matrix view;
        public Matrix View { get { return view; } }

        public Camera()
        {
            this.cameraAngle = MathHelper.ToRadians(180f);
            this.cameraMagnification = 1f;
            this.cameraBaseDistance = 10f;

            // Mouse interaction: save old mouse state to figure out when state changes
            this.oldState = Mouse.GetState();
        }

        public void Update()
        {
            this.newState = Mouse.GetState();

            // On click, save x coordinate & current camera angle
            if (this.newState.LeftButton == ButtonState.Pressed && this.oldState.LeftButton == ButtonState.Released)
            {
                this.startX = this.newState.X;
                this.previousAngle = this.cameraAngle;
            }

            // If dragging mouse, update camera angle
            if (this.newState.LeftButton == ButtonState.Pressed)
            {
                int diff = this.startX - this.newState.X;
                this.cameraAngle = this.previousAngle + diff * 0.01f;
            }

            // Check for differences in scroll wheel value and apply magnification
            int scrollValue = this.oldState.ScrollWheelValue - this.newState.ScrollWheelValue;

            if (scrollValue != 0)
            {
                this.cameraMagnification = Math.Clamp(this.cameraMagnification + scrollValue * 0.001f, 0.3f, 5f);
            }

            // Update camera with new angle
            float xPos = (float)Math.Sin(this.cameraAngle) * this.cameraMagnification * this.cameraBaseDistance;
            float yPos = (float)Math.Cos(this.cameraAngle) * this.cameraMagnification * this.cameraBaseDistance;
            float zPos = (float)Math.Sin(MathHelper.ToRadians(45f)) * this.cameraMagnification * this.cameraBaseDistance;

            this.view = Matrix.CreateLookAt(new Vector3(xPos, yPos, zPos), new Vector3(0, 0, 0), Vector3.UnitZ);

            // Save mouse state to check against next time
            this.oldState = this.newState;
        }
    }
}
