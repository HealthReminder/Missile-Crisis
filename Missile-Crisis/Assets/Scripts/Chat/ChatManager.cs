using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChatManager : MonoBehaviour
{
    public PhotonView photon_view;
    public bool is_on = false;
    [SerializeField] GameObject chat_container;
    [SerializeField] Transform chat_content;
    [SerializeField] GameObject chat_entry;
    [SerializeField] List<ChatEntry> current_entries;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.L))
            ToggleGUI(!is_on);
    }
    void ToggleGUI(bool on){
        is_on = on;
        if(on){
            chat_container.gameObject.SetActive(true);
        } else {
            chat_container.gameObject.SetActive(true);
        }
    }
#region During Adventure
    public void AddEntry(string player_name,string message,string color_name,string color_message, bool is_online){
        byte[] p_bytes = System.Text.Encoding.UTF8.GetBytes(player_name);
        byte[] m_bytes = System.Text.Encoding.UTF8.GetBytes(message);
        byte[] cn_bytes = System.Text.Encoding.UTF8.GetBytes(color_name);
        byte[] cm_bytes = System.Text.Encoding.UTF8.GetBytes(color_message);
        if(is_online)
            photon_view.RPC("RPC_AddEntry",RpcTarget.All,p_bytes,m_bytes,cn_bytes,cm_bytes);
        else
            RPC_AddEntry(p_bytes,m_bytes,cn_bytes,cm_bytes);
    } 
    [PunRPC]void RPC_AddEntry(byte[] p_bytes, byte[] m_bytes,byte[] cn_bytes, byte[] cm_bytes){
        string player_name = System.Text.Encoding.UTF8.GetString(p_bytes);
        string message = System.Text.Encoding.UTF8.GetString(m_bytes);
        string color_name = System.Text.Encoding.UTF8.GetString(cn_bytes);
        string color_message = System.Text.Encoding.UTF8.GetString(cm_bytes);
        //Instantiate object
        //Add entry
        GameObject new_obj = Instantiate(chat_entry,transform.position,Quaternion.identity);
        new_obj.transform.parent = chat_content;
        ChatEntry new_entry = new_obj.GetComponent<ChatEntry>();
        current_entries.Add(new_entry);

        new_entry.text.text = "<color="+color_name+">"+player_name+"</color>: "+"<color="+color_message+">"+message+"</color>";

        //After everything is done, turn it on
        new_obj.SetActive(true);  
    }
#endregion

#region On Start
public static ChatManager instance;
    private void Awake() {
        instance = this;
        Setup();
    }
    private void Setup(){
        current_entries = new List<ChatEntry>();
    }
#endregion

}
