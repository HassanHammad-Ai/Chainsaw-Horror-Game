using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Enemy Speeds")]
    public float enemyWalkSpeed;
    public float enemyChaseSpeed;

    [Header("UI")]
    public GameObject menuUI;

    public bool isGameActive = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // وقف اللعبة في البداية
        Time.timeScale = 0f;

        if (menuUI != null)
            menuUI.SetActive(true);
    }

    public void StartGame(int level)
    {
        Debug.Log("StartGame Called");

        // تحديد الصعوبة
        if (level == 1)
        {
            enemyWalkSpeed = 2f;
            enemyChaseSpeed = 3.5f;
        }
        else if (level == 2)
        {
            enemyWalkSpeed = 3f;
            enemyChaseSpeed = 5f;
        }
        else if (level == 3)
        {
            enemyWalkSpeed = 4f;
            enemyChaseSpeed = 7f;
        }

        // تشغيل اللعبة
        isGameActive = true;
        Time.timeScale = 1f;

        // اخفاء المينيو
        if (menuUI != null)
            menuUI.SetActive(false);
    }
}