using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Blocks/BlocksData")]
public class PuzzleBlock : ScriptableObject
{
    public GameObject blockGameObject;
    public Transform transform;
}
