using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneMgr.Instance.LoadSceneAsync("01.Field", () =>
        {

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount -1 );

            SceneManager.SetActiveScene(newScene);
            
            EventMgr.Instance.EventTrigger("SwitchCameraBounds");
        }, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
