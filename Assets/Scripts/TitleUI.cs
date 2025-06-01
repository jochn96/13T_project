using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public void OnClickStartButton()
    {
        GameManager.Instance.StartGame();
    }
}
