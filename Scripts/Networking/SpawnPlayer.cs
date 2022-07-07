using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Transform[] SpawnArea;

    void Start()
    {
        Transform selArea = SpawnArea[Random.Range(0, 3)];
        float rndX = Random.Range(selArea.GetChild(0).transform.position.x, selArea.GetChild(1).transform.position.x);
        float rndZ = Random.Range(selArea.GetChild(0).transform.position.z, selArea.GetChild(1).transform.position.z);

        PhotonNetwork.Instantiate("Player", new Vector3(rndX, 15, rndZ), Quaternion.identity);
    }
}
