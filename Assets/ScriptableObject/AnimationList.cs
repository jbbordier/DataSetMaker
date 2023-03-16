using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AnimationList")]
public class AnimationList : ScriptableObject
{
   
    public List<AnimationClip> clips = new List<AnimationClip>();
}
