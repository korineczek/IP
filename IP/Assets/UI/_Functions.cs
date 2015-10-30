using UnityEngine;
using System.Collections;

public class _Functions : Singleton<_Functions>
{
    public Color[,] GetPixels2D(Texture2D t)
    {
        Color[,] texture2d = new Color[t.width, t.height];
        Color[] texture1d = t.GetPixels();

        for (int h = 0; h < t.height; h++)
        {
            for (int w = 0; w < t.width; w++)
            {
                texture2d[w, h] = texture1d[h * t.width + w];
            }
        }
        return texture2d;
    }

    /// <summary>
    /// Use to set a 2D Color array as a texture.
    /// </summary>
    /// <param name="i">Input array</param>
    /// <param name="t">Texture we are changing</param>
    public void SetPixels2D(Color[,] i, Texture2D t)
    {
        Color[] texture1d = new Color[i.Length];

        int width = i.GetLength(0);
        int height = i.GetLength(1);

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                texture1d[h * width + w] = i[w, h];
            }
        }
        t.SetPixels(texture1d);
        t.Apply();
    }

    /// <summary>
    /// Descending sort
    /// </summary>
    /// <param name="input">input 1d array</param>
    /// <returns>sorted 1d array</returns>
    public float[] BubbleSort(float[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input.Length - 1 - i; j++)
            {
                float tmp;
                if (input[j] < input[j + 1])
                {
                    tmp = input[j];
                    input[j] = input[j + 1];
                    input[j + 1] = tmp;
                }
            }
        }
        return input;
    }
}
