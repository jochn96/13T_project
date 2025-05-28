using UnityEngine;

public class WeatherDrop
{
    public GameObject DropPrefab;
    public int value;
}
[CreateAssetMenu(fileName = "Weather", menuName = "New Weather")]
public class Weatherdata : ScriptableObject
{
    [Header("Info")]
    public string name;
    public GameObject weaterPrefabs;
}
