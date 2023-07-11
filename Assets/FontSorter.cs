using System;
using UnityEditor;
using UnityEngine;

public class FontSorter : EditorWindow {

    Texture2D input;
    int charWidth = 8;
    int charHeight = 16;

    [MenuItem("Window/Font Sorter")]
    static void Init() {
        FontSorter window = (FontSorter)GetWindow(typeof(FontSorter));
        window.Show();
    }

    static T[] SubArray<T>(T[] data, int index, int length) {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }

    void OnGUI() {

        input = EditorGUILayout.ObjectField("Input font texture", input, typeof(Texture2D), true) as Texture2D;
        charWidth = EditorGUILayout.IntField("Character width (px)", charWidth);
        charHeight = EditorGUILayout.IntField("Character height (px)", charHeight);

        if (GUILayout.Button("Sort")) {

            if (input == null) {

                ShowNotification(new GUIContent("No input font texture selected"));

            } else {

                int charColumns = input.width / charWidth;
                int charRows = input.height / charHeight;
                int charCount = charRows * charColumns;

                int[] lightPixelCounts = new int[charCount];
                int[] indices = new int[charCount];

                int emptyCharacters = 0;
                int fullCharacters = 0;

                for (int charIndex = 0; charIndex < charCount; charIndex++) {
                    int lightPixels = countLightPixels(charIndex, charWidth, charHeight, charColumns, charRows);
                    if (lightPixels == charWidth * charHeight) {
                        emptyCharacters++;
                    } else if (lightPixels == 0) {
                        fullCharacters++;
                    }
                    lightPixelCounts[charIndex] = lightPixels;
                    indices[charIndex] = charIndex;
                }

                Array.Sort(lightPixelCounts, indices);

                if (emptyCharacters > 0) {
                    int popCount = emptyCharacters - 1;
                    Debug.Log("Omitting " + popCount + " repeated empty characters");
                    lightPixelCounts = SubArray(lightPixelCounts, 0, lightPixelCounts.Length - popCount);
                    indices = SubArray(indices, 0, indices.Length - popCount);
                }

                if (fullCharacters > 0) {
                    int unshiftCount = fullCharacters - 1;
                    Debug.Log("Omitting " + unshiftCount + " repeated full characters");
                    lightPixelCounts = SubArray(lightPixelCounts, unshiftCount, lightPixelCounts.Length - unshiftCount);
                    indices = SubArray(indices, unshiftCount, indices.Length - unshiftCount);
                }

                charCount = lightPixelCounts.Length;
                Debug.Log("charWidth: " + charWidth + ", charHeight: " + charHeight + ", charCount: " + charCount);

                Texture2D output = new Texture2D(charWidth * charCount, charHeight, TextureFormat.R8, false) {
                    filterMode = FilterMode.Point
                };

                for (int outputCharIndex = 0; outputCharIndex < charCount; outputCharIndex++) {

                    int inputCharIndex = indices[outputCharIndex];

                    getLowerLeftCorner(inputCharIndex, charWidth, charHeight, charColumns, charRows, out int inputLowerLeftCornerX, out int inputLowerLeftCornerY);
                    int outputLowerLeftCornerX = outputCharIndex * charWidth;

                    for (int x = 0; x < charWidth; x++) {
                        for (int y = 0; y < charHeight; y++) {
                            Color color = input.GetPixel(x + inputLowerLeftCornerX, y + inputLowerLeftCornerY);
                            color.r = 1 - color.r;
                            color.g = 1 - color.g;
                            color.b = 1 - color.b;
                            output.SetPixel(x + outputLowerLeftCornerX, y, color);
                        }
                    }
                }

                AssetDatabase.DeleteAsset("Assets/sortedFont.asset");
                AssetDatabase.CreateAsset(output, "Assets/sortedFont.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    private static void getLowerLeftCorner(int charIndex, int charWidth, int charHeight, int charColumns, int charRows, out int lowerLeftCornerX, out int lowerLeftCornerY) {
        lowerLeftCornerX = (charIndex % charColumns) * charWidth;
        lowerLeftCornerY = (charRows - 1 - (charIndex / charColumns)) * charHeight;
    }

    private int countLightPixels(int charIndex, int charWidth, int charHeight, int charColumns, int charRows) {

        getLowerLeftCorner(charIndex, charWidth, charHeight, charColumns, charRows, out int lowerLeftCornerX, out int lowerLeftCornerY);

        int lightPixels = 0;

        for (int x = 0; x < charWidth; x++) {
            for (int y = 0; y < charHeight; y++) {
                if (input.GetPixel(lowerLeftCornerX + x, lowerLeftCornerY + y).r > 0.5f){
                    lightPixels++;
                }
            }
        }

        return lightPixels;
    }
}
