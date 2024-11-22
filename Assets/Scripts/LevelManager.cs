using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private void Start()
    {
        GameObject myPlayer = PhotonNetwork.Instantiate("Player", new Vector3(-4, 2, 0), Quaternion.identity);

        int index = GameManager.instance.playerNumber;

        myPlayer.transform.position += Vector3.right * index * 0.2f;
        myPlayer.GetComponent<Player>().SetPlayerNumber(index);

        myPlayer.GetComponent<PhotonView>().RPC("SetPlayerNumber", RpcTarget.AllBuffered, index);
    }
}
