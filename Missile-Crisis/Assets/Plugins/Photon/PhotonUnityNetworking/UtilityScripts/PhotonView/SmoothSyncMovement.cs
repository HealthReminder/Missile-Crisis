// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmoothSyncMovement.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Smoothed out movement for network gameobjects
// </summary>                                                                                             
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Smoothed out movement for network gameobjects
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class SmoothSyncMovement : Photon.Pun.MonoBehaviourPun, IPunObservable
    {
        private Rigidbody2D rb;
        public float SmoothingDelay = 5;
        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            bool observed = false;
            foreach (Component observedComponent in this.photonView.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    observed = true;
                    break;
                }
            }
            if (!observed)
            {
                Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //We own this player: send the others our data
                stream.SendNext(new Vector2(transform.position.x,transform.position.y));
                //stream.SendNext(transform.rotation);
                stream.SendNext(rb.velocity);
            }
            else
            {
                //Network player, receive data
                correctPlayerPos = (Vector2)stream.ReceiveNext();
                //correctPlayerRot = (Quaternion)stream.ReceiveNext();
                correctRigidbodyVelocity = (Vector2)stream.ReceiveNext();
            }
        }

        private Vector2 correctRigidbodyVelocity = Vector3.zero;
        private Vector2 correctPlayerPos = Vector3.zero; //We lerp towards this
        //private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

        public void Update()
        {
            if (!photonView.IsMine)
            {
                //Update remote player (smooth this, this looks good, at the cost of some accuracy)
                transform.position = Vector3.Lerp(transform.position, new Vector3(correctPlayerPos.x,correctPlayerPos.y,0), Time.deltaTime * this.SmoothingDelay);
                //transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
                rb.velocity = Vector2.Lerp(rb.velocity,correctRigidbodyVelocity, Time.deltaTime * this.SmoothingDelay);
            }
        }

    }
}