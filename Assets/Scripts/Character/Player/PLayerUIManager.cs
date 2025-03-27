using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class PLayerUIManager : MonoBehaviour
{
    private static PLayerUIManager instance;
    public static PLayerUIManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

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
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Header("Network Join")]
    [SerializeField] bool startGameAsClient;

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
