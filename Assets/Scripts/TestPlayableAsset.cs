using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[System.Serializable]
public class TestPlayableAsset : PlayableAsset
{
    [Header("Dialog")] public ExposedReference<Text> dialog;
    [Multiline(3)] public string dialog_string;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<TestPlayableBehaviour>.Create(graph);
        playable.GetBehaviour().dialog = dialog.Resolve(graph.GetResolver());
        playable.GetBehaviour().dialog_string = dialog_string;
		return playable;
	}
}
