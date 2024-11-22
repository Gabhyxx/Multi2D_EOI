using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    [SerializeField] Button buttonPlayMP;
    [SerializeField] GameObject panelMP;
    [SerializeField] GameObject sceneManager;

    private void Awake()
    {
        panelMP.SetActive(false);
    }
    void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            buttonPlayMP.interactable = true;
        }
        else
        {
            buttonPlayMP.interactable = false;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    override
    public void OnConnectedToMaster()
    {
        buttonPlayMP.interactable = true;
    }
    public void Play()
    {
        PhotonNetwork.JoinOrCreateRoom("GameRoom", new RoomOptions { }, TypedLobby.Default);
    }
    override
    public void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(3);
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ExitPhoton()
    {
        PhotonNetwork.Disconnect();
    }

    public void DisablePanelMP()
    {
        panelMP.SetActive(false);
    }
    public void EnablePanelMP()
    {
        panelMP.SetActive(true);
        Connect();
    }
    override
        public void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings();
    }
}
