using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame3DPlayground.Entities;

namespace MonoGame3DPlayground
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Matrix projection;

        private BaseEntity entity;
        private Camera camera;
        private World world;

        private bool debugMode; 
        private KeyboardState keyboardState;

        // For instructions
        private SpriteFont font;
        private Instructions instructions;

        public Main()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1200;
            this.graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.debugMode = true;
            this.keyboardState = Keyboard.GetState();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.font = Content.Load<SpriteFont>("fonts/coordinateFont");
            this.instructions = new Instructions(this.graphics.PreferredBackBufferWidth, this.font);

            // Create player object & load model
            this.entity = new BaseEntity(Content);

            // Create camera object
            this.camera = new Camera();

            // Create world
            this.world = new World(12, 12, entity, Content);
            this.projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 200f);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();
            // Exit using escape
            if (newState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (newState.IsKeyDown(Keys.Tab) && keyboardState.IsKeyUp(Keys.Tab))
            {
                this.debugMode = !this.debugMode;
            }
            this.keyboardState = newState;

            this.camera.Update();
            this.world.Update();

            base.Update(gameTime);
        }

        private void DrawInstructions()
        {

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(255, 205, 205));

            // Draw minimap (if debugging)
            if (this.debugMode)
            {
                this.world.Draw2D(spriteBatch);
            }

            // Draw 3D assets
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            this.world.Draw3D(camera.View, projection, this.debugMode);

            // Add instructions
            this.instructions.Draw(spriteBatch);
            
            base.Draw(gameTime);
        }
    }
}
