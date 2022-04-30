﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirection
{
    NORTH,
    WEST,
    SOUTH,
    EAST
};


public enum CardinalDirectionComposite
{
    NORTH_WEST,
    SOUTH_WEST,
    NORTH_EAST,
    SOUTH_EAST
};

public class RoomComponent : MonoBehaviour
{
    [SerializeField] private int roomWidth;
    [SerializeField] private int roomHeight;

    [SerializeField] private Vector2 index;

    private Dictionary<CardinalDirection, RoomComponent> adjacentRoom = new Dictionary<CardinalDirection, RoomComponent>();
    private Dictionary<CardinalDirection, bool> doorOpen = new Dictionary<CardinalDirection, bool>();
    private Dictionary<CardinalDirection, bool> wallOpen = new Dictionary<CardinalDirection, bool>();

    [SerializeField] private DungeonGenerator dungeonGenerator;

    public GameObject Walls;
    public GameObject Doors;
    public GameObject Floor;
    public GameObject Exit;

    public Vector2 Index { get => index; set => index = value; }

    public void SetDungeonGenerator(DungeonGenerator dungeon)
    {
        dungeonGenerator = dungeon;
    }

    public void CreateRoom(int width, int height)
    {
        // Initialize open dictionaries
        InitOpenVariables();

        // Get odd height and width
        AdaptDimensions(width, height);
        
        // Create floor
        CreateFloor();
    }

    private void AdaptDimensions(int width, int height)
    {
        roomWidth = (width%2==1) ? width : width+1;
        roomHeight = (height%2==1) ? height : height+1;
    }

