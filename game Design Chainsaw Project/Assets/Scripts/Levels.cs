using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public int level; // 1 = Easy, 2 = Medium, 3 = Hard

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SetLevel);
    }

    public void SetLevel()
    {
        Time.timeScale = 1f;
        Debug.Log("FORCED START");
    }
}