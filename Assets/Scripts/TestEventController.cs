using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestEventController : MonoBehaviour
{
    private PlayableDirector playable_director_ = null;
    private bool trigger_ = false;

	// Use this for initialization
	private void Start ()
    {
        playable_director_ = GetComponent<PlayableDirector>();
	}
	
	// Update is called once per frame
	private void Update ()
    {
		if(trigger_)
        {
            Debug.Log("Play!!");
            playable_director_.Play();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            trigger_ = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            trigger_ = false;
        }
    }
}
