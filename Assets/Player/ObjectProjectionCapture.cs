using UnityEngine;

public class ObjectProjectionCapture : MonoBehaviour
{
    public Camera playerCamera;
    public Camera captureCamera;

    public int textureSize = 1024;

    public Texture2D CaptureObject(GameObject target)
    {
        // match player camera
        captureCamera.transform.position = playerCamera.transform.position;
        captureCamera.transform.rotation = playerCamera.transform.rotation;

        // create render texture
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 24);
        captureCamera.targetTexture = rt;

        // render
        captureCamera.Render();

        // convert to Texture2D
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        captureCamera.targetTexture = null;

        Destroy(rt);

        return tex;
    }
}