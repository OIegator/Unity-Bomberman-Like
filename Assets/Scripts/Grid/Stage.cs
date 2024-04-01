using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "Stage")]
public class Stage : ScriptableObject
{
    public Vector2Int size;
    public Vector2Int start;
    public Vector3 cameraConfinerCenter;
    public Vector3 cameraConfinerSize;

    public List<BlockInfo> blocks;
}

[Serializable]
public class BlockInfo
{
    public ObjectType objectType = ObjectType.Ground;
    public AboveObjectType objectAbove;
}
