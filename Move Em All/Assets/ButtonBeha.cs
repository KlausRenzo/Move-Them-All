using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBeha : MonoBehaviour
{
    public GridManager gridManager;
    public AudioSource source;
    public void ClickMe()
    {
        source.Play();
        gridManager.Lose();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
