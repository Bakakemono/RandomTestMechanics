using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BSP : MonoBehaviour
{
    public void DoBSP()
    {
        StartBSP();
    }

    public void ResetBSP()
    {
        ResetList();
    }

    struct Room
    {
        public Vector2 position;
        public Vector2 size;

        public List<Room> child;
    }

    private Room alphaRoom;
    [SerializeField] private Vector2 alphaRoomSize = new Vector2(100, 100);

    [Range(1, 1000)] [SerializeField] private int maxRoomSizeX = 20;
    [Range(1, 1000)] [SerializeField] private int maxRoomSizeY = 20;

    [Range(1, 1000)] [SerializeField] private int minRoomSizeX = 20;
    [Range(1, 1000)] [SerializeField] private int minRoomSizeY = 20;

    [Range(0, 100)] [SerializeField] private int minSplitRange = 20;
    [Range(0, 100)] [SerializeField] private int maxSplitRange = 20;

    [Range(0, 100)] [SerializeField] private float splitLuckX = 20;
    [Range(0, 100)] [SerializeField] private float splitLuck = 20;

    private void ResetList()
    {
        alphaRoom.position = new Vector2(0, 0);
        alphaRoom.size = alphaRoomSize;
        alphaRoom.child = new List<Room>();
    }

    private void StartBSP()
    {
        if (minSplitRange > maxSplitRange || minRoomSizeX > maxRoomSizeX || minRoomSizeY > maxRoomSizeY)
        {
            Debug.LogError("ONE OR MORE OF YOUR RANGE IS NOT COHERENT (MIN < MAX !!!)");
            return;
        }

        alphaRoom.position = new Vector2(0, 0);
        alphaRoom.size = alphaRoomSize;
        alphaRoom.child = new List<Room>();

        alphaRoom.child.AddRange(Split(alphaRoom));
    }

    List<Room> Split(Room room)
    {
        if (room.size.x > maxRoomSizeX || room.size.y > maxRoomSizeY)
        {
            if (room.size.x > maxRoomSizeX && room.size.y > maxRoomSizeY)
            {
                return Random.value <= splitLuckX / 100 ? SplitX(room) : SplitY(room);
            }
            if (room.size.x > maxRoomSizeX)
            {
                return SplitX(room);
            }
            else
            {
                return SplitY(room);
            }
        }

        if (Random.value >= splitLuck / 100)
        {
            return new List<Room>();
        }

        if (room.size.x > minRoomSizeX * 100 / minSplitRange || room.size.y > minRoomSizeY * 100 / minSplitRange)
        {
            if (room.size.x > minRoomSizeX * 100 / minSplitRange && room.size.y > minRoomSizeY * 100 / minSplitRange)
            {
                return Random.value <= splitLuckX / 100 ? SplitX(room) : SplitY(room);
            }
            if (room.size.x > minRoomSizeX * 100 / minSplitRange)
            {
                return SplitX(room);
            }
            else
            {
                return SplitY(room);
            }
        }

        return new List<Room>();
    }

    List<Room> SplitX(Room room)
    {
        List<Room> newRooms = new List<Room>();
        Room newRoomOne;
        Room newRoomTwo;

        int cut = Random.Range(minSplitRange, maxSplitRange);

        newRoomOne.size = new Vector2(Mathf.RoundToInt(room.size.x * cut / 100), room.size.y);
        newRoomOne.position = new Vector2(room.position.x + newRoomOne.size.x * 0.5f - room.size.x * 0.5f, room.position.y);
        newRoomOne.child = new List<Room>();

        newRoomOne.child.AddRange(Split(newRoomOne));

        newRooms.Add(newRoomOne);

        newRoomTwo.size = new Vector2(room.size.x - newRoomOne.size.x, room.size.y);
        newRoomTwo.position = new Vector2(room.position.x - newRoomTwo.size.x * 0.5f + room.size.x * 0.5f, room.position.y);
        newRoomTwo.child = new List<Room>();

        newRoomOne.child.AddRange(Split(newRoomTwo));

        newRooms.Add(newRoomTwo);
        
        return newRooms;
    }

    List<Room> SplitY(Room room)
    {
        List<Room> newRooms = new List<Room>();

        Room newRoomOne;
        Room newRoomTwo;

        int cut = Random.Range(minSplitRange, maxSplitRange);

        newRoomOne.size = new Vector2(room.size.x, Mathf.RoundToInt(room.size.y * cut / 100));
        newRoomOne.position = new Vector2(room.position.x, room.position.y + newRoomOne.size.y * 0.5f - room.size.y * 0.5f);
        newRoomOne.child = new List<Room>();

        newRoomOne.child.AddRange(Split(newRoomOne));

        newRooms.Add(newRoomOne);

        newRoomTwo.size = new Vector2(room.size.x, room.size.y - newRoomOne.size.y);
        newRoomTwo.position = new Vector2(room.position.x, room.position.y - newRoomTwo.size.y * 0.5f + room.size.y * 0.5f);
        newRoomTwo.child = new List<Room>();

        newRoomOne.child.AddRange(Split(newRoomTwo));

        newRooms.Add(newRoomTwo);

        return newRooms;
    }


    void OnDrawGizmos()
    {
        DrawRoom(alphaRoom);
    }

    void DrawRoom(Room room)
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(room.position, room.size);

        if (room.child == null) return;
        foreach (Room roomChild in room.child)
        {
            DrawRoom(roomChild);
        }
    }
}

[CustomEditor(typeof(BSP))]
public class BSPCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BSP bsp = (BSP)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            bsp.ResetBSP();
        }
        if (GUILayout.Button("Generate"))
        {
            bsp.DoBSP();
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();
    }
}
