using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class InventoryUiControl : MonoBehaviourPun, IPunOwnershipCallbacks
{
    [SerializeField]
    private PlayerController playerContr;
    [SerializeField]
    private Transform DroppingZone, IGpanel, IINVPanel;

    [SerializeField]
    private Animator InvButtAnim, CraftButtAnim;
    [SerializeField]
    private GameObject InvPanel, CraftPanel;
    [SerializeField]
    private Image InvButtIm, CraftButtIm;
    [SerializeField]
    private InventorySlot[] Slot = new InventorySlot[16], PlayerIGSlot = new InventorySlot[4], PlayerIINVSlot = new InventorySlot[4];

    [SerializeField]
    private Slider[] HealthSlider, StaminaSlider, HungerSlider;

    [SerializeField]
    private Sprite[] sprites;
    public static Sprite[] SpriteList;

    [SerializeField]
    private GameObject Msg;
    [SerializeField]
    private Transform MsgZone;

    bool InInv = true;
    bool InvOpened = false;
    bool InContainer = false;
    bool transitioning = false;

    public InventorySlot PickedItemSlot;

    private void Start()
    {
        if (!photonView.IsMine) gameObject.SetActive(false);

        CraftButtAnim.SetTrigger("Up");

        SpriteList = new Sprite[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteList[i] = sprites[i];
            sprites[i] = null;
        }
    }

    private void OnEnable()
    {
        InGameEvents.current.OnPickableObjectInteracted += PickableObjectInteracted;
        InGameEvents.current.OnInventoryClosed += InventoryClosed;
        InGameEvents.current.OnGamePaused += HideIG;
        InGameEvents.current.OnGameResumed += ShowIG;

        PhotonNetwork.AddCallbackTarget(this);

        foreach (InventorySlot slot in PlayerIGSlot)
        {
            slot.Initialize();
        }

        UpdateBar();
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);

        InGameEvents.current.OnPickableObjectInteracted -= PickableObjectInteracted;
        InGameEvents.current.OnInventoryClosed -= InventoryClosed;
        InGameEvents.current.OnGamePaused -= HideIG;
        InGameEvents.current.OnGameResumed -= ShowIG;
    }

    public void InvButtClicked()
    {
        if (InInv) return;
        InInv = true;

        InvButtAnim.SetTrigger("Down");
        CraftButtAnim.SetTrigger("Up");

        InvPanel.SetActive(true);
        CraftPanel.SetActive(false);

        InvButtIm.CrossFadeAlpha(1, 0, true);
        CraftButtIm.CrossFadeAlpha(0.9f, 0, true);
    }

    public void CraftButtClicked()
    {
        if (!InInv) return;
        InInv = false;

        InvButtAnim.SetTrigger("Up");
        CraftButtAnim.SetTrigger("Down");

        InvPanel.SetActive(false);
        CraftPanel.SetActive(true);

        CraftButtIm.CrossFadeAlpha(1, 0, true);
        InvButtIm.CrossFadeAlpha(0.9f, 0, true);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory") && !transitioning && !CarelessUIScript.paused && !CarelessUIScript.optionsOpened && !CarelessUIScript.transitioning)
        {
            if (!InvOpened && !InContainer) OpenInv();
            else if (InvOpened) CloseInv(false);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject a = PhotonNetwork.Instantiate("0", DroppingZone.position, Quaternion.identity);
            a.GetComponent<Rigidbody>().AddForce(DroppingZone.forward * 5, ForceMode.VelocityChange);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !transitioning)
        {
            if (InvOpened) CloseInv(true);
        }

        HealthSlider[0].value = playerContr.Health;
        HealthSlider[1].value = playerContr.Health;

        StaminaSlider[0].value = playerContr.Stamina;
        StaminaSlider[1].value = playerContr.Stamina;

        HungerSlider[0].value = playerContr.Hunger;
        HungerSlider[1].value = playerContr.Hunger;
    }

    private async void OpenInv()
    {
        if (transitioning) return;

        InGameEvents.current.InventoryOpened();

        UpdateIINVBar();

        InvOpened = true;
        transitioning = true;

        float v = 0, ny = 1080;

        while (ny > 1)
        {
            ny = Mathf.SmoothDamp(ny, 0, ref v, 0.15f);

            IINVPanel.localPosition = new Vector3(0, ny, 0);
            IGpanel.localPosition = new Vector3(0, -1080 + ny, 0);
            await Task.Yield();
        }

        transitioning = false;
    }

    private async void CloseInv(bool WithEsc)
    {
        if (transitioning) return;

        InGameEvents.current.InventoryClosed(WithEsc);

        UpdateBar();

        InvOpened = false;
        transitioning = true;

        float v = 0, ny = 1080;

        while (ny > 1)
        {
            ny = Mathf.SmoothDamp(ny, 0, ref v, 0.15f);

            IINVPanel.localPosition = new Vector3(0, 1080 - ny, 0);
            if (!WithEsc) IGpanel.localPosition = new Vector3(0, -ny, 0);
            await Task.Yield();
        }

        transitioning = false;
    }

    private async void HideIG()
    {
        float v = 0, ny = 1080;

        while (ny > 1)
        {
            ny = Mathf.SmoothDamp(ny, 0, ref v, 0.15f);

            IGpanel.localPosition = new Vector3(0, -1080 + ny, 0);
            await Task.Yield();
        }
    }

    private async void ShowIG()
    {
        float v = 0, ny = 1080;

        while (ny > 1)
        {
            ny = Mathf.SmoothDamp(ny, 0, ref v, 0.15f);

            IGpanel.localPosition = new Vector3(0, -ny, 0);
            await Task.Yield();
        }

    }

    public void LeftClicked(InventorySlot slot)
    {
        if (slot.Item == null)
        {
            if (PickedItemSlot.Item != null)
            {
                if (!slot.isPlayerSlot)
                {
                    slot.AddItem(PickedItemSlot.Item, PickedItemSlot.Item.objAmount);
                    PickedItemSlot.RemoveItem();
                }
                else
                {
                    if (!PickedItemSlot.Item.equipable) return;

                    slot.AddItem(PickedItemSlot.Item, PickedItemSlot.Item.objAmount);
                    PickedItemSlot.RemoveItem();
                }
            }
        }
        else
        {
            if (PickedItemSlot.Item == null)
            {
                PickedItemSlot.AddItem(slot.Item, slot.Item.objAmount);
                slot.RemoveItem();
            }
            else
            {
                if (PickedItemSlot.Item.ID != slot.Item.ID || (PickedItemSlot.Item.objAmount + slot.Item.objAmount) > PickedItemSlot.Item.ObjMaxAmmount)
                {
                    InventoryItem r = PickedItemSlot.Item;

                    PickedItemSlot.RemoveItem();
                    PickedItemSlot.AddItem(slot.Item, slot.Item.objAmount);

                    slot.RemoveItem();
                    slot.AddItem(r, r.objAmount);

                    return;
                }

                slot.Item.objAmount += PickedItemSlot.Item.objAmount;
                PickedItemSlot.RemoveItem();
            }
        }

        PickedItemSlot.UpdateImage();
        PickedItemSlot.UpdateAmountText();
        slot.UpdateAmountText();
        slot.UpdateImage();
    }

    public void RightClicked(InventorySlot slot)
    {
        if (slot.Item == null)
        {
            if (PickedItemSlot.Item != null)
            {
                if (!slot.isPlayerSlot)
                {
                    slot.AddItem(PickedItemSlot.Item, 1);

                    PickedItemSlot.Item.objAmount -= 1;
                }
                else
                {
                    if (!PickedItemSlot.Item.equipable) return;

                    slot.AddItem(PickedItemSlot.Item, 1);

                    PickedItemSlot.Item.objAmount -= 1;
                }
            }
        }
        else
        {
            if (PickedItemSlot.Item != null)
            {
                if (slot.Item.ID != PickedItemSlot.Item.ID || slot.Item.objAmount + 1 > slot.Item.ObjMaxAmmount) return;

                slot.Item.objAmount += 1;
                PickedItemSlot.Item.objAmount -= 1;
            }
            else
            {
                if (slot.Item.objAmount % 2 == 0)
                {
                    slot.Item.objAmount /= 2;
                    PickedItemSlot.AddItem(slot.Item, slot.Item.objAmount);
                }
                else
                {
                    slot.Item.objAmount = (slot.Item.objAmount - 1) / 2;
                    PickedItemSlot.AddItem(slot.Item, slot.Item.objAmount);
                    PickedItemSlot.Item.objAmount = PickedItemSlot.Item.objAmount + 1;
                }
            }  
        }

        PickedItemSlot.UpdateImage();
        PickedItemSlot.UpdateAmountText();
        slot.UpdateAmountText();
        slot.UpdateImage();

        if (slot.Item.objAmount <= 0) slot.RemoveItem();
        if (PickedItemSlot.Item.objAmount <= 0) PickedItemSlot.RemoveItem();
    }

    public void MiddleClicked(InventorySlot slot)
    {
        DropItem(slot);
    }

    PickableObject intobj;
    bool wannaPickup = false;

    void PickableObjectInteracted(PickableObject obj)
    {
        if (!wannaPickup)
        {
            wannaPickup = true;

            obj.RequestOwner();

            intobj = obj;
        }
    }

    void PickableAproved()
    {
        int amountRemaining = intobj.ammount;

        if (intobj.type == PickableObject.Type.tool)
        {
            for (int i = 0; i < PlayerIINVSlot.Length; i++)
            {
                if (PlayerIINVSlot[i].Item == null)
                {
                    PlayerIINVSlot[i].AddItemThroughPickable(intobj, amountRemaining);
                    amountRemaining = 0;
                }
            }

            UpdateBar();
            return;
        }

        for (int i = 0; i < 16 && amountRemaining > 0; i++)
        {
            if (Slot[i].Item?.ID == intobj.ID)
            {
                if (Slot[i].Item.objAmount < Slot[i].Item.ObjMaxAmmount)
                {
                    int toFill = Slot[i].Item.ObjMaxAmmount - Slot[i].Item.objAmount;

                    if (amountRemaining <= toFill)
                    {
                        Slot[i].Item.objAmount += amountRemaining;
                        Slot[i].UpdateAmountText();
                        Slot[i].UpdateImage();
                        amountRemaining = 0;
                    }
                    else
                    {
                        Slot[i].Item.objAmount += toFill;
                        Slot[i].UpdateAmountText();
                        Slot[i].UpdateImage();
                        amountRemaining -= toFill;
                    }
                }
            }
        }

        for (int i = 0; i < 16 && amountRemaining > 0; i++)
        {
            if (Slot[i].Item == null)
            {
                Slot[i].AddItemThroughPickable(intobj, amountRemaining);
                amountRemaining = 0;
            }
        }

        PickedItemSlot.UpdateImage();
        PickedItemSlot.UpdateAmountText();

        int pickUp = intobj.ammount - amountRemaining;

        ShowMsg(new string[3] { "Picked up x" + pickUp.ToString() + " of " + intobj.nameInLanguage[0],
            "Has recogido x" + pickUp.ToString() + " de " + intobj.nameInLanguage[1],
            "Has recollit x" + pickUp.ToString() + " de " + intobj.nameInLanguage[2] });

        if (amountRemaining == 0)
        {
            PhotonNetwork.Destroy(intobj.gameObject);
        }
        else
        {
            intobj.ammount = amountRemaining;
            intobj.UpdateMesh();
        }
        wannaPickup = false;
    }


    void InventoryClosed(bool withEsc)
    {
        if (PickedItemSlot.Item != null)
        {
            int amountRemaining = PickedItemSlot.Item.objAmount;

            for (int i = 0; i < 16 && amountRemaining > 0; i++)
            {
                if (Slot[i].Item == null)
                {
                    Slot[i].AddItem(PickedItemSlot.Item, PickedItemSlot.Item.objAmount);
                    amountRemaining = 0;
                }
                else
                {
                    if (Slot[i].Item.ID == PickedItemSlot.Item.ID)
                    {
                        if (Slot[i].Item.objAmount < Slot[i].Item.ObjMaxAmmount)
                        {
                            int toFill = Slot[i].Item.ObjMaxAmmount - Slot[i].Item.objAmount;

                            if (amountRemaining <= toFill)
                            {
                                Slot[i].Item.objAmount += amountRemaining;
                                amountRemaining = 0;
                            }
                            else
                            {
                                Slot[i].Item.objAmount += toFill;
                                amountRemaining -= toFill;
                            }
                        }
                    }
                }

                Slot[i].UpdateAmountText();
                Slot[i].UpdateImage();
            }

            PickedItemSlot.UpdateImage();
            PickedItemSlot.UpdateAmountText();

            if (amountRemaining == 0)
            {
                PickedItemSlot.RemoveItem();
            }
            else
            {
                DropItem(PickedItemSlot);
            }
        }
    }

    public void DropItem(InventorySlot slot)
    {
        if (slot.Item == null) return;

        GameObject droppedItem = PhotonNetwork.Instantiate(slot.Item.ID.ToString(), DroppingZone.position, Quaternion.identity);
        droppedItem.GetComponent<Rigidbody>().AddForce(DroppingZone.forward * 5, ForceMode.VelocityChange);
        droppedItem.GetComponent<PickableObject>().ammount = slot.Item.objAmount;

        ShowMsg(new string[3] { "Dropped x" + slot.Item.objAmount + " of " + droppedItem.GetComponent<PickableObject>().nameInLanguage[0],
            "Has soltado x" + slot.Item.objAmount + " de " + droppedItem.GetComponent<PickableObject>().nameInLanguage[1],
            "Has llençat x" + slot.Item.objAmount + " of " + droppedItem.GetComponent<PickableObject>().nameInLanguage[2] });

        slot.RemoveItem();
        if (slot.isPlayerSlot) InGameEvents.current.UnEquiped(int.Parse(slot.name.ToCharArray()[10].ToString()));
    }

    void UpdateBar()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerIGSlot[i].gameObject.SetActive(true);

            if (PlayerIINVSlot[i].Item != null)
                PlayerIGSlot[i].AddItem(PlayerIINVSlot[i].Item, PlayerIINVSlot[i].Item.objAmount);
            else if (PlayerIGSlot[i] != null)
                PlayerIGSlot[i].RemoveItem();


            PlayerIGSlot[i].UpdateAmountText();
            PlayerIGSlot[i].UpdateImage();

            if (PlayerIGSlot[i].Item == null)
            {
                PlayerIGSlot[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateIINVBar()
    {
        for (int i = 0; i < PlayerIINVSlot.Length; i++)
        {
            if (PlayerIGSlot[i].Item != null)
                PlayerIINVSlot[i].AddItem(PlayerIGSlot[i].Item, PlayerIGSlot[i].Item.objAmount);
            else if (PlayerIINVSlot[i] != null)
                PlayerIINVSlot[i].RemoveItem();

            PlayerIINVSlot[i].UpdateAmountText();
            PlayerIINVSlot[i].UpdateImage();
        }
    }

    void ShowMsg(string[] txt)
    {
        GameObject ins = Instantiate(Msg, MsgZone);
        ins.GetComponent<TextMeshProUGUI>().SetText(txt[ConfigurationUIManager.current.language]);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView.Controller != photonView.Controller && !wannaPickup)
        {
            return;
        }

        PickableAproved();
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        wannaPickup = false;
    }
}
