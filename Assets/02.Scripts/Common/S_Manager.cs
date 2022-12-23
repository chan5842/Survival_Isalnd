using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Scene 관련 기능을 가져오기 위해 선언
using UnityEngine.SceneManagement;

public class S_Manager : MonoBehaviour
{
    private void Start()
    {
        // 커서 보이게
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        /*
        커서 안보이게
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        */
    }
    // 게임 시작 버튼
    public void PlayGame ()
    {
        // 게임 시작 버튼을 누르면 "PlayScene"로 이동
        // SceneManager.LoadScene("PlayScene");
        SceneManager.LoadScene("SceneLoader");
    }

    // 게임 종료 버튼
    public void QuitGame()
    {
        // 게임 종료버튼을 누르면 게임 종료
        Application.Quit();
    }
}
