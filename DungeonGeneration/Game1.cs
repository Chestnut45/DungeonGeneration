using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace DungeonGeneration
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Generator generator = new Generator();
        KeyboardState ks, prevks;

        //Graphics stuff
        int tilesize = 8;
        SpriteFont testfont;

        //Dungeon stuff

        //Game objects
        Player player = new Player();

        //Textures
        Texture2D sWall;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            sWall = Content.Load<Texture2D>("sWallTest");
            testfont = Content.Load<SpriteFont>("Fonts/testfont");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.P) & !prevks.IsKeyDown(Keys.P))
            {
                player.currentDungeon = generator.generateDungeon(50, false);
                player.currentRoom = player.currentDungeon[0]; //currentDungeon is just a 1d array of rooms that all belong to the current dungeon
            }

            //Change rooms
            if (ks.IsKeyDown(Keys.Left) & !prevks.IsKeyDown(Keys.Left))
            {
                if (player.currentRoom.adjacentRooms[0] != null)
                {
                    player.currentRoom = player.currentRoom.adjacentRooms[0];
                }
            }

            if (ks.IsKeyDown(Keys.Up) & !prevks.IsKeyDown(Keys.Up))
            {
                if (player.currentRoom.adjacentRooms[1] != null)
                {
                    player.currentRoom = player.currentRoom.adjacentRooms[1];
                }
            }

            if (ks.IsKeyDown(Keys.Right) & !prevks.IsKeyDown(Keys.Right))
            {
                if (player.currentRoom.adjacentRooms[2] != null)
                {
                    player.currentRoom = player.currentRoom.adjacentRooms[2];
                }
            }

            if (ks.IsKeyDown(Keys.Down) & !prevks.IsKeyDown(Keys.Down))
            {
                if (player.currentRoom.adjacentRooms[3] != null)
                {
                    player.currentRoom = player.currentRoom.adjacentRooms[3];
                }
            }

            //Updsate previous keyboard state
            prevks = ks;
            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred);

            //Go through current room and draw tiles
            if (player.currentRoom != null)
            {
                for (int i = 0; i < player.currentRoom.map.GetLength(0); i++)
                {
                    for (int j = 0; j < player.currentRoom.map.GetLength(1); j++)
                    {
                        if (player.currentRoom.map[i, j] == 1)
                        {
                            spriteBatch.Draw(sWall, new Vector2(i * tilesize, j * tilesize));
                        }
                    }
                }
            }

            //Draw debug text
            if (player.currentRoom != null)
            {
                spriteBatch.DrawString(testfont, "Room " + player.currentRoom.id.ToString(), Vector2.Zero, Color.White);
                spriteBatch.DrawString(testfont, "X: " + player.currentRoom.mx.ToString() + " " + "Y: " + player.currentRoom.my.ToString(), new Vector2(0,16), Color.White);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
