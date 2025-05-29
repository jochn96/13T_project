using UnityEngine;

public enum WeatherBuffType
{
    Health,
    Stamina
}
public class WeatherDrop
{
    public string name;
    public GameObject DropPrefab;
    public int value;
    public string weatherBuffType;
}
[CreateAssetMenu(fileName = "Weather", menuName = "New Weather")]
public class Weatherdata : ScriptableObject
{
    [Header("Info")]
    public string name;
    public float value;
    public WeatherBuffType weatherBuffType;
    public GameObject weaterPrefabs;
}
