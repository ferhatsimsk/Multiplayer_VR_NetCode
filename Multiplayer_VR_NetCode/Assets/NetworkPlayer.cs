using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

public class NetworkPlayer : NetworkBehaviour
{
    //First define some global variables in order to speed up the Update() function
    GameObject myXRRig;
    HandsAndControllersManager HaCM;                    //what mode are we in: controller, hands, none?
    Transform myXRLH, myXRRH, myXRLC, myXRRC, myXRCam;  //positions and rotations
    Transform avHead, avLeft, avRight, avBody;          //avatars moving parts 

    //some fine tuning parameters if needed
    [SerializeField]
    private Vector3 avatarLeftPositionOffset, avatarRightPositionOffset;
    [SerializeField]
    private Quaternion avatarLeftRotationOffset, avatarRightRotationOffset;
    [SerializeField]
    private Vector3 avatarHeadPositionOffset;
    [SerializeField]
    private Quaternion avatarHeadRotationOffset;

    public override void OnNetworkSpawn()
    {
        var myID = transform.GetComponent<NetworkObject>().NetworkObjectId;
        if (IsOwnedByServer)
            transform.name = "Host:" + myID;    //this must be the host
        else
            transform.name = "Client:" + myID; //this must be the client 

        if (!IsOwner) return;

        myXRRig = GameObject.Find("XR Origin");
        if (myXRRig) Debug.Log("Found XR Origin");
        else Debug.Log("Could not find XR Origin!");

        //pointers to the XR RIg
        HaCM = myXRRig.GetComponent<HandsAndControllersManager>();
        myXRLH = HaCM.m_LeftHand.transform.Find("Direct Interactor");
        myXRLC = HaCM.m_LeftController.transform;
        myXRRH = HaCM.m_RightHand.transform.Find("Direct Interactor");
        myXRRC = HaCM.m_RightController.transform;
        myXRCam = GameObject.Find("Main Camera").transform;

        //pointers to the avatar
        avLeft = transform.Find("Left Hand");
        avRight = transform.Find("Right Hand");
        avHead = transform.Find("Head");
        avBody = transform.Find("Body");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (!myXRRig) return;

        switch (HaCM.m_LeftMode)
        {
            case HandsAndControllersManager.Mode.MotionController:
                if (avLeft)
                {
                    avLeft.position = myXRLC.position + avatarLeftPositionOffset;
                    avLeft.rotation = myXRLC.rotation * avatarLeftRotationOffset;
                }
                break;
            case HandsAndControllersManager.Mode.TrackedHand:
                if (avLeft)
                {
                    avLeft.position = myXRLH.position + avatarLeftPositionOffset;
                    avLeft.rotation = myXRLH.rotation * avatarLeftRotationOffset;
                }
                break;

            case HandsAndControllersManager.Mode.None:
                break;
        }

        switch (HaCM.m_RightMode)
        {
            case HandsAndControllersManager.Mode.MotionController:
                if (avRight)
                {
                    avRight.position = myXRRC.position + avatarRightPositionOffset;
                    avRight.rotation = myXRRC.rotation * avatarRightRotationOffset;
                }
                break;
            case HandsAndControllersManager.Mode.TrackedHand:
                if (avRight)
                {
                    avRight.position = myXRRH.position + avatarRightPositionOffset;
                    avRight.rotation = myXRRH.rotation * avatarRightRotationOffset;
                }
                break;

            case HandsAndControllersManager.Mode.None:
                break;
        }

        if (avHead)
        {
            avHead.position = myXRCam.position + avatarHeadPositionOffset;
            avHead.rotation = myXRCam.rotation * avatarHeadRotationOffset;
        }

        if (avBody)
        {
            avBody.position = avHead.position + new Vector3(0, -0.5f, 0);
        }
    }
}
