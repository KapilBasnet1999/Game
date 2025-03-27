using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TitleScreenManager : MonoBehaviour
{
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartNewGame()
    {
        StartCoroutine(World_Save_Game_Manager.Instance.LoadNewGame());
    }

}
