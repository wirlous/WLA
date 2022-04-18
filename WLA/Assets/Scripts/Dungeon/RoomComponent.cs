using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomComponent : MonoBehaviour
{
    [SerializeField] private int roomWidth;
    [SerializeField] private int roomHeight;

    [SerializeField] private Vector2 index;

    public bool nOpen;
    public bool sOpen;
    public bool wOpen;
    public bool eOpen;

    [SerializeField] private DungeonGenerator dungeonGenerator;

    public GameObject Walls;
    public GameObject Doors;
    public GameObject Floor;

    public Vector2 Index { get => index; set => index = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetDungeonGenerator(DungeonGenerator dungeon)
    {
        dungeonGenerator = dungeon;
    }

    public void CreateRoom(int width, int height)
    {
        // Get odd height and width
        AdaptDimensions(width, height);

        CreateFloor();
        CreateWallCorners();
        // CreateDoors();
    }

    private void AdaptDimensions(int width, int height)
    {
        roomWidth = (width%2==1) ? width : width+1;
        roomHeight = (height%2==1) ? height : height+1;
    }

    void CreateFloor()
    {
        int xi = -roomWidth/2;
        int yi = -roomHeight/2;
        int xf = roomWidth/2;
        int yf = roomHeight/2;

        for (int x = xi; x <= xf; x++)
        {
            for (int y = yi; y <= yf; y++)
            {
                GameObject f = Instantiate(dungeonGenerator.floorPrefab, new Vector3(x + transform.position.x, y + transform.position.y, transform.position.z), Quaternion.identity);
                f.transform.parent = Floor.transform;
            }
        }
    }

    private void CreateWallCorners()
    {
        int xi = -roomWidth/2;
        int yi = -roomHeight/2;

        int xf = roomWidth/2;
        int yf = roomHeight/2;

        for (int x = xi-1; x <= xf+1; x++)
        {
            for (int y = yi-1; y <= yf+1; y++)
            {
                if (((x < xi) || (x > xf) || (y < yi) || (y > yf)) && (x!=0 && y!=0))
                {
                    GameObject w = Instantiate(dungeonGenerator.wallPrefab, new Vector3(x + transform.position.x, y + transform.position.y, transform.position.z), Quaternion.identity);
                    w.transform.parent = Walls.transform;
                }
            }
        }
    }

    public void CreateDoors()
    {
        int xf = roomWidth/2;
        int yf = roomHeight/2;

        GameObject prefab;
        if (nOpen)
            prefab = dungeonGenerator.doorPrefab;
        else
            prefab = dungeonGenerator.wallPrefab;
        GameObject nDoor = Instantiate(prefab, new Vector3(transform.position.x, yf+1 + transform.position.y, transform.position.z), Quaternion.identity);
        nDoor.transform.parent = Doors.transform;

        if (sOpen)
            prefab = dungeonGenerator.doorPrefab;
        else
            prefab = dungeonGenerator.wallPrefab;
        GameObject sDoor = Instantiate(prefab, new Vector3(transform.position.x, -(yf+1) + transform.position.y, transform.position.z), Quaternion.identity);
        sDoor.transform.parent = Doors.transform;

        if (wOpen)
            prefab = dungeonGenerator.doorPrefab;
        else
            prefab = dungeonGenerator.wallPrefab;
        GameObject wDoor = Instantiate(prefab, new Vector3(-(xf+1) + transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        wDoor.transform.parent = Doors.transform;

        if (eOpen)
            prefab = dungeonGenerator.doorPrefab;
        else
            prefab = dungeonGenerator.wallPrefab;
        GameObject eDoor = Instantiate(prefab, new Vector3(xf+1 + transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        eDoor.transform.parent = Doors.transform;
    }

    public int GetNumberDoors()
    {
        int numDoors = 0;
        if (nOpen) numDoors++;
        if (sOpen) numDoors++;
        if (wOpen) numDoors++;
        if (eOpen) numDoors++;
        return numDoors;
    }

    private void CleanChildren(GameObject go)
    {
        foreach (Transform child in go.transform)
            GameObject.Destroy(child.gameObject);
    }


// #if (UNITY_EDITOR)
//     void OnGUI()
//     {
//         AdaptDimensions(roomWidth, roomHeight');

//         CleanChildren(Floor);
//         CreateFloor();
        
//         CleanChildren(Walls);
//         CreateWallCorners();

//         CleanChildren(Doors);
//         CreateDoors();
//     }
// #endif


    // Update is called once per frame
    void Update()
    {
        
    }
}
