using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]	public class StaticMap {
		public int type = 0;
        public float perlin = 0;
        public Vector2[] adjacent_land;
}
[System.Serializable]	public class DynamicMap {
	public int owner_id;
    public bool is_capital;
    public Vector2 coordinates;
}
[System.Serializable] public class MapCellData{
    //Type
    public int owner_id;
    //Conditions
    public bool has_silo;
	public bool is_nuked;
    
}


public static class MapGenerator 
{
#region Synchronize Map
    public static MapCellData[,] GetMapData(StaticMap[,] s_map,DynamicMap[,] d_map) {
        int xsize = d_map.GetLength(0);
        int ysize = d_map.GetLength(1);
        MapCellData[,] new_map = new MapCellData[xsize,ysize];
        for (int y = 0; y < ysize; y++)
            for (int x = 0; x < xsize; x++)
                new_map[x,y] = new MapCellData { has_silo = false, is_nuked = false };
        
        for (int y = 0; y < ysize; y++)
            for (int x = 0; x < xsize; x++){
                //new_map[x,y].type = s_map[x,y].type;
                new_map[x,y].owner_id = d_map[x,y].owner_id;
                //new_map[x,y].adjacent_land = s_map[x,y].adjacent_land;
            }
        Debug.Log("Amassed map of size " + xsize + "/" + ysize);
        return(new_map);
    }
#endregion
#region Dynamic Map
    protected class DynamicPlayer {
        public Vector2 capital_coord;
        public List<DynamicMap> occupied_cells;
        public List<DynamicMap> empty_cells;
        public void OccupyNeighbour() {
           // if(empty_cells.Count > 0)
              //  for (int i = 0; i < empty_cells.Count; i++)
                //{
                    //if(empty_cells[i].owner_id == -1)
               // }
        }
    }
    public static DynamicMap[,] GenerateValidDynamicMap(StaticMap[,] static_map, int player_quantity, int random_seed){
        //Create the players
        Random.InitState(random_seed);
        DynamicPlayer[] players = new DynamicPlayer[player_quantity];
        for (int i = 0; i < player_quantity; i++)
        {
            players[i] = new DynamicPlayer();
        }
        //Generate an invalid map
        DynamicMap[,] new_map = GenerateDynamicMap(static_map,players,player_quantity);
        
        //Smooth
        for (int k = 0; k < 3; k++)
            new_map = SmoothDynamicMap(new_map,static_map);

        return (new_map);
    }
    static DynamicMap[,] GenerateDynamicMap(StaticMap[,] static_map,DynamicPlayer[] players, int player_quantity) {
        int sizex = static_map.GetLength(0);
        int sizey = static_map.GetLength(1);
        DynamicMap[,] new_map = new DynamicMap[sizex,sizey];
        for (int y = 0; y < sizey; y++)
            for (int x = 0; x < sizex; x++)
            {
                DynamicMap new_cell = new DynamicMap();
                new_cell.is_capital = false;
                new_cell.owner_id = -1;
                new_cell.coordinates = new Vector2(x,y);
                new_map[x,y] = new_cell;
            }
        //Get the empty cells 
        List<DynamicMap> empty_cells = new List<DynamicMap>();
        for (int y = 0; y < sizey; y++)
            for (int x = 0; x < sizex; x++)
                if(static_map[x,y].type == 1)
                    empty_cells.Add(new_map[x,y]);
        Debug.Log("Generated empty cells list with "+empty_cells.Count);
        //Each player will receive a capital that will be removed from the empty cells general list
        List<Vector2> capitals = new List<Vector2>();
        for (int i = 0; i < players.Length; i++){
            DynamicPlayer new_player = players[i];
            new_player.occupied_cells = new List<DynamicMap>();
            int r = -1;
            bool is_valid;
            bool is_done = false;
            while(!is_done) {
                is_valid = true;
                r  = Random.Range(0,empty_cells.Count);
                for(int o = 0; o < capitals.Count; o++)
                {
                    if(Vector2.Distance(capitals[o],empty_cells[r].coordinates) < 25)
                        is_valid = false;
                }
                if(is_valid){
                    capitals.Add(empty_cells[r].coordinates);
                    is_done = true;
                }
            }
            empty_cells[r].is_capital = true;
            new_player.occupied_cells.Add(empty_cells[r]);
            new_player.capital_coord = empty_cells[r].coordinates;
            empty_cells.RemoveAt(r);
            players[i] = new_player;
            Debug.Log("Created a capital for player ID: "+i);
        }
        //Each player will take the empty list and create its own, ordered by the closest to furthest
        for (int i = 0; i < players.Length; i++){
            players[i].empty_cells = new List<DynamicMap>();
            List<float> distances = new List<float>();
            for (int o = 0; o < empty_cells.Count; o++)
            {
                float distance_from_capital = Vector2.Distance(empty_cells[o].coordinates,players[i].capital_coord);
                int add_at = 0;
                for (int p = 0; p < players[i].empty_cells.Count; p++)
                    if(distance_from_capital < distances[p]){
                        add_at = p;
                        p = players[i].empty_cells.Count;
                    } else 
                        add_at = p+1;
                
                players[i].empty_cells.Insert(add_at,empty_cells[o]);
                distances.Insert(add_at,distance_from_capital);
            }
            Debug.Log("Created empty list for player id "+i+" with "+players[i].empty_cells.Count+ " 1st "+
                        players[i].empty_cells[0].coordinates+" last "+players[i].empty_cells[players[i].empty_cells.Count-1].coordinates);
        }
        //Enquanto estiverem celulas vazias
        while(empty_cells.Count > 0) {
            //Pra cada jogador até acabarem as células vazias
            for (int id = 0; id < player_quantity; id++) {
                int provinces_it_had = players[id].occupied_cells.Count;
                //For each occupied cells
                DynamicPlayer current_player = players[id];
                DynamicMap checking_dynamic;
                StaticMap checking_static;
                bool found_neighbour = false;
                for (int c = current_player.occupied_cells.Count - 1; c >= 0 ; c--) {
                    checking_dynamic = current_player.occupied_cells[c];
                    checking_static = static_map[(int)checking_dynamic.coordinates.x,(int)checking_dynamic.coordinates.y]; 
                    //Check if it has any available neighbours
                    List<DynamicMap> neighbours = new List<DynamicMap>();
                    for (int i = 0; i < checking_static.adjacent_land.Length; i++)
                        neighbours.Add(new_map[(int)checking_static.adjacent_land[i].x,(int)checking_static.adjacent_land[i].y]);
                    bool has_available_neighbour = false;
                    while(neighbours.Count > 0) {
                        int r = Random.Range(0,neighbours.Count);
                        DynamicMap checking_neighbour = neighbours[r];
                        neighbours.RemoveAt(r);
                        if(!found_neighbour)
                        if(checking_neighbour.owner_id == -1 && checking_neighbour.owner_id != id) {
                            //Found available neighbour
                            checking_neighbour.owner_id = id;
                            current_player.occupied_cells.Add(checking_neighbour);
                            //Remove it from general and private list
                            for (int i = empty_cells.Count - 1; i >= 0 ; i--)
                                if(empty_cells[i] == checking_neighbour)
                                    empty_cells.RemoveAt(i);
                            for (int i = 0; i < players.Length; i++)
                                for (int o = players[i].empty_cells.Count - 1; o >= 0 ; o--)
                                    if(players[i].empty_cells[o] == checking_neighbour)
                                        players[i].empty_cells.RemoveAt(o);
                            c = -1;
                            found_neighbour = true;
                            has_available_neighbour = true;
                        } 
                    }
                    //If not, remove it from list
                    if(!has_available_neighbour)
                        current_player.occupied_cells.RemoveAt(c);
                    
                }
                if(current_player.empty_cells.Count > 0)
                if(!found_neighbour) { 
                    if(current_player.empty_cells[0].owner_id == -1) {
                        //Get a random cell from its ordered list
                    DynamicMap new_cell = current_player.empty_cells[0];
                    new_cell.owner_id = id;
                    current_player.occupied_cells.Add(new_cell);
                    //Remove it from the general list
                    for (int i = empty_cells.Count - 1; i >= 0 ; i--)
                        if(empty_cells[i] == new_cell)
                            empty_cells.RemoveAt(i);
                    for (int i = 0; i < players.Length; i++)
                        for (int o = players[i].empty_cells.Count - 1; o >= 0 ; o--)
                            if(players[i].empty_cells[o] == new_cell)
                                players[i].empty_cells.RemoveAt(o);
                    
                    }
                }
                //Debug.Log("Player of id "+id+" from "+provinces_it_had+ " to "+players[id].occupied_cells.Count+ " gain "+(players[id].occupied_cells.Count-provinces_it_had));
            }
        }
        return(new_map);
    }
    struct Influence
    {
        public int owner_id;
        public int quantity;
    }
    static DynamicMap[,] SmoothDynamicMap(DynamicMap[,] smoothing_map, StaticMap[,] static_map) {
        int xsize = smoothing_map.GetLength(0);
        int ysize = smoothing_map.GetLength(1);
        DynamicMap[,] result = new DynamicMap[xsize,ysize];
        for (int y = 0; y < ysize; y++)
            for (int x = 0; x < xsize; x++)
                result[x,y] = smoothing_map[x,y];
        
        for (int y = 0; y < ysize; y++)
            for (int x = 0; x < xsize; x++){
                //Get influences over this cell and store it
                DynamicMap d = result[x,y];
                StaticMap s = static_map[x,y];
                List<Influence> influences = new List<Influence>();
                Vector2[] adjacents = {new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),new Vector2(1,-1),new Vector2(0,-1),new Vector2(-1,-1),new Vector2(-1,0),new Vector2(-1,1)};
                //if(s.adjacent_land != null){
                if(d.owner_id != -1)
                    for (int i = 0; i < adjacents.Length; i++)
                    {   
                        int cx = (int)(adjacents[i].x + d.coordinates.x);
                        int cy = (int)(adjacents[i].y + d.coordinates.y);
                        if(CheckIfCellExists(static_map,cx,cy)){
                            if(result[cx,cy].owner_id != -1)  {  
                                DynamicMap c = result[cx,cy];
                                bool created_already = false;
                                for (int o = 0; o < influences.Count; o++)
                                    if(influences[o].owner_id == c.owner_id){
                                        influences[o] = new Influence{owner_id = influences[o].owner_id,quantity = influences[o].quantity +1};
                                        created_already = true;
                                    }
                                if(!created_already)
                                    influences.Add(new Influence{owner_id = c.owner_id, quantity = 0});  
                            }
                        }                
                    }
                    //Find the most influential player on this cell
                    Influence influencer = new Influence{owner_id = d.owner_id,quantity = -1};
                    for (int i = 0; i < influences.Count; i++){
                        if(influences[i].quantity > influencer.quantity)
                            influencer = influences[i];
                        else if(influences[i].quantity == influencer.quantity){
                            int coin = Random.Range(0,2);
                            if(coin == 0)
                                influencer = influences[i];
                        }
                    }
                    d.owner_id = influencer.owner_id;
                //}
            }

