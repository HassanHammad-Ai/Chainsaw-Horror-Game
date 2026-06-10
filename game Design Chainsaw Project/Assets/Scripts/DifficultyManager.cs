using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static float walkSpeed = 2f;
    public static float chaseSpeed = 4f;

    public GameObject menuUI;

    void Start()
    {
        Time.timeScale = 0f; // وقف اللعبة لحد ما يختار
    }

    public void Easy()
    {
        Debug.Log("Easy Pressed"); // 👈 مهم
        walkSpeed = 2f;
        chaseSpeed = 3.5f;
        StartGame();
    }

    public void Medium()
    {
        walkSpeed = 3f;
        chaseSpeed = 5f;
        StartGame();
    }

    public void Hard()
    {
        walkSpeed = 4f;
        chaseSpeed = 7f;
        StartGame();
    }

    void StartGame()
    {
        menuUI.SetActive(false);

        // 👇 أهم سطرين
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
    }
}