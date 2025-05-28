using UnityEngine;

public enum EventType
{
    Drop,
    Effect
}

public enum WeaterType
{
    Rain,
    Snow,
    bloom
}

public class WeatherDrop
{
    public WeaterType Type;
    public GameObject DropPrefab;
    public int value;
}
[CreateAssetMenu(fileName = "Weather", menuName = "New Weather")]
public class Weatherdata : ScriptableObject
{
    [Header("Info")]
    public EventType Event;
    public WeaterType Type;
    public GameObject weaterPrefabs;
}
