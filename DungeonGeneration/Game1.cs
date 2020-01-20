using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
        Camera cam = new Camera();

        //Dungeon stuff

        //Game objects
        Player player = new Player();
        Physics physics = new Physics();

        //Textures
        Texture2D sWall, sPlayer, sLockDoor, sLockDoorV, sKey;
        Texture2D hbtex;

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
            sPlayer = Content.Load<Texture2D>("sPlayerTest");
            sLockDoor = Content.Load<Texture2D>("sLockDoor");
            sLockDoorV = Content.Load<Texture2D>("sLockDoorVertical");
            testfont = Content.Load<SpriteFont>("Fonts/testfont");
            sKey = Content.Load<Texture2D>("sKey");

            //Hitbox shit
            //Make one pixel of color and transparency
            Byte transparency_amount = 100; //0 transparent; 255 opaque
            hbtex = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] c = new Color[1];
            c[0] = Color.FromNonPremultiplied(255, 0, 0, transparency_amount);
            hbtex.SetData<Color>(c);
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

            //Texture cheating
            if (player.texture != sPlayer)
            {
                player.texture = sPlayer;
            }

            // TODO: Add your update logic here
            ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.P) & !prevks.IsKeyDown(Keys.P))
            {
                player.currentDungeon = generator.generateDungeon(20, true);
                player.currentRoom = player.currentDungeon[0]; //currentDungeon is just a 1d array of rooms that all belong to the current dungeon
                player.x = player.currentRoom.map.GetLength(0) / 2;
                player.y = player.currentRoom.map.GetLength(1) / 2;

                //TODO: Build dungeon room objects. Load all in memory or one room / dungeon at a time?
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

            if (ks.IsKeyDown(Keys.F) & !prevks.IsKeyDown(Keys.F))
            {
                graphics.ToggleFullScreen();
            }

            //Update camera
            cam.Update();
            //physics.Update(player);
            player.Update();

            //Update previous keyboard state
            prevks = ks;
            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, cam.ViewMatrix);

            //Draw Player
            spriteBatch.Draw(player.texture, new Vector2(player.x*tilesize, player.y*tilesize));

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
                        switch (player.currentRoom.map[i, j])
                        {
                            case 1:
                                spriteBatch.Draw(sWall, new Vector2(i * tilesize, j * tilesize));
                                break;
                            case 2:
                                spriteBatch.Draw(sLockDoor, new Vector2(i * tilesize, j * tilesize));
                                break;
                            case 3:
                                spriteBatch.Draw(sLockDoorV, new Vector2(i * tilesize, j * tilesize));
                                break;
                            case 4:
                                spriteBatch.Draw(sKey, new Vector2(i * tilesize, j * tilesize));
                                break;
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

            //Draw hitboxes
            spriteBatch.Draw(hbtex, player.boundingBox, Color.White);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
