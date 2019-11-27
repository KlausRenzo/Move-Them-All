using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ui;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<RoomConfiguration> rooms = new List<RoomConfiguration>();
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UiManager uiManager;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator uiAnimator;

#if UNITY_EDITOR
    [Button(Name = "Get Rooms")]
    private void GetAllRooms()
    {
        rooms = Utilities.GetAllInstances<RoomConfiguration>().ToList();
    }
#endif
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
        text.text = $"LIVELLO {rooms[roomNumber].roomNumber}";
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
        uiAnimator.gameObject.SetActive(true);
        uiAnimator.SetTrigger("Animation");
        uiManager.isEnabled = false;
        yield return new WaitForSeconds(.8f);
        LoadRoom();
        yield return new WaitForSeconds(.8f);
        uiAnimator.gameObject.SetActive(false);
    }
}