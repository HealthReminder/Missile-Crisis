﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombView : MonoBehaviour
{
    
    [Header("Movement")]
    public AnimationCurve movement_curve;
    public GameObject missile_obj;
    [Range(0,5)]
    public float height;
    [Header("Core")]
    public bool is_exploded = false;
    public Transform core_transform;
    public Renderer core_renderer;
    public ParticleSystem core_particles;
    public Gradient core_color;
    [Header("Flash")]
    public AnimationCurve smooth_curve;
    public Renderer flash_renderer;
    [Header("Impact")]
    public ParticleSystem impact_particles;
    [Header("Appearance")]
    public TrailRenderer trail;
    public Renderer renderer;
    public void ShowGUI(bool is_on, Color color)
    {
        Debug.Log("ShouwGUI");
        if (is_on)
        {
            //Trail
            color.a = 0.75f;
            trail.startColor = color + new Color(0.1f, 0.1f, 0.1f, 0);
            trail.endColor = color + new Color(0.2f, 0.2f, 0.2f, 0);
            //Ogiva Color
            MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", color - new Color(0.2f, 0.2f, 0.2f, -1));
            renderer.SetPropertyBlock(_propBlock);
            Debug.Log("ShouwGUI ON");
        } else
        {
            renderer.gameObject.SetActive(is_on);
            trail.enabled = is_on; Debug.Log("ShouwGUI OFF");
        }
    }

    public IEnumerator LaunchMissile(Vector3 init_pos, Vector3 final_pos, float size) {
        float prog = 0;
        while(prog <= 1) {
            Vector3 new_pos = Vector3.Lerp(init_pos,final_pos,prog);
            new_pos.y = movement_curve.Evaluate(prog)*height;
            transform.position = new_pos;
            prog += Time.deltaTime;
            yield return null;
        }

        Explode(size);
        //yield return new WaitForSeconds(Random.Range(8f,10f));
        //Destroy(gameObject);
        yield break;
    }
    void Explode(float size){
        missile_obj.SetActive(false);
        size = size+0.2f;
        is_exploded = true;
        if(AudioController.instance)
            AudioController.instance.PlaySound("Bomb_Explosion",transform.position);
        StartCoroutine(CoreRoutine(0.01f,size));
        StartCoroutine(FlashRoutine(0.005f,size));
    }
    void SendCellsToMatchManager(float size){
        Collider[] colliders = Physics.OverlapSphere(transform.position,size/2);
        //for (int i = 0; i < colliders.Length; i++)
            //colliders[i].transform.position+= new Vector3(0,-5,0);
        List<Vector2> coordinates = new List<Vector2>();
        for (int i = 0; i < colliders.Length; i++)
            if(colliders[i].GetComponent<BoardCell>())
                coordinates.Add(colliders[i].GetComponent<BoardCell>().coordinates);
        //Debug.Log(colliders.Length +" with sphere of size "+size/2);
        if(MatchManager.instance)
            MatchManager.instance.ExplodeCells(coordinates.ToArray());
        
    }
    IEnumerator FlashRoutine(float rate, float size) {

        MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
        flash_renderer.GetPropertyBlock(_propBlock);

        float progress = 0;
        flash_renderer.transform.localScale = new Vector3(1*size*size,1*size*size,1*size*size);
        _propBlock.SetColor("_Color",new Color(1,1,1,0.5f));
        flash_renderer.SetPropertyBlock(_propBlock);
        
        float current_alpha = 0.5f;
        while (true)
        {
            if(current_alpha <= 0)
                break;
            else {
                _propBlock.SetColor("_Color",new Color(1,1,1,current_alpha));
                flash_renderer.SetPropertyBlock(_propBlock);
                current_alpha -= rate;
            }
            yield return null;
        }
        yield break;
    }

    IEnumerator CoreRoutine(float rate, float size) {
        MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
        core_renderer.GetPropertyBlock(_propBlock);
        size *= 3;

        float progress = 0;
        core_transform.localScale = new Vector3(0,0,0);
        _propBlock.SetColor("_Color",new Color(1,1,1,1f));
        core_renderer.SetPropertyBlock(_propBlock);
        core_particles.startSize = size/2;
        core_particles.startSpeed = size;
        impact_particles.startSpeed = size*2;
        core_particles.Play();
        impact_particles.Play();
        bool shaked = false;
        while (true)
        {
            if(core_transform.localScale.x >= size)
                break;
            else {
                float add = size*smooth_curve.Evaluate(progress);
                core_transform.localScale = new Vector3(add,add,add);
                progress += rate;
                if(progress >= 0.5f && !shaked) {
                    shaked = true;
                    if(MatchManager.instance)
                        MatchManager.instance.ShakePlayers();
                }
            }
            yield return null;
        }
        
        SendCellsToMatchManager(size);
        float prog = 0;
        while (true)
        {
            if(prog >=1 )
                break;
            else {
                _propBlock.SetColor("_Color",core_color.Evaluate(prog));
                core_renderer.SetPropertyBlock(_propBlock);
                prog += rate/size*2;
            }
            yield return null;
        }
        Debug.Log("Core dissipated");
        yield break;
    }
}
