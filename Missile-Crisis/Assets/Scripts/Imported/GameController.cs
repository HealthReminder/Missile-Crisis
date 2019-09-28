using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]	public class StaticMapOLD {
		public int type = 0;
}
[System.Serializable]	public class DynamicMapOLD {
	public int playerId;
	public int hasSilo = 0;
	public int isDestroyed;
}

public class GameController : MonoBehaviour
{
   [HideInInspector]
	public static GameController instance;
	void Awake()	{	
		//Make it the only one
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}

	}

     public void DestroyCellsInRadius(DynamicMapOLD[,] currentDynamicMap, Vector2 coordinates, int radius) {
        int x = (int)coordinates.x;
        int z = (int)coordinates.y;


        for (z = -(radius); z <= radius; z++)
        {
            for (x = -(radius); x <= radius; x++)
            {
                Vector2 currentCellCoordinates = new Vector2(coordinates.x+x,coordinates.y+z);
                if(CheckIfCellExists(currentCellCoordinates,currentDynamicMap.GetLength(0),currentDynamicMap.GetLength(1))
                && Vector2.Distance(currentCellCoordinates, coordinates) <= radius)
                    currentDynamicMap[(int)currentCellCoordinates.x,(int)currentCellCoordinates.y].isDestroyed = 1;
            }
        }
        
        Debug.Log("A nuclear bomb of power "+radius+" just went off in " + coordinates+ "!!");

    }

    public bool CheckIfCellExists(Vector2 coordinates, int matrixSizeX, int matrixSizeY){
        if(coordinates.x < 0 || coordinates.x >= matrixSizeX)
            return(false);
        else if (coordinates.y < 0 || coordinates.y >= matrixSizeY)
            return(false);
        else 
            return(true);            
    }

    public StaticMapOLD[,] SetupStaticMap(Vector2 size) {
        int xSize = (int)size.x;
        int zSize = (int) size.y;

        StaticMapOLD[,] newMap = new StaticMapOLD[xSize,zSize];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                StaticMapOLD newMapCell = newMap[x,z] = new StaticMapOLD();
                newMapCell.type = 1;

            }
        }
        Debug.Log("Setup static map");
        return(newMap);

    }
    
    public List<PlayerData> SetupPlayerList(int playerQuantity) {
        List<PlayerData> newPlayers = new List<PlayerData>();

        for (int p = 0; p < playerQuantity; p++)
        {
            PlayerData newPlayer = new PlayerData();
            newPlayers.Add(newPlayer);
        }
        Debug.Log("Setup player list");
        return(newPlayers);
    }

    public DynamicMapOLD[,] SetupDynamicMap(Vector2 size, int playerQuantity) {
        int xSize = (int)size.x;
        int zSize = (int) size.y;

        DynamicMapOLD[,] newMap = new DynamicMapOLD[xSize,zSize];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                DynamicMapOLD newMapCell = newMap[x,z] = new DynamicMapOLD();
                Debug.Log(size +" size / pqtd " + playerQuantity+" - "+(int)Mathf.Lerp(0,playerQuantity,(xSize/size.x)));
                newMapCell.playerId = (int)Mathf.Lerp(0,playerQuantity,(x/size.x));
                //newMapCell.playerId = (int)(Random.Range(0,playerQuantity));
               

            }
        }
        Debug.Log("Dynamic static map");
        return(newMap);

    }
}