        return(result);
    }
    #endregion
#region Static Map
    public static StaticMap[,] GenerateValidStaticMap(int size, int smooth_iterations, int min_lan_percentage, int random_seed) {
        Random.InitState(random_seed);
        StaticMap[,] new_map = GenerateMap(size);
        //Return new map here to avoid validation
        //This is useful in case you change the default size of the map
        
        //Check validity
        int max_check_iterations = 50;
        float min_lan = 100/min_lan_percentage;
        while(!IsStaticMapValid(new_map,min_lan)) {
            new_map = GenerateMap(size);
            max_check_iterations--;
            if(max_check_iterations<=0)
                break;
        }

        //Smooth map
        for (int c = 0; c < smooth_iterations; c++)
            new_map = SmoothStaticMap(new_map);

        //Get neighbours
        new_map = SetAdjacentLand(new_map);

        return (new_map);
    }
    static StaticMap[,] SetAdjacentLand (StaticMap[,] map) {
        int sizex = map.GetLength(0);
        int sizey = map.GetLength(1);
        StaticMap[,] new_map = new StaticMap[sizex,sizey];
        for (int y = 0; y < sizey; y++)
            for (int x = 0; x < sizex; x++)
                new_map[x,y] = map[x,y];
        for (int y = 0; y < sizey; y++)
            for (int x = 0; x < sizex; x++)
                if(new_map[x,y].type == 1){
                    Vector2[] neighbours = new Vector2[4];
                    neighbours[0] = new Vector2(x+1,y);
                    neighbours[1] = new Vector2(x-1,y);
                    neighbours[2] = new Vector2(x,y+1);
                    neighbours[3] = new Vector2(x,y-1);
                    List<Vector2> found = new List<Vector2>();
                    for (int i = 0; i < neighbours.Length; i++){
                        int xindex = (int)neighbours[i].x;
                        int yindex = (int)neighbours[i].y;
                        if(CheckIfCellExists(new_map,xindex,yindex))
                            if(new_map[xindex,yindex].type == 1)
                                found.Add(new Vector2(xindex,yindex));
                    }
                    new_map[x,y].adjacent_land = found.ToArray();
                }
        return(new_map);
    }
    static StaticMap[,] SmoothStaticMap(StaticMap[,] smoothing_map) {
        Debug.Log("Smoothing map");
        //Generates a map with perlin noise
        //No smoothness or validity
        int sy = smoothing_map.GetLength(1);
        int sx = smoothing_map.GetLength(0);
        StaticMap[,] smoothen_map = new StaticMap[sx,sy];
        for (int y = 0; y < sy; y++)
            for (int x = 0; x < sx; x++)
                smoothen_map[x,y] = smoothing_map[x,y];
        for (int y = 0; y < sy; y++){
            for (int x = 0; x < sx; x++){
                bool is_edge = false;
                Vector2[] neighbours = new Vector2[8];
                neighbours[0] = new Vector2(x+1,y);
                neighbours[1] = new Vector2(x-1,y);
                neighbours[2] = new Vector2(x,y+1);
                neighbours[3] = new Vector2(x,y-1);
                neighbours[2] = new Vector2(x+1,y+1);
                neighbours[3] = new Vector2(x-1,y-1);
                neighbours[2] = new Vector2(x+1,y-1);
                neighbours[3] = new Vector2(x-1,y+1);
                //Clear cell if it is on edge
                foreach (Vector2 v in neighbours)
                    if(!CheckIfCellExists(smoothen_map,(int)v.x,(int)v.y))
                        is_edge = true;
                if(is_edge)
                    smoothen_map[x,y].type = 0;
                else{
                    int land_around = 0;
                    foreach (Vector2 v in neighbours)
                        if(smoothen_map[(int)v.x,(int)v.y].type == 1)
                            land_around++;          
                    
                    bool is_water = false;
                    int r = Random.Range(0,100);
                    int maximum;
                    if(land_around == 0)
                        maximum = 100;
                    else if(land_around == 1)
                        maximum = 90;
                    else if(land_around == 2)
                        maximum = 70;
                    else if(land_around == 3)
                        maximum = 10;
                    //else if(land_around == 4)
                    //    maximum = 0;
                    //else if(land_around == 5)
                    //    maximum = 0;
                    //else if(land_around == 6)
                    //    maximum = 0;
                    //else if(land_around == 7)
                    //    maximum = 0;
                    else 
                        maximum = 0;
                    if(r < maximum)
                        is_water = true;
                    if(is_water)
                        smoothen_map[x,y].type = 0;
                    
                    


                }
                
            }
        }
        return(smoothen_map);
    }
    static bool CheckIfCellExists(StaticMap[,] map,int xcoord, int ycoord) {
        if(xcoord >= map.GetLength(0) || xcoord < 0 || ycoord >= map.GetLength(1) || ycoord < 0)
            return (false);
        else
            return(true);
    }
    static StaticMap[,] GenerateMap(int size) {
        //Generates a map with perlin noise
        //No smoothness or validity
        StaticMap[,] new_map = new StaticMap[size*2,size];
        float xseed = Random.Range(0f,100f);
        float yseed = Random.Range(0f,100f);
        xseed = Random.Range(0f,1f);
            yseed = Random.Range(0f,1f);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size*2; x++)
            {
                new_map[x,y] = new StaticMap();
                float cell_perlin = Mathf.PerlinNoise(((float)x/35)+xseed,((float)y/35)+yseed)*2;
                new_map[x,y].perlin = cell_perlin;
            }
        xseed = Random.Range(0f,100f);
        yseed = Random.Range(0f,100f);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size*2; x++)
            {
                float cell_perlin = Mathf.PerlinNoise(new_map[x,y].perlin*x/15+xseed,y/15+yseed)*new_map[x,y].perlin;
                new_map[x,y].perlin = cell_perlin;
                if(cell_perlin >= 0.4f)
                    new_map[x,y].type = 1;
                else 
                    new_map[x,y].type = 0;
            }
        return(new_map);
    }
    static bool IsStaticMapValid(StaticMap[,] m, float target_lan) {
        float current_land = 0;
        int sy = m.GetLength(1);
        int sx = m.GetLength(0);
        float land_cost = 1/((float)sy*(float)sx);
        for (int i = 0; i < sy; i++)
            for (int o = 0; o < sx; o++)
                if(m[o,i].type == 1)
                    current_land += land_cost;
        //Debug.Log("This map has " + current_land+" usable land");
        if(current_land >= target_lan)
            return(true);
        else
            return(false);
    }
    #endregion
}
