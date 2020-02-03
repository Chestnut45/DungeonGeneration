using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DungeonGeneration
{
    public class Generator
    {
        // ******** LEGEND ********
        // 0 = Air
        // 1 = Wall
        // 2 = Locked door (Horizontal)
        // 3 = Locked door (Vertical)
        // 4 = Key
        // 5 = Ladder

        //This code is kinda messy, will clean up at a later date
        int createdRooms;
        Room currentRoom;//, previousRoom;
        bool[] currentExits = new bool[4];
        public string seed;
        public int nx, ny;
        int inc;
        bool platform;

        //Room map size
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
                seed = ((DateTime.Now.Millisecond + 1) * (DateTime.Now.Second + 1) * (DateTime.Now.Minute + 1)).ToString(); //Better seed
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
                    //roomToCheck = Math.Max(r1, Math.Max(r2, r3)); //max of 3 rolls, not clustered enough?
                    roomToCheck = Math.Max(r2, r3);
                    currentRoom = notDeadRooms[roomToCheck];

                    //Check free exits
                    if (areAllTrue(currentRoom.exits))
                    {
                        notDeadRooms.RemoveAt(roomToCheck);
                    } else
                    {
                        int exitIndex = rng.Next(0, 4); // random exit index
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
                            //This is where to put the code to randomize exit locations.
                            //create exit in current room
                            int el = 0;
                            int vel = 0;
                            switch (exitIndex)
                            {
                                case 0:
                                    //left
                                    el = rng.Next(1, 4);
                                    currentRoom.exitLocation[0] = el;
                                    break;
                                case 1:
                                    //Up
                                    vel = rng.Next(1, 3);
                                    currentRoom.exitLocation[1] = vel;
                                    break;
                                case 2:
                                    //Right
                                    el = rng.Next(1, 4);
                                    currentRoom.exitLocation[2] = el;
                                    break;
                                case 3:
                                    //Down
                                    vel = rng.Next(1, 3);
                                    currentRoom.exitLocation[3] = vel;
                                    break;
                            }
                            currentRoom.exits[exitIndex] = true; //Create the exit from the current room
                            rooms[createdRooms] = new Room(nx, ny); //Create a new room with newx and newy
                            currentRoom.adjacentRooms[exitIndex] = rooms[createdRooms]; //Make the newly created room a reference in the current rooms adjacents

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

                            //Add proper exit location to newly created room
                            switch (exitIndex)
                            {
                                case 0:
                                    //left
                                    rooms[createdRooms].exitLocation[0] = el;
                                    break;
                                case 1:
                                    //Up
                                    rooms[createdRooms].exitLocation[1] = vel;
                                    break;
                                case 2:
                                    //Right
                                    rooms[createdRooms].exitLocation[2] = el;
                                    break;
                                case 3:
                                    //Down
                                    rooms[createdRooms].exitLocation[3] = vel;
                                    break;
                            }

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
                    switch (rooms[i].exitLocation[0])
                    {
                        case 1: //Normal bottom exit
                            rooms[i].map[0, rooms[i].map.GetLength(1) - 2] = 0;
                            rooms[i].map[0, rooms[i].map.GetLength(1) - 3] = 0;
                            break;
                        case 2:
                            //Middle exit
                            rooms[i].map[0, rooms[i].map.GetLength(1) / 2] = 0;
                            rooms[i].map[0, (rooms[i].map.GetLength(1) / 2) - 1] = 0;
                            break;
                        case 3:
                            //Top exit
                            rooms[i].map[0, 1] = 0;
                            rooms[i].map[0, 2] = 0;
                            break;
                    }
                }

                if (rooms[i].exits[1]) //UP EXIT
                {
                    switch (rooms[i].exitLocation[1])
                    {
                        case 1:
                            //Left side
                            //Exit
                            rooms[i].map[12, 0] = 0;
                            rooms[i].map[10, 0] = 0;
                            rooms[i].map[11, 0] = 0;

                            //Platform
                            rooms[i].map[12, rooms[i].map.GetLength(1) / 2] = 1;
                            rooms[i].map[10, rooms[i].map.GetLength(1) / 2] = 1;
                            rooms[i].map[11, rooms[i].map.GetLength(1) / 2] = 1;

                            //Ladder
                            inc = 0;
                            while (rooms[i].map[11, 0 + inc] != 1)
                            {
                                rooms[i].map[11, 0 + inc] = 5;
                                inc++;
                            }

                            //Choose right or left ladder based on upper exit x pos
                            platform = true;
                            if (rng.Next(0, 6) == 0)
                            {
                                //Left Ladder
                                inc = 0;
                                while (rooms[i].map[9, (rooms[i].map.GetLength(1) / 2) + inc] != 1)
                                {
                                    rooms[i].map[9, (rooms[i].map.GetLength(1) / 2) + inc] = 5;
                                    inc++;
                                }
                                platform = false;
                            }
                            else if (rng.Next(0, 6) == 0)
                            {
                                //Right ladder
                                inc = 0;
                                while (rooms[i].map[13, (rooms[i].map.GetLength(1) / 2) + inc] != 1)
                                {
                                    rooms[i].map[13, (rooms[i].map.GetLength(1) / 2) + inc] = 5;
                                    inc++;
                                }
                                platform = false;
                            }

                            //Platform logic
                            if (platform)
                            {
                                if (rng.Next(0, 2) == 0)
                                {
                                    //Left
                                    rooms[i].map[6, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[4, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[5, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                }
                                else
                                {
                                    //Right
                                    rooms[i].map[18, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[16, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[17, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                }
                            }
                            break;
                        case 2:
                            //Right side
                            //Exit
                            rooms[i].map[19, 0] = 0;
                            rooms[i].map[20, 0] = 0;
                            rooms[i].map[21, 0] = 0;

                            //Platform
                            rooms[i].map[19, rooms[i].map.GetLength(1) / 2] = 1;
                            rooms[i].map[20, rooms[i].map.GetLength(1) / 2] = 1;
                            rooms[i].map[21, rooms[i].map.GetLength(1) / 2] = 1;

                            //Ladder
                            inc = 0;
                            while (rooms[i].map[20, 0 + inc] != 1)
                            {
                                rooms[i].map[20, 0 + inc] = 5;
                                inc++;
                            }

                            //Choose right or left ladder based on upper exit x pos
                            platform = true;
                            if (rng.Next(0, 6) == 0)
                            {
                                //Left Ladder
                                inc = 0;
                                while (rooms[i].map[18, (rooms[i].map.GetLength(1) / 2) + inc] != 1)
                                {
                                    rooms[i].map[18, (rooms[i].map.GetLength(1) / 2) + inc] = 5;
                                    inc++;
                                }
                                platform = false;
                            }
                            else if (rng.Next(0, 6) == 0)
                            {
                                //Right ladder
                                inc = 0;
                                while (rooms[i].map[22, (rooms[i].map.GetLength(1) / 2) + inc] != 1)
                                {
                                    rooms[i].map[22, (rooms[i].map.GetLength(1) / 2) + inc] = 5;
                                    inc++;
                                }
                                platform = false;
                            }

                            //Platform logic
                            if (platform)
                            {
                                if (rng.Next(0, 2) == 0)
                                {
                                    //Left
                                    rooms[i].map[13, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[14, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[15, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                }
                                else
                                {
                                    //Right
                                    rooms[i].map[25, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[26, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                    rooms[i].map[27, rooms[i].map.GetLength(1) / 2 + rooms[i].map.GetLength(1) / 4] = 1;
                                }
                            }
                            break;
                    }
                }

                if (rooms[i].exits[2]) //RIGHT
                {
                    //Choose random y position
                    switch (rooms[i].exitLocation[2])
                    {
                        case 1: //Normal bottom exit
                            rooms[i].map[rooms[i].map.GetLength(0) - 1, rooms[i].map.GetLength(1) - 2] = 0;
                            rooms[i].map[rooms[i].map.GetLength(0) - 1, rooms[i].map.GetLength(1) - 3] = 0;
                            break;
                        case 2:
                            //Middle exit
                            rooms[i].map[rooms[i].map.GetLength(0) - 1, rooms[i].map.GetLength(1) / 2] = 0;
                            rooms[i].map[rooms[i].map.GetLength(0) - 1, (rooms[i].map.GetLength(1) / 2) - 1] = 0;
                            break;
                        case 3:
                            //Top exit
                            rooms[i].map[rooms[i].map.GetLength(0) - 1, 1] = 0;
                            rooms[i].map[rooms[i].map.GetLength(0) - 1, 2] = 0;
                            break;
                    }
                }

                if (rooms[i].exits[3]) //DOWN EXIT
                {
                    //Down exit
                    switch (rooms[i].exitLocation[3])
                    {
                        case 1:
                            //Left
                            rooms[i].map[10, rooms[i].map.GetLength(1) - 1] = 0;
                            rooms[i].map[11, rooms[i].map.GetLength(1) - 1] = 0;
                            rooms[i].map[12, rooms[i].map.GetLength(1) - 1] = 0;
                            break;
                        case 2:
                            //Right
                            rooms[i].map[19, rooms[i].map.GetLength(1) - 1] = 0;
                            rooms[i].map[20, rooms[i].map.GetLength(1) - 1] = 0;
                            rooms[i].map[21, rooms[i].map.GetLength(1) - 1] = 0;
                            break;
                    }
                }

                if (rooms[i] != null)
                {
                    Console.WriteLine("Room " + i + " at position: " + rooms[i].mx + "," + rooms[i].my);
                    Console.WriteLine("[{0}]", string.Join(", ", rooms[i].exits));
                }
            }

            //Lock rooms?
            lockRooms(rooms, rng);

            //TODO: add loadingzones......... hmm
            for (int i = 0; i < rooms.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (rooms[i].exitLocation[j] != 0)
                    {
                        switch (j)
                        {
                            case 0:
                                //Left exit
                                switch (rooms[i].exitLocation[j])
                                {
                                    case 1:
                                        //Bottom level left exit
                                        rooms[i].loadingZones[j] = new Rectangle(0, (rooms[i].map.GetLength(1) - 3) * 8, 8, 16);
                                        break;
                                    case 2:
                                        //Middle level left exit
                                        rooms[i].loadingZones[j] = new Rectangle(0, ((rooms[i].map.GetLength(1) / 2) - 1) * 8, 8, 16);
                                        break;
                                    case 3:
                                        //Top level left exit
                                        rooms[i].loadingZones[j] = new Rectangle(0, 8, 8, 16);
                                        break;
                                }
                                break;
                            case 1:
                                //Up exit
                                switch (rooms[i].exitLocation[j])
                                {
                                    case 1:
                                        //Left up exit
                                        rooms[i].loadingZones[j] = new Rectangle(10 * 8, 0, 24, 8);
                                        break;
                                    case 2:
                                        //Right up exit
                                        rooms[i].loadingZones[j] = new Rectangle(19 * 8, 0, 24, 8);
                                        break;
                                }
                                break;
                            case 2:
                                //Right exit
                                switch (rooms[i].exitLocation[j])
                                {
                                    case 1:
                                        //Bottom level right exit
                                        rooms[i].loadingZones[j] = new Rectangle((rooms[i].map.GetLength(0) - 1) * 8, (rooms[i].map.GetLength(1) - 3) * 8, 8, 16);
                                        break;
                                    case 2:
                                        //Middle level right exit
                                        rooms[i].loadingZones[j] = new Rectangle((rooms[i].map.GetLength(0) - 1) * 8, ((rooms[i].map.GetLength(1) / 2) - 1) * 8, 8, 16);
                                        break;
                                    case 3:
                                        //Top level right exit
                                        rooms[i].loadingZones[j] = new Rectangle((rooms[i].map.GetLength(0) - 1) * 8, 8, 8, 16);
                                        break;
                                }
                                break;
                            case 3:
                                //Down exit
                                switch (rooms[i].exitLocation[j])
                                {
                                    case 1:
                                        //Left down exit
                                        rooms[i].loadingZones[j] = new Rectangle(10 * 8, (rooms[i].map.GetLength(1) - 1) * 8, 24, 8);
                                        break;
                                    case 2:
                                        //Right down exit
                                        rooms[i].loadingZones[j] = new Rectangle(19 * 8, (rooms[i].map.GetLength(1) - 1) * 8, 24, 8);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        public void lockRooms(Room[] rms, Random rng)
        {
            int n, index;
            Room b = null;
            List<Room> lockedRooms = new List<Room>();
            List<Room> singleExitRooms = new List<Room>();
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
                    singleExitRooms.Add(rms[i]);
                } else
                {
                    multiExitRooms.Add(rms[i]);
                }
            }

            //Add some amount of single exit rooms to locked rooms, the rest are for keys
            foreach (Room r in singleExitRooms)
            {
                if (rng.Next(0,10) > -1)
                {
                    lockedRooms.Add(r);
                }
            }

            //Remove lockedrooms from singleexitrooms
            foreach (Room r in lockedRooms)
            {
                singleExitRooms.Remove(r);
            }

            //Perform the check to make sure there is enough key rooms for locked rooms
            //This will probably NEVER happen in the life of the game, but it needs to be here on that off chance.
            while (lockedRooms.Count > multiExitRooms.Count)
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
                        switch (b.exitLocation[2])
                        {
                            case 1:
                                //Ground level
                                b.map[b.map.GetLength(0) - 1, b.map.GetLength(1) - 3] = 2;
                                break;
                            case 2:
                                //middle
                                b.map[b.map.GetLength(0) - 1, (b.map.GetLength(1) / 2) - 1] = 2;
                                break;
                            case 3:
                                //Top
                                b.map[b.map.GetLength(0) - 1, 1] = 2;
                                break;
                        }
                        break;

                    case 1:
                        //Up exit (Lock Bottom of Adjacent)
                        switch (b.exitLocation[3])
                        {
                            case 1:
                                //Left
                                b.map[10, b.map.GetLength(1) - 1] = 3;
                                break;
                            case 2:
                                //Right
                                b.map[19, b.map.GetLength(1) - 1] = 3;
                                break;
                        }
                        break;

                    case 2:
                        //Right exit (Lock Left of Adjacent)
                        switch (b.exitLocation[0])
                        {
                            case 1:
                                //Ground level
                                b.map[0, b.map.GetLength(1) - 3] = 2;
                                break;
                            case 2:
                                //Middle
                                b.map[0, (b.map.GetLength(1) / 2) - 1] = 2;
                                break;
                            case 3:
                                //Top
                                b.map[0, 1] = 2;
                                break;
                        }
                        break;

                    case 3:
                        //Bottom exit (Lock Top of Adjacent)
                        switch (b.exitLocation[1])
                        {
                            case 1:
                                //Left
                                b.map[10, 0] = 3;
                                break;
                            case 2:
                                //Right
                                b.map[19, 0] = 3;
                                break;
                        }
                        break;
                }
            }

            //First go through single exit rooms, favorable for keys
            foreach (Room ser in singleExitRooms)
            {
                if (keyRooms.Count >= lockedRooms.Count)
                {
                    break;
                }
                keyRooms.Add(ser);
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
                placeKey(kr, rng);
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

        public void placeKey(Room kr, Random rng)
        {
            //Key placement logic
            if (kr.exits[0] && !kr.exits[2]) // Left and !right
                kr.map[kr.map.GetLength(0) - 3, kr.map.GetLength(1) - 2] = 4;
            if (!kr.exits[0] && kr.exits[2]) // !Left and right
                kr.map[2, kr.map.GetLength(1) - 2] = 4;
            if ((kr.exits[0] && kr.exits[2]) || !kr.exits[0] && !kr.exits[2]) // Left AND right || !left AND !right exits
            {
                if (!kr.exits[3]) //Down exit exists
                {
                    kr.map[(kr.map.GetLength(0) / 2) - 1, kr.map.GetLength(1) - 2] = 4;
                }
                else // Down exit does not exist
                {
                    if (kr.exits[1]) // Up exit exists
                    {
                        //Right or left platform
                        if (rng.Next(0, 2) == 0) //If this looks weird, the first value of rng.Next is inclusive, but the second value is exclusive. DUMB.
                        {
                            //Left platform
                            kr.map[1, kr.map.GetLength(1) - 4] = 1;
                            kr.map[2, kr.map.GetLength(1) - 4] = 1;
                            kr.map[1, kr.map.GetLength(1) - 5] = 4;
                        }
                        else
                        {
                            //Right platform
                            kr.map[kr.map.GetLength(0) - 2, kr.map.GetLength(1) - 4] = 1;
                            kr.map[kr.map.GetLength(0) - 3, kr.map.GetLength(1) - 4] = 1;
                            kr.map[kr.map.GetLength(0) - 2, kr.map.GetLength(1) - 5] = 4;
                        }
                    }
                    else // Up exit does not exist
                    {
                        //Middle platform
                        kr.map[kr.map.GetLength(0) / 2, kr.map.GetLength(1) - 5] = 1;
                        kr.map[(kr.map.GetLength(0) / 2) - 1, kr.map.GetLength(1) - 5] = 1;
                        kr.map[(kr.map.GetLength(0) / 2) + 1, kr.map.GetLength(1) - 5] = 1;
                        kr.map[(kr.map.GetLength(0) / 2), kr.map.GetLength(1) - 6] = 4;
                    }
                }
            }
        }
    }
}

//TODO: somehow ensure some rooms will be locked, can't have a dungeon with 0 locked rooms.
//Possible solution... reject dungeons generated with less than optimal lock amounts. (seems maybe cheaty) (could cause inconsistent generation times

//TODO: Generate "Sections" that are locked off? - might require BIG rework of how dungeons work / are generated