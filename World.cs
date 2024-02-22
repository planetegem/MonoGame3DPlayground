using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame3DPlayground.Entities;
using System;

namespace MonoGame3DPlayground
{
    internal struct Cell
    {
        public int X; // X-coordinate in field
        public int Y; // Y-coordinate in field
        public bool Tile; // Cell is valid tile or not (for collision detection)
    }

    internal class World
    {
        // Models and textures: need to be loaded after running constructor
        private Texture2D tileTexture;
        private Texture2D squareTexture;
        private Texture2D circleTexture;
        private Model tileModel;
        private Model circleModel;
        private SpriteFont coordinateFont;

        // World matrix, redeclared evry time world is drawn
        private Matrix world;

        // Width & height of field, provided in constructor
        private int width;
        private int height;

        // Field: boolean value of a tile determines if player can move here
        private Cell[,] field;
        private int tileSize;

        // Player entity
        private BaseEntity player;

        public World(int width, int height, BaseEntity player, ContentManager gameContent)
        {
            this.width = width;
            this.height = height;
            this.field = new Cell[width, height];

            // Start filling the field: outer layer = not valid for movement
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell newCell = new Cell();
                    newCell.X = x;
                    newCell.Y = y;
                    newCell.Tile = !(x == 0 || y == 0 || x == width - 1 || y == height - 1);
                    this.field[x, y] = newCell;
                }
            }

            // will determine size of a tile when drawn on screen
            this.tileSize = 20;
            this.player = player;

            // Load all models & textures
            this.tileTexture = gameContent.Load<Texture2D>("textures/tile");
            this.circleTexture = gameContent.Load<Texture2D>("textures/circle");
            this.squareTexture = gameContent.Load<Texture2D>("textures/square");
            this.tileModel = gameContent.Load<Model>("models/tile");
            this.circleModel = gameContent.Load<Model>("models/circle");
            this.coordinateFont = gameContent.Load<SpriteFont>("fonts/coordinateFont");
        }

        public void Update()
        {
            // First move player
            this.player.Update();

            // Then check if move was valid
            int newX = (int)Math.Round((this.player.Position.X + (this.width * 0.5f)) * this.tileSize);
            int newY = (int)Math.Round((this.player.Position.Y + (this.height * 0.5f)) * this.tileSize);
            int radius = (int)(this.player.BoundingScale * this.tileSize * 0.5f);

            foreach (Cell cell in this.field)
            {
                if (!cell.Tile)
                {
                    // Compare center of circle to every cell in field
                    int closestX = Math.Clamp(newX, cell.X * this.tileSize, (cell.X + 1) * this.tileSize);
                    int closestY = Math.Clamp(newY, cell.Y * this.tileSize, (cell.Y + 1) * this.tileSize);
                    int distanceX = newX - closestX;
                    int distanceY = newY - closestY;
                    double realDistance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

                    // If distance shows that circle is overlapping with invalid tile, reverse move
                    if (realDistance < radius)
                    {
                        this.player.Undo();
                    }
                }
            }
        }

        public void Draw2D(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Cycle through field array and draw cells where applicable
            foreach (Cell cell in this.field)
            {
                if (cell.Tile)
                {
                    spriteBatch.Draw(this.squareTexture, new Rectangle(cell.X * this.tileSize, cell.Y * this.tileSize, this.tileSize, this.tileSize), Color.White);
                }
            }

            // Draw player bounding circle on field
            int newX = (int)((player.Position.X + (this.width * 0.5f)) * this.tileSize);
            int newY = this.height * this.tileSize - (int)((player.Position.Y + (this.height * 0.5f)) * this.tileSize);
            int radius = (int)(player.BoundingScale * this.tileSize * 0.5f);

            spriteBatch.Draw(circleTexture, new Rectangle(newX - radius, newY - radius, radius * 2, radius * 2), Color.White);

            // Show player coordinates under minimap
            string coordinates = "X" + Math.Round(this.player.Position.X, 2) + " / Y" + Math.Round(this.player.Position.Y, 2);
            float stringX = Instructions.AlignText(coordinates, this.coordinateFont, new Rectangle(0, 0, this.width * this.tileSize, 0));

            spriteBatch.DrawString(this.coordinateFont, coordinates, new Vector2(stringX, (this.height - 0.5f) * this.tileSize), Color.Black);

            spriteBatch.End();
        }

        public void Draw3D(Matrix view, Matrix projection, bool debug)
        {
            float startX = 0.5f - (this.width * 0.5f);
            float startY = 0.5f - (this.height * 0.5f);

            foreach (Cell cell in field)
            {
                if (cell.Tile)
                {
                    this.world = Matrix.CreateTranslation(new Vector3(startX + cell.X, startY + cell.Y, 0));

                    foreach (ModelMesh mesh in tileModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.TextureEnabled = true;
                            effect.Texture = this.tileTexture;
                            effect.World = world;
                            effect.View = view;
                            effect.Projection = projection;
                        }
                        mesh.Draw();
                    }
                }
            }

            if (debug)
            {
                this.world = Matrix.CreateScale(player.BoundingScale) * Matrix.CreateTranslation(new Vector3(player.Position, 0.01f));
                foreach (ModelMesh mesh in this.circleModel.Meshes)
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
            this.player.DrawModel(view, projection);
        }
    }
}
