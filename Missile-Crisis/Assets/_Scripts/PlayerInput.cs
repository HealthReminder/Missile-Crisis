using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

    //OAEvent is an Unity Event that returns one argument
[System.Serializable]
public class OAEvent : UnityEvent<float>
{

}
[SerializeField]    
public class KeyEvent : UnityEvent<bool>
{

}
public class PlayerInput : MonoBehaviour
{
   //This is how the input manager works
    //All the inputs will be set in update and 
    //Will evoke unity events that can be set in each scene
    //For each mini-game for example
    public PhotonView photonView;
    public int isOn = 0;
    public OAEvent horizontalAxis, verticalAxis;
    public UnityEvent onMouseDown;


    void Update()   {
        if(!photonView.IsMine)
            return;
        if(isOn == 0)
            return;
        //Can be responsible for jumping or doing single time actions e.g

        //In the other hand the horizontal axis will invoke with its own value
        //Can be responsible for moving horizontally or handling rotatins on the X axis e.g

        if(Input.GetAxis("Horizontal") !=0 && horizontalAxis != null)
        {
            horizontalAxis.Invoke(Input.GetAxisRaw("Horizontal"));
            Debug.Log("Horizontal axis = "+ Input.GetAxisRaw("Horizontal"));
        }

        //Can be responsible for moving vertically or handling rotations on the Z axis e.g
        if(Input.GetAxis("Vertical") !=0 && verticalAxis != null)
        {
            verticalAxis.Invoke(Input.GetAxisRaw("Vertical"));
            //Debug.Log("Vertical axis = "+ Input.GetAxisRaw("Vertical"));
        }

        //Can be responsible for clicking down on map cells e.g
        if(Input.GetMouseButtonDown(0) && onMouseDown != null)
        {
            onMouseDown.Invoke();
            //Debug.Log("Vertical axis = "+ Input.GetAxisRaw("Vertical"));
        }
    }
}
