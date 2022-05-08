using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cinemachine;
using Freya;

[RequireComponent(typeof(SpawnManager))]
public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon data")]
    [Range(3,25)] public int roomWidth;
    [Range(3,25)] public int roomHeight;
    [Range(2,100)] public int roomCount;

    [Range(1,100)] public int enemyChance;
    [Range(1,100)] public int weaponChance;
    [Range(1,100)] public int pickUpChance;
    [Range(1,100)] public int nothingChance;

    [Header("Random")]
    public string seed;
    public bool useRandomSeed;
    [HideInInspector] private System.Random prng;

    [Header("Room objects")]
    public GameObject roomPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject floorPrefab;
    public GameObject exitPrefab;
    public GameObject outsidePrefab;

    [Header("Spawn manager")]
    [SerializeField] private SpawnManager spawnManager;


    [Header("Debug")]
    [SerializeField] private List<Vector2> spawnPos = new List<Vector2>();
    [SerializeField] private List<GameObject> spawnObj = new List<GameObject>();
    
    private GameObject enemyContainer;
    private GameObject weaponContainer;
    private GameObject pickupContainer;

    private Vector3 initialPosition;


    [SerializeField] private RoomComponent startingRoom;
    [SerializeField] private RoomComponent endRoom;
    [SerializeField] private GameObject exitDungeon;
    [SerializeField] private List<RoomComponent> rooms = new List<RoomComponent>();
    private List<RoomComponent> roomsChecked = new List<RoomComponent>();
    private Dictionary<Vector2, RoomComponent> indexDict = new Dictionary<Vector2, RoomComponent>();
    [SerializeField] private List<Vector2> roomSpawnPos = new List<Vector2>();
    [SerializeField] private List<Vector2> roomIndexes = new List<Vector2>();

    private bool doCheck = false;

    public Vector3 InitialPosition { get => initialPosition; }

    void CreateRandomGenerator()
    {
        if (useRandomSeed)
        {
            // seed = Time.time.ToString();
            seed = seed + System.DateTime.Now.ToString();
        }
        // Debug.Log("Seed = " + seed);
        prng = new System.Random(seed.GetHashCode());
    }

    // Start is called before the first frame update
    void Update()
    {
        if (doCheck)
        {
            int enemiesInDungeon = enemyContainer.transform.childCount;
            if (enemiesInDungeon == 0)
            {
                Debug.Log("No enemies left");
                doCheck = false;
                OpenExit();
            }
        }
    }

    private void OpenExit()
    {
        exitDungeon.GetComponent<ExitController>().Open();
    }

    public void StartDungeon()
    {
        CreateRandomGenerator();

        // Transform changes to accumulated
        weaponChance = enemyChance+weaponChance;
        pickUpChance = enemyChance+pickUpChance;
        nothingChance = enemyChance+nothingChance;


        spawnManager = gameObject.GetComponent<SpawnManager>();
        spawnManager.SetDungeonGenerator(this);

        float initialTime = Time.realtimeSinceStartup;
        CreateDungeon();

        endRoom = GetFurthestRoom();
        exitDungeon = endRoom.PlaceExit();

        SpawnObjects();
        
        float deltaTime = Time.realtimeSinceStartup - initialTime;
        // Debug.Log("Time building dungeon: " + deltaTime.ToString("f6") + " seconds");

        PlaceOutside();

        // GameReferences.gameManager.PlacePlayer();

        doCheck = true;
    }

    private RoomComponent GetFurthestRoom()
    {
        RoomComponent furthestRoom = startingRoom;
        RoomComponent tentativeFurthestRoom = startingRoom;

        float maxDist = 0f;
        float tentativeMaxDist = 0f;
        
        foreach (var room in rooms)
        {
            float dist = Vector2.Distance(startingRoom.transform.position, room.transform.position);

            if (dist > tentativeMaxDist)
            {
                tentativeMaxDist = dist;
                tentativeFurthestRoom = room;
            }

            // Fursthest must have only one door
            if ((dist > maxDist) && (room.GetNumberDoorsOpen() == 1))
            {
                maxDist = dist;
                furthestRoom = room;
            }
        }
        if (furthestRoom == startingRoom)
        {
            Debug.LogWarning("Failed getting fursthest room with 1 door. Using fursthest room with more doors");
            furthestRoom = tentativeFurthestRoom;
        }
        return furthestRoom;
    }

    private void PlaceOutside()
    {
        float maxTop    = float.MinValue;
        float maxBottom = float.MaxValue;
        float maxRight  = float.MinValue;
        float maxLeft   = float.MaxValue;

        foreach (var room in rooms)
        {
            float roomX = room.transform.position.x;
            float roomY = room.transform.position.y;
            if (roomY > maxTop)
                maxTop = roomY;
            if (roomY < maxBottom)
                maxBottom = roomY;
            if (roomX > maxRight)
                maxRight = roomX;
            if (roomX < maxLeft)
                maxLeft = roomX;
        }

        Vector2 pos = new Vector2((maxRight+maxLeft)/2f, (maxTop+maxBottom)/2f);
        Vector2 size = new Vector2(maxRight-maxLeft+2*roomWidth, maxTop-maxBottom+2*roomHeight);
        // Vector2 size = new Vector2(maxRight-maxLeft, maxTop-maxBottom);

        // Debug.Log("Dungeon center: " + pos);
        // Debug.Log("Dungeon size: " + size);

        GameObject outside = Instantiate(outsidePrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        outside.transform.parent = this.transform;
        SpriteRenderer outsideSR = outside.GetComponent<SpriteRenderer>();
        outsideSR.size = size;
    }


    void AddRoom()
    {
        // Debug.Log("Add room");
        Vector2 index = SelectRandomSpawnPoint();
        var room = CreateRoom(index);
        room.CreateSpawnPoints();
    }

    void CreateDungeon()
    {
        startingRoom = CreateRoom(new Vector2(0, 0));
        initialPosition = startingRoom.transform.position;
        // startingRoom.CreateSpawnPoints();

        for (int count = 1; count < roomCount; count++)
        {
            AddRoom();
        }

        roomsChecked = new List<RoomComponent>();
        Queue<RoomComponent> roomQueue = new Queue<RoomComponent>();
        bool ok = OpenDoors(roomQueue);
        if (!ok)
        {
            Debug.LogError("ERROR OPEN DOOR");
        }

        foreach (var room in rooms)
        {
            room.CreateDoors();
            room.CreateWallCorners();
            room.CreateWallSides();
        }
    }

    private void SpawnObjects()
    {
        enemyContainer = new GameObject("Enemy Container");
        weaponContainer = new GameObject("Weapon Container");
        pickupContainer = new GameObject("Pickup Container");
        enemyContainer.transform.parent = this.transform;
        weaponContainer.transform.parent = this.transform;
        pickupContainer.transform.parent = this.transform;
        foreach (var pos in spawnPos)
        {
            int chance = GenerateRandomInt(0, nothingChance);
            GameObject container = null;
            GameObject obj = null;

            if (chance < enemyChance)
            {
                obj = spawnManager.GetEnemy(pos);
                container = enemyContainer;
            }
            else if (chance < weaponChance)
            {
                obj = spawnManager.GetWeapon(pos);
                container = weaponContainer;
            }
            else if (chance < pickUpChance)
            {
                obj = spawnManager.GetPickup(pos);
                container = pickupContainer;
            }
            else
            {
                continue;
            }

            obj.transform.parent = container.transform;
            spawnObj.Add(obj);

        }
    }

    public void FreePosition(Vector2 pos)
    {
        int index = spawnPos.IndexOf(pos);
        
        if (index != -1)
            spawnPos.Remove(pos);
    }

    internal bool CheckRoom(Vector2 cornerIndex)
    {
        return indexDict.ContainsKey(cornerIndex);
    }

    private bool OpenDoors(Queue<RoomComponent> roomQueue)
    {
        int counterLimit = 100000;
        int counter = 0;

        FindNeighbours(startingRoom, roomQueue);
        while (roomQueue.Count > 0 && counter < counterLimit)
        {
            RoomComponent nextRoom = roomQueue.Dequeue();
            FindNeighbours(nextRoom, roomQueue);
            counter++;
        }

        if (counter >= counterLimit)
            return false;
        return true;
    }

    private void FindNeighbours(RoomComponent room, Queue<RoomComponent> roomQueue)
    {
        Vector2 up = room.Index + Vector2.up;
        if (indexDict.ContainsKey(up))
        {
            RoomComponent upRoom = indexDict[up];
            if (!roomsChecked.Contains(upRoom))
            {
                roomsChecked.Add(upRoom);
                roomQueue.Enqueue(upRoom);
            }
            SetRoomsAdjacent(room, CardinalDirection.NORTH, upRoom, CardinalDirection.SOUTH);
        }

        Vector2 right = room.Index + Vector2.right;
        if (indexDict.ContainsKey(right))
        {
            RoomComponent rightRoom = indexDict[right];
            if (!roomsChecked.Contains(rightRoom))
            {
                roomsChecked.Add(rightRoom);
                roomQueue.Enqueue(rightRoom);
            }
            SetRoomsAdjacent(room, CardinalDirection.EAST, rightRoom, CardinalDirection.WEST);
        }

        Vector2 down = room.Index + Vector2.down;
        if (indexDict.ContainsKey(down))
        {
            RoomComponent downRoom = indexDict[down];
            if (!roomsChecked.Contains(downRoom))
            {
                roomsChecked.Add(downRoom);
                roomQueue.Enqueue(downRoom);
            }
            SetRoomsAdjacent(room, CardinalDirection.SOUTH, downRoom, CardinalDirection.NORTH);
        }

        Vector2 left = room.Index + Vector2.left;
        if (indexDict.ContainsKey(left))
        {
            RoomComponent leftRoom = indexDict[left];
            if (!roomsChecked.Contains(leftRoom))
            {
                roomsChecked.Add(leftRoom);
                roomQueue.Enqueue(leftRoom);
            }
            SetRoomsAdjacent(room, CardinalDirection.WEST, leftRoom, CardinalDirection.EAST);
        }
    }

    private void SetRoomsAdjacent(RoomComponent room1, CardinalDirection dir1, RoomComponent room2, CardinalDirection dir2)
    {
        room1.SetAdjacent(dir1, room2);
        room2.SetAdjacent(dir2, room1);
    }

    private void AddRoomSpawnPoints(RoomComponent roomComponent)
    {
        if (!roomSpawnPos.Contains(roomComponent.Index + Vector2.up) && !roomIndexes.Contains(roomComponent.Index + Vector2.up))
        {
            roomSpawnPos.Add(roomComponent.Index + Vector2.up);
        }

        if (!roomSpawnPos.Contains(roomComponent.Index + Vector2.down) && !roomIndexes.Contains(roomComponent.Index + Vector2.down))
        {
            roomSpawnPos.Add(roomComponent.Index + Vector2.down);
        }

        if (!roomSpawnPos.Contains(roomComponent.Index + Vector2.left) && !roomIndexes.Contains(roomComponent.Index + Vector2.left))
        {
            roomSpawnPos.Add(roomComponent.Index + Vector2.left);
        }

        if (!roomSpawnPos.Contains(roomComponent.Index + Vector2.right) && !roomIndexes.Contains(roomComponent.Index + Vector2.right))
        {
            roomSpawnPos.Add(roomComponent.Index + Vector2.right);
        }

    }

    Vector2 SelectRandomSpawnPoint()
    {
        int randomIndex = GenerateRandomInt(0, roomSpawnPos.Count-1);
        Vector2 element = roomSpawnPos[randomIndex];
        roomSpawnPos.Remove(element);
        return element;
    }

    public int GenerateRandomInt(int minValue, int maxValue)
    {
        return prng.Next(minValue, maxValue);
    }

    public void AddSpawnPoints(Vector2 pos)
    {
        spawnPos.Add(pos);
    }

    RoomComponent CreateRoom(Vector2 index)
    {
        Vector3 pos;
        IndexToPosition(index, out pos);

        GameObject roomGo = Instantiate(roomPrefab, pos+transform.position, Quaternion.identity);
        roomGo.transform.parent = transform;
        RoomComponent room = roomGo.GetComponent<RoomComponent>();
        room.SetDungeonGenerator(this);
        room.Index = index;
        room.CreateRoom(roomWidth, roomHeight);

        rooms.Add(room);
        indexDict[index] = room;

        AddRoomSpawnPoints(room);
        roomIndexes.Add(index);

        return room;
    }


    // void CreateDungeonDummy()
    // {
    //     int xi = -dungeonWidth/2;
    //     int yi = -dungeonHeight/2;
    //     int xf = dungeonWidth/2;
    //     int yf = dungeonHeight/2;

    //     for (int x = xi; x <= xf; x++)
    //     {
    //         for (int y = yi; y <= yf; y++)
    //         {
    //             CreateRoom(new Vector2(x, y));
    //         }
    //     }
    // }

    void IndexToPosition(Vector2 index, out Vector3 position)
    {
        // Add 2 to the size to account for the walls
        position = new Vector3(index.x * (roomWidth+2), index.y * (roomHeight+2), 0);
    }

}
