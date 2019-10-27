using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    public struct BombAppearance
    {
        public GameObject gameObject;
        public Image image;
        public int type;
    }
    public GameObject bomb_template;
    public Transform inventory_container;
    public AnimationCurve smooth_curve;
    public List<BombAppearance> bombs;
    private void Awake() {
        Setup();
    }
    void Setup() {
        bombs = new List<BombAppearance>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.T))
            AddBomb(0);
        if(Input.GetKeyDown(KeyCode.Y))
            RemoveBomb(0);
    }

    public void AddBomb(int type) {
        BombAppearance new_bomb = new BombAppearance();
        new_bomb.type = type;
        new_bomb.gameObject = Instantiate(bomb_template, inventory_container.position, Quaternion.identity);
        new_bomb.image = new_bomb.gameObject.transform.GetChild(0).GetComponent<Image>();
        new_bomb.image.transform.parent = inventory_container;
        new_bomb.gameObject.SetActive(true);
        bombs.Add(new_bomb);
        Debug.Log("Added a bomb");
    }

    public void RemoveBomb(int type) {
        Debug.Log("Removed a bomb");
        for (int i = 0; i < bombs.Count; i++)
            if(bombs[i].type == type){
                Destroy(bombs[i].image.gameObject);
                bombs.RemoveAt(i);
                Debug.Log("Removed a bomb");
                break;
            }
    }

}
