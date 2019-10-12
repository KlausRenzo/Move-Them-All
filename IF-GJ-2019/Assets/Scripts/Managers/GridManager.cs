using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private List<RoomConfiguration> rooms = new List<RoomConfiguration>();

    [Button]
    private void GetAllRooms()
    {
        rooms = GetAllInstances<RoomConfiguration>().ToList();
    }

    void Start()
    { }

    // Update is called once per frame
    void Update()
    { }

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++) //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;
    }
}