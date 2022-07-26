using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // LoadSceneAdditively("01.Field");
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0)) 测试用
        // {
        //     LoadSceneAdditively("02.House");
        //     SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); //卸载现在的场景
        // }        
        // if (Input.GetMouseButtonDown(1))
        // {
        //     LoadSceneAdditively("01.Field");
        //     SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); //卸载现在的场景
        // }
    }

    private void LoadSceneAdditively(string SceneName)
    {
        SceneMgr.Instance.LoadSceneAsync(SceneName, () =>
        {

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount -1); //正常情况下count会是2（一直存在和加载上来的场景）

            SceneManager.SetActiveScene(newScene);
            
            EventMgr.Instance.EventTrigger("SwitchCameraBounds");
        }, LoadSceneMode.Additive);
    }
}
