using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public Button hostButton;
    public Button joinButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        Debug.Log("starting host");
        NetworkManager.Singleton.StartHost();
        menuPanel.SetActive(false);
    }

    void StartClient()
    {
        Debug.Log("starting client");
        NetworkManager.Singleton.StartClient();
        menuPanel.SetActive(false);
    }
}
