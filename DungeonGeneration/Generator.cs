using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DungeonGeneration
{
    public class Generator
    {
        // ******** LEGEND ********
        // 0 = Air
        // 1 = Wall
        // 2 = Locked door (Horizontal)
        // 3 = Locked door (Vertical)

        //This code is kinda messy, will clean up at a later date
        int createdRooms;
        Room currentRoom;//, previousRoom;
        bool[] currentExits = new bool[4];
        public string seed;
        public int nx, ny;

        //Room maps are all 32x18 tiles
        public int[,] defaultMap = new int[32, 18];

        public void generateDefaultMap()
        {
            for (int i = 0; i < defaultMap.GetLength(0); i++)
            {
                for (int j = 0; j < defaultMap.GetLength(1); j++)
                {
                    if (i == 0 || i == defaultMap.GetLength(0) - 1 || j == 0 || j == defaultMap.GetLength(1) - 1)
                    {
                        defaultMap[i, j] = 1;
                    } else
                    {
                        defaultMap[i, j] = 0;
                    }
                }
            }
        }

        public int countRooms(Room[] roomArray)
        {
            createdRooms = 0;
            foreach (Room r in roomArray)
            {
                if (r != null)
                {
                    createdRooms++;
                }
            }
            return createdRooms;
        }

        public bool areAllTrue(bool[] b)
        {
            foreach (bool x in b)
            {
                if (x == false)
                {
                    return false;
                }
            }
            return true;
        }

        public int cycleIndex(int x)
        {
            x++;
            if (x>3)
            {
                return 0;
            }
            return x;
        }

        public bool canCreateRoom(Room[] rms, int x, int y)
        {
            foreach (Room r in rms)
            {
                if (r != null)
                {
                    if (r.mx == x & r.my == y)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void calcNXY(int ei)
        {
            int checkMapXOffset = 0;
            int checkMapYOffset = 0;

            switch (ei)
            {
                case 0:
                    //Left
                    checkMapXOffset = -1;
                    break;
                case 1:
                    //Up
                    checkMapYOffset = -1;
                    break;
                case 2:
                    //Right
                    checkMapXOffset = 1;
                    break;
                case 3:
                    //Down
                    checkMapYOffset = 1;
                    break;
            }

            //Loop through every room to check if the mx and my values match
            nx = currentRoom.mx + checkMapXOffset;
            ny = currentRoom.my + checkMapYOffset;
        }

        public Room[] generateDungeon(int targetrooms, bool useRandomSeed)
        {
            //Seed code
            if(useRandomSeed)
            {
                seed = DateTime.Now.Millisecond.ToString();
            } else
            {
                seed = "01234567";
            }

            //Random number generator
            Random rng = new Random(seed.GetHashCode());

            Room[] rooms = new Room[targetrooms]; //empty array of rooms
            List<Room> notDeadRooms = new List<Room>();

            //Create first room
            rooms[0] = new Room(0, 0);
            currentRoom = rooms[0];
            notDeadRooms.Add(currentRoom);

            //Count rooms that are already created
            countRooms(rooms);

            //Check if required number of rooms is met
            if (createdRooms == targetrooms)
            {
                //Done

            } else
            {
                //Loop this area until another room is created
                while (createdRooms < targetrooms)
                {
                    //Choose random room to check
                    //int roomToCheck = rng.Next(notDeadRooms.Count); //This code clusters to middle

                    //int roomToCheck = notDeadRooms.Count-1; //This always tries to use the latest room created to branch from if possible
                    int roomToCheck;
                    int r1 = rng.Next(notDeadRooms.Count);
                    int r2 = rng.Next(notDeadRooms.Count);
                    int r3 = rng.Next(notDeadRooms.Count);
                    roomToCheck = Math.Max(r1, Math.Max(r2, r3));
                    currentRoom = notDeadRooms[roomToCheck];

                    //Check free exits
                    if (areAllTrue(currentRoom.exits))
                    {
                        notDeadRooms.RemoveAt(roomToCheck);
                    } else
                    {
                        int exitIndex = rng.Next(0, 3); // random exit index
                        currentExits[exitIndex] = true;

                        //First time calc
                        calcNXY(exitIndex);

                        bool canCreateRoomHere = false;
                        if (canCreateRoom(rooms, nx, ny))
                        {
                            canCreateRoomHere = true;
                        } else
                        {
                            exitIndex = cycleIndex(exitIndex);
                            calcNXY(exitIndex);
                            if (canCreateRoom(rooms, nx, ny))
                            {
                                canCreateRoomHere = true;
                            }
                            else
                            {
                                exitIndex = cycleIndex(exitIndex);
                                calcNXY(exitIndex);
                                if (canCreateRoom(rooms, nx, ny))
                                {
                                    canCreateRoomHere = true;
                                }
                                else
                                {
                                    exitIndex = cycleIndex(exitIndex);
                                    calcNXY(exitIndex);
                                    if (canCreateRoom(rooms, nx, ny))
                                    {
                                        canCreateRoomHere = true;
                                    }
                                    else
                                    {
                                        //deadroom
                                        notDeadRooms.RemoveAt(roomToCheck);
                                    }
                                }
                            }
                        }

                        //Check flag for creating room
                        if (canCreateRoomHere)
                        {
                            //create exit in current room
                            currentRoom.exits[exitIndex] = true;
                            rooms[createdRooms] = new Room(nx, ny);
                            currentRoom.adjacentRooms[exitIndex] = rooms[createdRooms];

                            //Invert index for newly created room
                            switch (exitIndex)
                            {
                                case 0:
                                    exitIndex = 2;
                                    break;
                                case 1:
                                    exitIndex = 3;
                                    break;
                                case 2:
                                    exitIndex = 0;
                                    break;
                                case 3:
                                    exitIndex = 1;
                                    break;
                            }

                            //Create exit and adj room ids for newly created room
                            rooms[createdRooms].exits[exitIndex] = true;
                            rooms[createdRooms].adjacentRooms[exitIndex] = currentRoom;

                            notDeadRooms.Add(rooms[createdRooms]);

                            //recount rooms
                            countRooms(rooms);

                            //AT THIS POINT, THE EXITS ARE CREATED, AND ROOMS ARE PROPERLY LINKED
                        }
                        else
                        {
                            //You should only arrive here if you are in a room with at least one non-exit, but it is surrounded by other rooms in all directions.
                        }
                    }
                }
            }

            //Have all adjacent rooms loaded for screen transitions OwO
            //Or not

            //Populate
            populateRooms(rooms, rng);

            //Return all rooms
            return rooms;
        }

        public void populateRooms(Room[] rooms, Random rng)
        {
            //TODO: Build actual objects for entire dungeon;

            //Generate the template
            generateDefaultMap();

            int l = 0;
            foreach (Room r in rooms)
            {
                for (int i = 0; i < r.map.GetLength(0); i++)
                {
                    for (int j = 0; j < r.map.GetLength(1); j++)
                    {
                        r.map[i, j] = defaultMap[i, j];
                    }
                }
                //r.map = defaultMap;
                r.id = l;
                l++;
            }

            for (int i = 0; i < rooms.Length; i++)
            {

                //Add (or rather subtract) exits
                if (rooms[i].exits[0]) //LEFT EXIT
                {
                    rooms[i].map[0, rooms[i].map.GetLength(1) - 2] = 0;
                    rooms[i].map[0, rooms[i].map.GetLength(1) - 3] = 0;
                }

                if (rooms[i].exits[1]) //UP EXIT
                {
                    rooms[i].map[(rooms[i].map.GetLength(0) / 2), 0] = 0;
                    rooms[i].map[(rooms[i].map.GetLength(0) / 2) - 1, 0] = 0;
                }

                if (rooms[i].exits[2]) //RIGHT
                {
                    rooms[i].map[rooms[i].map.GetLength(0) - 1, rooms[i].map.GetLength(1) - 2] = 0;
                    rooms[i].map[rooms[i].map.GetLength(0) - 1, rooms[i].map.GetLength(1) - 3] = 0;
                }

                if (rooms[i].exits[3]) //DOWN EXIT
                {
                    rooms[i].map[(rooms[i].map.GetLength(0) / 2), rooms[i].map.GetLength(1) - 1] = 0;
                    rooms[i].map[(rooms[i].map.GetLength(0) / 2) - 1, rooms[i].map.GetLength(1) - 1] = 0;
                }

                if (rooms[i] != null)
                {
                    Console.WriteLine("Room " + i + " at position: " + rooms[i].mx + "," + rooms[i].my);
                    Console.WriteLine("[{0}]", string.Join(", ", rooms[i].exits));
                }
            }

            //Lock rooms?
            lockRooms(rooms, rng);
        }

        public void lockRooms(Room[] rms, Random rng)
        {
            int n, index;
            Room a, b = null;
            List<Room> lockedRooms = new List<Room>();
            List<Room> multiExitRooms = new List<Room>();
            List<Room> keyRooms = new List<Room>();

            //Loop through every room, add rooms with only one exit to a list
            for (int i = 1; i < rms.Length; i++)
            {
                //for every single room except the entry room
                //Count number of exits
                n = countExits(rms[i]);

                //If only one exit, it can be locked
                if (n == 1)
                {
                    //For now just add to room list
                    //TODO: make it a random chance to lock the room. It's no fun if every single dead end is locked.
                    //TODO: make sure you never lock more rooms than we have available key rooms
                    lockedRooms.Add(rms[i]);
                } else
                {
                    multiExitRooms.Add(rms[i]);
                }
            }

            //Perform the check to make sure there is enough key rooms for locked rooms
            while (lockedRooms.Count > multiExitRooms.Count - 6) //This magic number is arbitrary and means almost nothing, it's the amount of rooms that will NOT have a key.
            {
                lockedRooms.RemoveAt(rng.Next(0, lockedRooms.Count - 1));
            }

            //Perform the actual locking of single exit rooms
            foreach (Room r in lockedRooms)
            {
                //Find out adjacent room, add lock to the path leading into current room
                index = -1;
                b = null;
                while (b == null)
                {
                    index++;
                    if (r.adjacentRooms[index] != null)
                    {
                        b = r.adjacentRooms[index];
                    }
                }

                //When the above loop is over, you'll have the adjacent room in b
                //And you will also have the index of where the adjacent room is
                switch (index)
                {
                    case 0:
                        //Left exit (Lock Right of Adjacent)
                        b.map[b.map.GetLength(0) - 1, b.map.GetLength(1) - 3] = 2;
                        break;

                    case 1:
                        //Up exit (Lock Bottom of Adjacent)
                        b.map[(b.map.GetLength(0) / 2) - 1, b.map.GetLength(1) - 1] = 3;
                        break;

                    case 2:
                        //Right exit (Lock Left of Adjacent)
                        b.map[0, b.map.GetLength(1) - 3] = 2;
                        break;

                    case 3:
                        //Bottom exit (Lock Top of Adjacent)
                        b.map[(b.map.GetLength(0) / 2) - 1, 0] = 3;
                        break;
                }
            }

            //Reverse list to make keys further away
            multiExitRooms.Reverse();

            //Go through multi exit rooms until you have placed enough keys to unlock every door
            foreach (Room mer in multiExitRooms)
            {
                if (keyRooms.Count >= lockedRooms.Count)
                {
                    break;
                }
                keyRooms.Add(mer);
            }

            //Go through every key room and generate a key based on the exits
            foreach (Room kr in keyRooms)
            {
                //TODO: Work out logic of where to place key

            }
        }

        public int countExits (Room r)
        {
            int ind = 0;
            int count = 0;

            while (ind < 4)
            {
                if (r.exits[ind])
                {
                    count++;
                }
                ind++;
            }

            return count;
        }
    }
}