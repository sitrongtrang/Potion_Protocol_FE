using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadingScene.Instance.RenderLoadingScene());
        MapLoader.Instance.RenderMap(new Vector3(0, 0, 0), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
