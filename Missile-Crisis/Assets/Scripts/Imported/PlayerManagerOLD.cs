using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
[System.Serializable]   public class PlayerManagerOLD : MonoBehaviour
{
    public int playerID;
    public int photonViewID;
    public PhotonView photonView;
    public PlayerInput playerInput;

    public int canPlaceSilos = 0;
    public int missileSilosLeft = 0;
    public int missileProjectilesLeft = 0;
    
    public BoardCell currentlySelectedCell;

    private void Start() {
        if(!photonView.IsMine)
            return;
            
        playerInput.isOn = 1;
 
    }
    private void Update() {
        if(!photonView.IsMine)
            return;

        if(Input.GetMouseButtonDown(0)){           
            //currentlySelectedCell = playerController.SelectCell();
            return;       
            if(currentlySelectedCell != null) {
               // DynamicMap dynamicCell = GameData.instance.dynamicMap[(int)currentlySelectedCell.coordinates.x,(int)currentlySelectedCell.coordinates.y];
                //if(dynamicCell.playerId == playerID){
                    if(missileSilosLeft > 0 && canPlaceSilos == 1) {
                        photonView.RPC("RPC_PlaceMissileSilo", RpcTarget.All,BitConverter.GetBytes(currentlySelectedCell.coordinates.x), BitConverter.GetBytes(currentlySelectedCell.coordinates.y) );
                    }
                //} else {
                    if(missileProjectilesLeft > 0) {
                        byte bombSize = (byte)UnityEngine.Random.Range(1,6);
                        GameManager.instance.photon_view.RPC("RPC_ExplodeCells", RpcTarget.All,BitConverter.GetBytes(photonView.ViewID),BitConverter.GetBytes(currentlySelectedCell.coordinates.x), 
                        BitConverter.GetBytes(currentlySelectedCell.coordinates.y),bombSize);
                        //guiController.DropNuclearBomb(bombSize,currentlySelectedCell.transform.position+new Vector3(0,0.5f,0));
                                
                    }                                
                //}
            }
        }
    }

    [PunRPC] void RPC_PlaceMissileSilo(byte[] xB, byte[] yB) {
        int x,y;
        x = (int)BitConverter.ToSingle(xB,0);
        y = (int)BitConverter.ToSingle(yB,0);

        Debug.Log("RPC_PlaceMissileSilo " + x +","+y);
        if(missileSilosLeft <= 0)
            return;
        
        missileSilosLeft --;
        AddSiloToCell(x,y);

        //foreach(PhotonView p in GameManager.instance.listOfPlayersPlaying) {
        //    p.RPC("RPC_UpdateDynamicMap",RpcTarget.All);
        //}
    }

    public void AddSiloToCell(int x, int y) {
        //GameData.instance.dynamicMap[x,y].hasSilo = 1;
        //Debug.Log("Changing game data to " + GameData.instance.dynamicMap[x,y].hasSilo);
    }
    [PunRPC] void RPC_UpdateNextBombGUI(byte[] secondsForNextBomb) {
        Debug.Log("RPC_ReceiveProjectiles");
        //if(photonView.IsMine)
            //StartCoroutine(guiController.FillBombIcon(BitConverter.ToSingle(secondsForNextBomb,0)));
        
    }

    [PunRPC] void RPC_ReceiveProjectiles(byte missileProjectileQuantity) {
        Debug.Log("RPC_ReceiveProjectiles");
        missileProjectilesLeft += (int)missileProjectileQuantity;
        
    }

    [PunRPC] void RPC_ReceiveSilos(byte missileSilosQuantity) {
        Debug.Log("RPC_ReceiveSilos");
        missileSilosLeft = (int)missileSilosQuantity;
        
    }

    [PunRPC] void RPC_ToggleMissilePlacement(byte isOn) {
        Debug.Log("RPC_ToggleMissilePlacement");
        if(photonView.IsMine){
            canPlaceSilos = (int)isOn;
            }
    }
    [PunRPC] void RPC_ShowNotification(byte[] message, byte Duration) {
        Debug.Log("RPC_ShowNotification");
        //if(photonView.IsMine)
            //StartCoroutine(guiController.ShowNotification(System.Text.Encoding.UTF8.GetString(message),(int)Duration));
    }
     [PunRPC] void RPC_ResetTimer(byte startingTime) {
        Debug.Log("RPC_ResetTimer");
        //if(photonView.IsMine)
            //guiController.ResetTimer((int)startingTime);
    }
    [PunRPC] void RPC_UpdateDynamicMap() {
        Debug.Log("RPC_UpdateDynamicMap");
        //if(photonView.IsMine)
            //guiController.UpdateDynamicMap(GameData.instance.dynamicMap,GameData.instance.playersInGame, playerID);
    }
    [PunRPC] void RPC_GenerateStaticMap () {
        Debug.Log("RPC_GenerateStaticMap");
        //if(photonView.IsMine)
            //guiController.GenerateMapView(GameData.instance.staticMap,GameData.instance.playersInGame);
    }
    
}
