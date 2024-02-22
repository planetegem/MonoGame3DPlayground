using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame3DPlayground.Entities
{
    internal class BaseEntity
    {
        // 3D model to be used
        private Model entityModel;
        public Model EntityModel { set { entityModel = value; } }
        private string entityName;
        public string EntityName { get { return entityName; } }       

        // Bounding circle used in collision detection
        private float boundingScale;
        public float BoundingScale { get { return boundingScale; } }

        // Positioning fields
        private float momentum = 0;
        private float angle = 0;
        public Vector2 Position { get; set; }

        // Previous position is saved for use in collision detection
        private Vector2 previousPosition;

        public BaseEntity(ContentManager gameContent)
        {
            this.entityModel = gameContent.Load<Model>("models/percolator");
            this.Position = new Vector2(0, 0);
            this.boundingScale = 0.5f;
            this.entityName = "models/percolator";
        }

        public void Update()
        {
            this.previousPosition = this.Position;
            // Process keyboard input
            KeyboardState keyboard = Keyboard.GetState();

            // Forwards and backwards movement
            if (keyboard.IsKeyDown(Keys.Down))
            {
                this.momentum += 0.0025f;
            }
            if (keyboard.IsKeyDown(Keys.Up))
            {
                this.momentum -= 0.0025f;
            }

            // If not moving forward or backward, apply inertion
            if (!keyboard.IsKeyDown(Keys.Up) && !keyboard.IsKeyDown(Keys.Down))
            {
                this.momentum *= 0.95f;
            }

            // Limit max momentum
            this.momentum = Math.Clamp(this.momentum, -0.075f, 0.075f);

            // Left and right turning behavior
            if (keyboard.IsKeyDown(Keys.Left))
            {
                this.angle += 0.1f;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                this.angle -= 0.1f;
            }

            // Calculate new position
            float xPos = (float) Math.Cos(this.angle);
            float yPos = (float) Math.Sin(this.angle);
            this.Position += new Vector2(xPos * this.momentum, yPos * this.momentum);
        }

        // Called when collision is detected in game area 
        public void Undo()
        {
            this.Position = this.previousPosition;
            this.momentum = 0;
        }

        public void DrawModel(Matrix view, Matrix projection)
        {         
            if (this.entityModel != null)
            {
                Matrix world = Matrix.CreateRotationZ(this.angle) * Matrix.CreateTranslation(new Vector3(this.Position, 0.02f));
                foreach (ModelMesh mesh in this.entityModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
            }
            else
            {
                throw new Exception("EntityModel was not loaded when rendering entity");
            }
        }
    }
}
