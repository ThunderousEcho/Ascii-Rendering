using System;
using UnityEngine;

public class AsciiPostProcessing : MonoBehaviour {

    public Material material;
    public Camera actualCamera;
    public new Camera camera;
    public int charWidth = 8;
    public int charHeight = 16;

    public bool autoRowAndColumnCount = true;
    public int columnCount = 200;
    public int rowCount = 50;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(actualCamera.targetTexture, destination, material);
    }
    private void LateUpdate() {

        if (autoRowAndColumnCount) {
            columnCount = Screen.width / charWidth;
            rowCount = Screen.height / charHeight;
        }

        int width = columnCount * charWidth;
        int height = rowCount * charHeight;

        int scale = Math.Min(Screen.width / width, Screen.height / height);
        if (scale == 0) {
            scale = 1;
        }

        width *= scale;
        height *= scale;

        int marginX = (Screen.width - width) / 2;
        int marginY = (Screen.height - height) / 2;

        Rect rect = new Rect(marginX, marginY, width, height);
        camera.pixelRect = rect;
    }
}
