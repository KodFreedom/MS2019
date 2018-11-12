using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    private InputManager input_ = null;
    private AsyncOperation title_scene_ = null;
    private bool to_title_scene = false;

	private void Start ()
    {
        input_ = JoyconManager.Instance.gameObject.GetComponent<InputManager>();
        title_scene_ = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Title");
        title_scene_.allowSceneActivation = false;
    }

    private void Update()
    {
        if (input_.GetPunchL()
            || input_.GetPunchR())
        {
            to_title_scene = true;
        }

        ToTitleScene();
    }

    private void ToTitleScene()
    {
        if (!to_title_scene) return;
        Debug.Log("Prepare to go to title scene");
        title_scene_.allowSceneActivation = true;
    }
}
