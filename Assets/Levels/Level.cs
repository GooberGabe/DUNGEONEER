using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level")]
public class Level : ScriptableObject
{
    [SerializeField] private List<TextAsset> waveData;
    [SerializeField] private Vector2[] startPositions;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private List<RoomPreview> roomModules;
    //[SerializeField] private TextAsset assetData;

    public List<TextAsset> WaveData => waveData;

    public Vector2[] StartPositions => startPositions;

    public Vector2 EndPositon => endPosition;

    public List<RoomPreview> RoomModules => roomModules;

}