using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualCamera : MonoBehaviour {

    public new Camera camera;

    public AsciiPostProcessing asciiPostProcessing;

    private void Update() {

        if (camera.targetTexture == null || camera.targetTexture.width != asciiPostProcessing.columnCount || camera.targetTexture.height != asciiPostProcessing.rowCount) {

            if (camera.targetTexture != null) {
                camera.targetTexture.Release();
            }
            camera.targetTexture = new RenderTexture(asciiPostProcessing.columnCount, asciiPostProcessing.rowCount, 24);
            camera.targetTexture.filterMode = FilterMode.Point;

            float aspectModifier = asciiPostProcessing.charHeight / (float)asciiPostProcessing.charWidth;
            camera.ResetProjectionMatrix();
            camera.projectionMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(aspectModifier, 1, 1));
        }
    }
}
