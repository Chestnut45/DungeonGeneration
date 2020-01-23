﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

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
        List<Wall> walls = new List<Wall>();
        List<Door> doors = new List<Door>();
        List<Chest> chests = new List<Chest>();

        //Game objects
        Player player = new Player();
        Physics physics = new Physics();

        //Textures
        Texture2D sWall, sPlayer, sLockDoor, sLockDoorV, sKey, sChestClosed, sChestOpen;
        Texture2D hbtex, hbtexWall;

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
            sChestClosed = Content.Load<Texture2D>("sChestClosed");
            sChestOpen = Content.Load<Texture2D>("sChestOpen");

            //Hitbox shit
            //Make one pixel of color and transparency
            Byte transparency_amount = 100; //0 transparent; 255 opaque
            hbtex = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] c = new Color[1];
            c[0] = Color.FromNonPremultiplied(255, 0, 0, transparency_amount);
            hbtex.SetData<Color>(c);

            hbtexWall = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] c1 = new Color[1];
            c1[0] = Color.FromNonPremultiplied(0, 0, 255, transparency_amount);
            hbtexWall.SetData<Color>(c1);
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
                player.currentDungeon = generator.generateDungeon(25, false);
                player.currentRoom = player.currentDungeon[0]; //currentDungeon is just a 1d array of rooms that all belong to the current dungeon
                player.x = (player.currentRoom.map.GetLength(0) / 2) * tilesize;
                player.y = (player.currentRoom.map.GetLength(1) / 2) * tilesize;

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

            //After switching rooms, load the objects in the currentRoom's map
            if (player.pr != player.currentRoom)
            {
                //Clear lists for objects to be added to
                walls.Clear();
                doors.Clear();
                chests.Clear();

                //Load room objects
                for (int i = 0; i < player.currentRoom.map.GetLength(0); i++)
                {
                    for (int j = 0; j < player.currentRoom.map.GetLength(1); j++)
                    {
                        switch (player.currentRoom.map[i,j])
                        {
                            case 1:
                                //Create wall objects with proper values
                                walls.Add(new Wall(i * tilesize, j * tilesize, sWall));
                                //walls[walls.Count - 1].texture = sWall;
                                break;

                            case 2:
                                //Create normal locked doors
                                doors.Add(new Door(i * tilesize, j * tilesize));
                                doors[doors.Count - 1].texture = sLockDoor;
                                break;

                            case 3:
                                //Create vertical locked doors
                                doors.Add(new Door(i * tilesize, j * tilesize));
                                doors[doors.Count - 1].vertical = true;
                                doors[doors.Count - 1].texture = sLockDoorV;
                                break;

                            case 4:
                                //Create key chests
                                chests.Add(new Chest(i * tilesize, j * tilesize));
                                chests[chests.Count - 1].texture = sChestClosed;
                                chests[chests.Count - 1].contents = new Drop(0, 0, Drop.dropType.key); //Create a new drop of type key and make the chest "contain" it.
                                //Drop dormancy TODO?
                                break;
                        }
                    }
                }
            }
            player.pr = player.currentRoom; //After the variable is needed, set the previous room to the current room

            //Deal with physics
            if (ks.IsKeyDown(Keys.A))
            {
                player.xacc = -1;
            }
            if (ks.IsKeyDown(Keys.D))
            {
                player.xacc = 1;
            }
            if (!ks.IsKeyDown(Keys.D) && !ks.IsKeyDown(Keys.A))
            {
                player.xacc = 0;
            }
            if (ks.IsKeyDown(Keys.W) & !prevks.IsKeyDown(Keys.W))
            {
                player.yvel = -7;
            }

            if (ks.IsKeyDown(Keys.F) & !prevks.IsKeyDown(Keys.F))
            {
                graphics.ToggleFullScreen();
            }

            //Update other instances
            cam.Update();
            if (player.currentDungeon != null)
            {
                physics.Update(player, walls);
            }
            
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
            spriteBatch.Draw(player.texture, new Vector2(player.x, player.y));

            //Go through current room and draw tiles
            if (player.currentRoom != null)
            {
                foreach (Wall w in walls)
                {
                    spriteBatch.Draw(w.texture, new Vector2(w.X, w.Y));

                    //Draw hitboxes (debug)
                    spriteBatch.Draw(hbtexWall, w.boundingBox, Color.White);
                }

                foreach (Door d in doors)
                {
                    spriteBatch.Draw(d.texture, new Vector2(d.X, d.Y));
                }

                foreach (Chest c in chests)
                {
                    spriteBatch.Draw(c.texture, new Vector2(c.X, c.Y));
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
