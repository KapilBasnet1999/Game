using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World_Save_Game_Manager : MonoBehaviour
{
    private static World_Save_Game_Manager instance;
    public static World_Save_Game_Manager Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    [SerializeField] int worldSceneIndex = 1;
    private void Awake()
    {//There can only be one intance of this script at a time if another exists Desgtroy it
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
      
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public IEnumerator LoadNewGame()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
        yield return null;
    }
    

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
