﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[CreateAssetMenu(fileName = "NewSceneData", menuName = "Scene Data", order = 51)]
public class SceneData : ScriptableObject {
    public string forwardAnimationClipName;
    public float forwardAnimationClipLength;
    public string backwardAnimationClipName;
    public float backwardAnimationClipLength;
    public string[] highlights;
    public string[] boundaries;
}