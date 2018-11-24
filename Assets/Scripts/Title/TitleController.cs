using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    private InputManager input_ = null;
    private AsyncOperation game_scene_ = null;
    private bool to_game_scene_ = false;

    public float timeOut;
    private float timeElapsed;

    // Use this for initialization
    void Start ()
    {
        input_ = JoyconManager.Instance.gameObject.GetComponent<InputManager>();
        game_scene_ = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
        game_scene_.allowSceneActivation = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(ImagePanel.isBreak)
        {
            timeElapsed++;
            GameObject.DontDestroyOnLoad(this);
            to_game_scene_ = true;
        }

        if(timeElapsed >= 60)
        {
            ToGameScene();
        }
    }

    private void ToGameScene()
    {
        if (!to_game_scene_) return;
        Debug.Log("Prepare to go to game scene");
        //if (!game_scene_.isDone) return;
        game_scene_.allowSceneActivation = true;
    }
}
