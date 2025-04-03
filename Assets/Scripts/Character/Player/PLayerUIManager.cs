using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class PLayerUIManager : MonoBehaviour
{
    public static PLayerUIManager instance;


    [Header("Network Join")]
    [SerializeField] bool startGameAsClient;

    [HideInInspector] public PlayerUiHudManager playerUiHudManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        playerUiHudManager = GetComponentInChildren<PlayerUiHudManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }



    private void Update()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            //We must first shut down, because we have started a host during the Title Screen
            NetworkManager.Singleton.Shutdown();
            //We Then Restart as a client
            NetworkManager.Singleton.StartClient();
        }
    }
}
