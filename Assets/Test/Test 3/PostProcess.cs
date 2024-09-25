using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess : MonoBehaviour
{
    [SerializeField] private Shader _shader;
    [SerializeField] private Material _mat;

    private void OnEnable()
    {
        //_mat = new Material(_shader);
    }

    private void OnDisable()
    {
        //_mat = null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _mat);
    }
}