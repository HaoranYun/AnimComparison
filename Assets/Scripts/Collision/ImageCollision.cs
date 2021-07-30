using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class ImageCollision : MonoBehaviour
{
    private readonly Vector2Int Resolution = new Vector2Int(480, 270);

    //public Transform Wall;
    public Shader CollisionShader;
    public ComputeShader ImageCollisionCS;

    private Camera Camera;
    private int CollisionRenderLayer;

    //private RenderTexture WallRT;
    private RenderTexture SilhouetteRT;
    private RenderTexture PlayerRT;
    private RenderTexture ResultRT;
    private Texture2D ResultTex;

    // Compute Shader
    private readonly Vector3Int ThreadGroup = new Vector3Int(8, 8, 1);
    private int IntersectionKernel;
    private int SilhouetteRTKey;
    private int PlayerRTKey;
    private int ResultKey;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        CollisionRenderLayer = LayerMask.NameToLayer("CollisionRender");

        InitCamera();
        InitRenderTextures();
        InitComputeShader();
    }

    public float ComputeIntersection(Image img, Transform shadowPlayer, Transform currentSilhouette)
    {
        // 1. Render Plane/Wall to a RT
        //RenderObject(Wall.gameObject, WallRT);

        // 2. Render Big Jackie to a RT
        RenderObject(currentSilhouette.gameObject, SilhouetteRT);

        // 3. Render Player to a RTs
        RenderObject(shadowPlayer.gameObject, PlayerRT);

        // 4. Dispatch Compute Shader and create a Texture with the intersection
        ImageCollisionCS.SetTexture(IntersectionKernel, SilhouetteRTKey, SilhouetteRT);
        ImageCollisionCS.SetTexture(IntersectionKernel, PlayerRTKey, PlayerRT);
        ImageCollisionCS.SetTexture(IntersectionKernel, ResultKey, ResultRT);
        ImageCollisionCS.SetInts("Resolution", Resolution.x, Resolution.y);

        Vector3Int numBlocks = new Vector3Int(Mathf.CeilToInt((float)Resolution.x / ThreadGroup.x), Mathf.CeilToInt((float)Resolution.y / ThreadGroup.y), 1);
        ImageCollisionCS.Dispatch(IntersectionKernel, numBlocks.x, numBlocks.y, numBlocks.z);

        // 5. Readback the result to CPU
        RenderTexture last = RenderTexture.active;
        RenderTexture.active = ResultRT;
        ResultTex.ReadPixels(new Rect(0, 0, Resolution.x, Resolution.y), 0, 0);
        ResultTex.Apply(); // GPU to CPU
        RenderTexture.active = last;

        // 6. Process result
        int red = 0;
        int white = 0;
        Color[] colors = ResultTex.GetPixels(0, 0, Resolution.x, Resolution.y);
        for (int i = 0; i < Resolution.y * Resolution.x; ++i)
        {
            red += colors[i].r > 0.5f && colors[i].g < 0.5f ? 1 : 0;
            white += colors[i].r > 0.5f && colors[i].g > 0.5f ? 1 : 0;
        }

        Sprite resultSprite = Sprite.Create(ResultTex, new Rect(0f, 0f, ResultTex.width, ResultTex.height), new Vector2(0.5f, 0.5f), 100.0f);

        img.sprite = resultSprite;

        return (float)white / (red + white);
    }


    private void RenderObject(GameObject obj, RenderTexture rt)
    {
        int layer = obj.layer;
        ChangeLayerRecursively(obj.transform, CollisionRenderLayer);
        Camera.targetTexture = rt;
        Camera.RenderWithShader(CollisionShader, "");
        ChangeLayerRecursively(obj.transform, layer);
    }

    private void ChangeLayerRecursively(Transform root, int layer)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = layer;
        }
    }

    private void InitCamera()
    {
        Camera.cullingMask = 1 << CollisionRenderLayer;
        Camera.clearFlags = CameraClearFlags.SolidColor;
        Camera.backgroundColor = Color.black;
    }

    private void InitRenderTextures()
    {
        //WallRT = new RenderTexture(Resolution.x, Resolution.y, 0);
        //WallRT.Create();
        SilhouetteRT = new RenderTexture(Resolution.x, Resolution.y, 0);
        SilhouetteRT.Create();
        PlayerRT = new RenderTexture(Resolution.x, Resolution.y, 0);
        PlayerRT.Create();
        ResultRT = new RenderTexture(Resolution.x, Resolution.y, 0)
        {
            enableRandomWrite = true
        };
        ResultRT.Create();
        ResultTex = new Texture2D(Resolution.x, Resolution.y);
    }

    private void InitComputeShader()
    {
        IntersectionKernel = ImageCollisionCS.FindKernel("IntersectionKernel");
        SilhouetteRTKey = Shader.PropertyToID("SilhouetteRT");
        PlayerRTKey = Shader.PropertyToID("PlayerRT");
        ResultKey = Shader.PropertyToID("Result");
    }

    private void OnDestroy()
    {
        //if (WallRT != null) WallRT.Release();
        if (SilhouetteRT != null) SilhouetteRT.Release();
        if (PlayerRT != null) PlayerRT.Release();
        if (ResultRT != null) ResultRT.Release();
    }
}
