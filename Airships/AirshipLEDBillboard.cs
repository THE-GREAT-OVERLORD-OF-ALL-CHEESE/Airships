using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AirshipLEDBillboard : MonoBehaviour
{
    public Renderer renderer;
    public int scrollingMaterialID;

    private Material material;

    public float screenAspectRatio;
    public Texture texture;
    public float scrollSpeed;

    private float imageAspect;

    private float offset;

    private void Start()
    {
        material = renderer.materials[scrollingMaterialID];
        SetTexture(material.GetTexture("_EmissionMap"));
    }

    private void Update()
    {
        offset += Time.deltaTime * scrollSpeed;
        material.SetTextureOffset("_MainTex", new Vector2((offset / imageAspect) * screenAspectRatio, 0));
    }

    public void SetTexture(Texture newTexture)
    {
        texture = newTexture;
        material.SetTexture("_EmissionMap", texture);

        imageAspect = (float)texture.width / (float)texture.height;
        material.SetTextureScale("_MainTex", new Vector2((1.0f / imageAspect) * screenAspectRatio, 1));
    }
}
