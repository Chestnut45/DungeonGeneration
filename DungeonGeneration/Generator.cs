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
        //This code is kinda messy, will clean up at a later date
        int createdRooms;
        Room currentRoom, previousRoom;
        bool[] currentExits = new bool[4];
        public string seed;
        public int nx, ny;

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

        public void generateDungeon(int targetrooms, bool useRandomSeed)
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
                        int exitIndex = rng.Next(0, 3); // random exit array
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

            //Populate
            populateRooms(rooms);
        }

        public void populateRooms(Room[] rooms)
        {
            //Check exit types and fill rooms

            //Display all room data
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i] != null)
                {
                    Console.WriteLine("Room " + i + " at position: " + rooms[i].mx + "," + rooms[i].my);
                    Console.WriteLine("[{0}]", string.Join(", ", rooms[i].exits));
                }
            }
        }
    }
}