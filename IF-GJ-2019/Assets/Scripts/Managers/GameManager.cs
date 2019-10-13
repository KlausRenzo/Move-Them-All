using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ui;
using Sirenix.OdinInspector;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<RoomConfiguration> rooms = new List<RoomConfiguration>();
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UiManager uiManager;

    [Button(Name = "Get Rooms")]
    private void GetAllRooms()
    {
        rooms = Utilities.GetAllInstances<RoomConfiguration>().ToList();
    }

    private int roomNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        gridManager.roomCompleted += OnRoomCompleted;
        gridManager.roomFailed += OnRoomFailed;

        StartCoroutine(LevelTransition());
    }

    private void LoadRoom()
    {
        gridManager.Initialize(rooms[roomNumber]);
        uiManager.isEnabled = true;
    }

    private void OnRoomFailed()
    {
        StartCoroutine(LevelTransition());
    }

    private void OnRoomCompleted()
    {
        roomNumber++;
        if (roomNumber >= rooms.Count)
        {
            return;
        }

        StartCoroutine(LevelTransition());
    }

    private IEnumerator LevelTransition()
    {
        uiManager.isEnabled = false;
        yield return new WaitForSeconds(1);
        LoadRoom();
    }
}