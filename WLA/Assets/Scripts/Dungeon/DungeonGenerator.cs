using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cinemachine;

public class DungeonGenerator : MonoBehaviour
{
    [Range(3,25)] public int roomWidth;
    [Range(3,25)] public int roomHeight;
    [Range(2,100)] public int roomCount;

    public string seed;
    public bool useRandomSeed;
    [HideInInspector] private System.Random prng;

    [SerializeField] private int dungeonCounter = 0;

    public GameObject roomPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject floorPrefab;
    public GameObject exitPrefab;
    public GameObject outsidePrefab;

    [SerializeField] private RoomComponent startingRoom;
    [SerializeField] private RoomComponent endRoom;
    [SerializeField] private List<RoomComponent> rooms = new List<RoomComponent>();
    private List<RoomComponent> roomsChecked = new List<RoomComponent>();
    private Dictionary<Vector2, RoomComponent> indexDict = new Dictionary<Vector2, RoomComponent>();
    [SerializeField] private List<Vector2> spawnPositions = new List<Vector2>();
    [SerializeField] private List<Vector2> roomIndexes = new List<Vector2>();


    void Awake()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }
        prng = new System.Random(seed.GetHashCode());
    }

    // Start is called before the first frame update
    void Start()
    {
        float initialTime = Time.realtimeSinceStartup;
        CreateDungeon();

        endRoom = GetFurthestRoom();
        endRoom.PlaceExit();
        
        float deltaTime = Time.realtimeSinceStartup - initialTime;
        Debug.Log("Time building dungeon: " + deltaTime.ToString("f6") + " seconds");

        PlaceOutside();
    }

    private RoomComponent GetFurthestRoom()
    {
        RoomComponent furthestRoom = startingRoom;
        float maxDist = 0f;
        foreach (var room in rooms)
        {
            float dist = Vector2.Distance(startingRoom.transform.position, room.transform.position);
            // if ((dist > maxDist))
            if ((dist > maxDist) && room.GetNumberDoorsOpen() == 1) // Fursthest must have only one door
            {
                maxDist = dist;
                furthestRoom = room;
            }
        }
        if (furthestRoom == startingRoom)
        {
            Debug.Log("Failed getting fursthest room");
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

        Debug.Log("Dungeon center: " + pos);
        Debug.Log("Dungeon size: " + size);

        GameObject outside = Instantiate(outsidePrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        outside.transform.parent = this.transform;
        SpriteRenderer outsideSR = outside.GetComponent<SpriteRenderer>();
        outsideSR.size = size;
        // CompositeCollider2D outsideCol = outside.GetComponent<CompositeCollider2D>();
        
        // CinemachineConfiner confiner = GameReferences.cmvcam.GetComponent<CinemachineConfiner>();
        // confiner.m_BoundingShape2D = outsideCol;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddRoom()
    {
        // Debug.Log("Add room");
        Vector2 index = SelectRandomSpawnPoint();
        CreateRoom(index);
    }

    void CreateDungeon()
    {
        startingRoom = CreateRoom(new Vector2(0, 0));

        for (int count = 1; count < roomCount; count++)
        {
            AddRoom();
        }

        roomsChecked = new List<RoomComponent>();
        Queue<RoomComponent> roomQueue = new Queue<RoomComponent>();
        bool ok = OpenDoors(roomQueue);
        if (!ok)
        {
            Debug.Log("ERROR OPEN DOOR");
        }

        foreach (var room in rooms)
        {
            room.CreateDoors();
            room.CreateWallCorners();
            room.CreateWallSides();
        }
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
        dungeonCounter++;
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

    private void AddSpawnPoints(RoomComponent roomComponent)
    {
        if (!spawnPositions.Contains(roomComponent.Index + Vector2.up) && !roomIndexes.Contains(roomComponent.Index + Vector2.up))
        {
            spawnPositions.Add(roomComponent.Index + Vector2.up);
        }

        if (!spawnPositions.Contains(roomComponent.Index + Vector2.down) && !roomIndexes.Contains(roomComponent.Index + Vector2.down))
        {
            spawnPositions.Add(roomComponent.Index + Vector2.down);
        }

        if (!spawnPositions.Contains(roomComponent.Index + Vector2.left) && !roomIndexes.Contains(roomComponent.Index + Vector2.left))
        {
            spawnPositions.Add(roomComponent.Index + Vector2.left);
        }

        if (!spawnPositions.Contains(roomComponent.Index + Vector2.right) && !roomIndexes.Contains(roomComponent.Index + Vector2.right))
        {
            spawnPositions.Add(roomComponent.Index + Vector2.right);
        }

    }

    Vector2 SelectRandomSpawnPoint()
    {
        int randomIndex = prng.Next(0, spawnPositions.Count-1);
        Vector2 element = spawnPositions[randomIndex];
        spawnPositions.Remove(element);
        return element;
    }

    RoomComponent CreateRoom(Vector2 index)
    {
        Vector3 position;
        IndexToPosition(index, out position);

        GameObject roomGo = Instantiate(roomPrefab, position, Quaternion.identity);
        roomGo.transform.parent = transform;
        RoomComponent room = roomGo.GetComponent<RoomComponent>();
        room.SetDungeonGenerator(this);
        room.Index = index;
        room.CreateRoom(roomWidth, roomHeight);

        rooms.Add(room);
        indexDict[index] = room;

        AddSpawnPoints(room);
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
