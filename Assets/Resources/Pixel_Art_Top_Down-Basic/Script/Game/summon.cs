using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class summon : MonoBehaviour
{
    // Start is called before the first frame update
    public int belong;
    summon_list deck;
    void Awake()
    {
        deck = GameObject.Find("summon_list").GetComponent<summon_list>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        Debug.Log("summon");
        deck.activate(true);
    }
}