    internal void PlaceExit()
    {
        GameObject exit = Instantiate(dungeonGenerator.exitPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        exit.transform.parent = Exit.transform;
    }

    private void InitOpenVariables()
    {
        foreach (CardinalDirection dir in Enum.GetValues(typeof(CardinalDirection)))
        {
            doorOpen[dir] = false;
            wallOpen[dir] = false;
        }
    }

    void CreateFloor()
    {
        GameObject floor = Instantiate(dungeonGenerator.floorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        floor.transform.parent = Floor.transform;
        SpriteRenderer floorSR = floor.GetComponent<SpriteRenderer>();
        floorSR.size = new Vector2(roomWidth+2, roomHeight+2);
    }

    public void CreateWallCorners()
    {
        int xf = roomWidth/2;
        int yf = roomHeight/2;

        foreach (CardinalDirectionComposite dirComposite in Enum.GetValues(typeof(CardinalDirectionComposite)))
        {
            Vector2 indexOffset = GetDirection(dirComposite);
            bool cornerExist = dungeonGenerator.CheckRoom(indexOffset + index);

            if (GetDictionaryState(wallOpen, dirComposite) && cornerExist)
                continue;

            GameObject prefab = dungeonGenerator.wallPrefab;
            Vector2 offset = indexOffset * new Vector2(xf+1, yf+1);
            
            GameObject corner = Instantiate(prefab, new Vector3(offset.x + transform.position.x, offset.y + transform.position.y, transform.position.z), Quaternion.identity);
            corner.transform.parent = Walls.transform;
        }
    }

    public void CreateWallSides()
    {
        foreach (CardinalDirection dir in Enum.GetValues(typeof(CardinalDirection)))
        {
            if (GetDictionaryState(wallOpen, dir))
                continue;

            GameObject prefab = dungeonGenerator.wallPrefab;

            Vector2 pos = GetWallPosition(dir);
            GameObject side = Instantiate(prefab, new Vector3(pos.x + transform.position.x, pos.y + transform.position.y, transform.position.z), Quaternion.identity);
            side.transform.parent = Walls.transform;
            
            SpriteRenderer sideSR = side.GetComponent<SpriteRenderer>();
            sideSR.size = GetWallSize(dir);
        }
    }

    public void CreateDoors()
    {
        int xf = roomWidth/2;
        int yf = roomHeight/2;

        foreach (CardinalDirection dir in Enum.GetValues(typeof(CardinalDirection)))
        {
            GameObject prefab = (doorOpen[dir]) ? dungeonGenerator.doorPrefab : dungeonGenerator.wallPrefab;
            Vector2 offset = GetDirection(dir) * new Vector2(xf+1, yf+1);
            GameObject door = Instantiate(prefab, new Vector3(offset.x + transform.position.x, offset.y + transform.position.y, transform.position.z), Quaternion.identity);
            door.transform.parent = Doors.transform;
        }
    }

    public void SetState(CardinalDirection dir, bool state, bool both)
    {
        if (both)
            wallOpen[dir] = state;
        doorOpen[dir] = state;
    }

    public void SetDoorState(CardinalDirection dir, bool state)
    {
        doorOpen[dir] = state;
    }

    public void SetWallState(CardinalDirection dir, bool state)
    {
        wallOpen[dir] = state;
    }

    public void SetAdjacent(CardinalDirection dir, RoomComponent room)
    {
        adjacentRoom[dir] = room;
        SetDoorState(dir, true);
        SetWallState(dir, true);
    }

    private Vector2 GetDirection(CardinalDirection dir)
    {
        switch (dir)
        {
            case CardinalDirection.NORTH:
                return Vector2.up;
            case CardinalDirection.SOUTH:
                return Vector2.down;
            case CardinalDirection.WEST:
                return Vector2.left;
            case CardinalDirection.EAST:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    private List<Vector2> GetRange(CardinalDirection dir, int minX, int maxX, int minY, int maxY)
    {
        Vector2 offset = GetDirection(dir);
        List<Vector2> range = new List<Vector2>();
        if (offset.x != 0)
        {
            // Vertical
            for (int y = minY; y <= maxY; y++)
            {
                if (y != 0)
                    range.Add(new Vector2(offset.x*(maxX+1), y));
            }
        }
        else
        {
            // Horizontal
            for (int x = minX; x <= maxX; x++)
            {
                if (x != 0)
                    range.Add(new Vector2(x, offset.y*(maxY+1)));
            }
        }
        return range;
    }

    private Vector2 GetWallSize(CardinalDirection cardDir)
    {
        Vector2 dir = GetDirection(cardDir);
        float width  = (dir.x != 0) ? 1 : roomWidth;
        float height = (dir.y != 0) ? 1 : roomHeight;
        return new Vector2(width, height);
    }

    private Vector2 GetWallPosition(CardinalDirection cardDir)
    {
        Vector2 dir = GetDirection(cardDir);
        return dir * new Vector2((roomWidth+1)/2f, (roomHeight+1)/2f);
    }

    private Vector2 GetDirection(CardinalDirectionComposite dir)
    {
        CardinalDirection dir0;
        CardinalDirection dir1;
        SplitCardinalDirectionComposite(dir, out dir0, out dir1);
        return GetDirection(dir0) + GetDirection(dir1);
    }

    private void SplitCardinalDirectionComposite(CardinalDirectionComposite dir, out CardinalDirection dirOut0, out CardinalDirection dirOut1)
    {
        dirOut0 = CardinalDirection.NORTH;
        dirOut1 = CardinalDirection.WEST;

        switch (dir)
        {
            case CardinalDirectionComposite.NORTH_WEST:
                dirOut0 = CardinalDirection.NORTH;
                dirOut1 = CardinalDirection.WEST;
                break;
            case CardinalDirectionComposite.SOUTH_WEST:
                dirOut0 = CardinalDirection.SOUTH;
                dirOut1 = CardinalDirection.WEST;
                break;
            case CardinalDirectionComposite.SOUTH_EAST:
                dirOut0 = CardinalDirection.SOUTH;
                dirOut1 = CardinalDirection.EAST;
                break;
            case CardinalDirectionComposite.NORTH_EAST:
                dirOut0 = CardinalDirection.NORTH;
                dirOut1 = CardinalDirection.EAST;
                break;
            default:
                break;
        }
    }

    private bool GetDictionaryState(Dictionary<CardinalDirection, bool> dictionary, CardinalDirectionComposite dir)
    {
        CardinalDirection dir0;
        CardinalDirection dir1;
        SplitCardinalDirectionComposite(dir, out dir0, out dir1);
        return (dictionary[dir0] && dictionary[dir1]);
    }

    private bool GetDictionaryState(Dictionary<CardinalDirection, bool> dictionary, CardinalDirection dir)
    {
        return (dictionary[dir]);
    }


    public int GetNumberDoorsOpen()
    {
        int numDoors = 0;
        foreach (CardinalDirection dir in Enum.GetValues(typeof(CardinalDirection)))
        {
            numDoors = doorOpen[dir] ? numDoors+1 : numDoors;
        }
        return numDoors;
    }

    public int GetNumberWallsOpen()
    {
        int numWalls = 0;
        foreach (CardinalDirection dir in Enum.GetValues(typeof(CardinalDirection)))
        {
            numWalls = wallOpen[dir] ? numWalls+1 : numWalls;
        }
        return numWalls;
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
}
