using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

// A behaviour that is attached to a playable
public class TestPlayableBehaviour : PlayableBehaviour
{
    public Text dialog;
    public string dialog_string;

	// Called when the owning graph starts playing
	public override void OnGraphStart(Playable playable)
    {
	}

	// Called when the owning graph stops playing
	public override void OnGraphStop(Playable playable) {
		
	}

	// Called when the state of the playable is set to Play
	public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if(dialog)
        {
            dialog.gameObject.SetActive(true);
            dialog.text = dialog_string;
        }
	}

	// Called when the state of the playable is set to Paused
	public override void OnBehaviourPause(Playable playable, FrameData info)
    {
		if(dialog)
        {
            dialog.gameObject.SetActive(false);
        }
	}

	// Called each frame while the state is set to Play
	public override void PrepareFrame(Playable playable, FrameData info) {
		
	}
}
