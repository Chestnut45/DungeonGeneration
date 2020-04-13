using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.ViewportAdapters;

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
        List<Ladder> ladders = new List<Ladder>();
        List<Rectangle> loadingZones = new List<Rectangle>();

        //Game objects
        Player player = new Player();
        Physics physics = new Physics();

        //Textures
        Texture2D sWall, sPlayer, sLockDoor, sLockDoorV, sKey, sChestClosed, sChestOpen, sLadder;
        Texture2D hbtex, hbtexWall, hbtexLoadingZone;

        //State system
        enum GameState
        {
            MainMenu,
            Gameplay,
            Pause
        }

        GameState state = GameState.MainMenu;

        //TODO: Set up game states, begin in main menu state, set up functioning main menu and loading functions.

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
            sLadder = Content.Load<Texture2D>("sLadder");

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

            hbtexLoadingZone = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] c2 = new Color[1];
            c2[0] = Color.FromNonPremultiplied(0, 255, 0, transparency_amount);
            hbtexLoadingZone.SetData<Color>(c2);
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

            ks = Keyboard.GetState();

            // Main game loop
            switch (state)
            {
                case GameState.MainMenu:
                    //Main menu code TODO: make the fucking menu dipshit
                    if (ks.IsKeyDown(Keys.P) & !prevks.IsKeyDown(Keys.P))
                    {
                        //change this to trigger on startGame();
                        player.currentDungeon = generator.generateDungeon(25, true);
                        player.currentRoom = player.currentDungeon[0]; //currentDungeon is just a 1d array of rooms that all belong to the current dungeon
                        player.x = (player.currentRoom.map.GetLength(0) / 2) * tilesize;
                        player.y = ((player.currentRoom.map.GetLength(1) / 2) + 1) * tilesize;

                        state = GameState.Gameplay;

                        //TODO: Build dungeon room objects. Load all in memory or one room / dungeon at a time?
                    }
                    break;

                case GameState.Gameplay:
                    //Gameplay code
                    //Change rooms
                    if (ks.IsKeyDown(Keys.P) & !prevks.IsKeyDown(Keys.P))
                    {
                        player.currentDungeon = generator.generateDungeon(25, true);
                        player.currentRoom = player.currentDungeon[0]; //currentDungeon is just a 1d array of rooms that all belong to the current dungeon
                        player.x = (player.currentRoom.map.GetLength(0) / 2) * tilesize;
                        player.y = ((player.currentRoom.map.GetLength(1) / 2) + 1) * tilesize;

                        state = GameState.Gameplay;

                        //TODO: Build dungeon room objects. Load all in memory or one room / dungeon at a time?
                    }
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
                        ladders.Clear();
                        loadingZones.Clear();

                        //Load loading zones
                        for (int i = 0; i < 4; i++)
                        {
                            if (player.currentRoom.loadingZones[i] != null)
                            {
                                loadingZones.Add(player.currentRoom.loadingZones[i]);
                            }
                        }

                        //Load room objects
                        for (int i = 0; i < player.currentRoom.map.GetLength(0); i++)
                        {
                            for (int j = 0; j < player.currentRoom.map.GetLength(1); j++)
                            {
                                switch (player.currentRoom.map[i, j])
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
                                        chests[chests.Count - 1].contents = new Drop(0, 0, Drop.dropType.key); //This code is untested
                                                                                                               //Drop dormancy TODO?
                                        break;
                                    case 5:
                                        //Create normal locked doors
                                        ladders.Add(new Ladder(i * tilesize, j * tilesize));
                                        ladders[ladders.Count - 1].texture = sLadder;
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
                    if (ks.IsKeyDown(Keys.W) && !prevks.IsKeyDown(Keys.W) && player.grounded)
                    {
                        player.yvel = -4;
                    }
                    break;

                case GameState.Pause:
                    //Pause code

                    break;
            }

            //Debug fullscreen
            if (ks.IsKeyDown(Keys.F) && !prevks.IsKeyDown(Keys.F))
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
                cam.scale = 7.5f;
                graphics.ToggleFullScreen();
            }

            //Update camera
            cam.Update(player, graphics);

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

            // TODO: draw based on gamestates
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, cam.ViewMatrix);

            switch (state)
            {
                case GameState.MainMenu:
                    //Menu drawing

                    break;
                case GameState.Gameplay:
                    //Gameplay drawing

                    //Go through current room and draw tiles
                    if (player.currentRoom != null)
                    {
                        foreach (Wall w in walls)
                        {
                            spriteBatch.Draw(w.texture, new Vector2(w.X, w.Y));

                            //Draw hitboxes (debug)
                            spriteBatch.Draw(hbtexWall, w.boundingBox, Color.White);
                        }

                        foreach (Chest c in chests)
                        {
                            spriteBatch.Draw(c.texture, new Vector2(c.X, c.Y));
                        }

                        foreach (Ladder l in ladders)
                        {
                            spriteBatch.Draw(l.texture, new Vector2(l.X, l.Y));
                        }

                        //Draw Player
                        spriteBatch.Draw(player.texture, new Vector2(player.x, player.y));

                        foreach (Door d in doors)
                        {
                            spriteBatch.Draw(d.texture, new Vector2(d.X, d.Y));
                        }

                        //Draw loading zones
                        foreach (Rectangle z in loadingZones)
                        {
                            spriteBatch.Draw(hbtexLoadingZone, z, Color.White);
                        }
                    }
                    break;
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
