using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using ExitGames.Client.Photon;

public class PickableObject : MonoBehaviourPun, IPunObservable, IPunOwnershipCallbacks, IOnEventCallback
{
    public int ID;
    public int maxAmmount;
    public int ammount;
    public string[] nameInLanguage;
    public bool equipable;
    
    [SerializeField]
    private Mesh NormMesh, Box;
    [SerializeField]
    private Material NormMat, BoxMat, IconMat;
    [SerializeField]
    private MeshFilter Filter;
    [SerializeField]
    private MeshRenderer Renderer;

    [Range(1, 2)]
    [SerializeField]
    private float boxSizeIncrement;

    [SerializeField]
    private BoxCollider BoxCol;
    [SerializeField]
    private MeshCollider MeshCol;

    const byte EventCode_Interacted = 1;

    float t = 0;

    public enum Type
    {
        tool = 1,
        food = 20,
        resource = 50,
    }

    public Type type;

    private void Start()
    {
        maxAmmount = (int)type;
        UpdateMesh();
    }

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        UpdateMesh();
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Update()
    {
        t += 1 * Time.deltaTime;

        if (t == 900) PhotonNetwork.Destroy(gameObject);
    }

    public void UpdateMesh()
    {
        PhotonNetwork.RaiseEvent(EventCode_Interacted, null, new RaiseEventOptions(), SendOptions.SendReliable);

        if (ammount == 1)
        {
            Filter.mesh = NormMesh;
            Renderer.materials = new Material[1] { NormMat };
            MeshCol.enabled = true;
            BoxCol.enabled = false;
            transform.localScale = Vector3.one * 100;
        }
        else
        {
            Filter.mesh = Box;
            Renderer.materials = new Material[2] { IconMat, BoxMat };
            MeshCol.enabled = false;
            BoxCol.enabled = true;
            transform.localScale = (((float)ammount / maxAmmount) + 1) * 125 * boxSizeIncrement * Vector3.one;
        }
    }

    public void RequestOwner()
    {
        photonView.RequestOwnership();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ammount);
        }
        else
        {
            ammount = (int)stream.ReceiveNext();
        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != photonView) return;

        photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != photonView) return;
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        if (targetView != photonView) return;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (EventCode_Interacted == photonEvent.Code)
        {
            UpdateMesh();
        }
    }
}
