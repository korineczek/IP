using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PointProcessing : MonoBehaviour
{

    public Texture2D InputTex;

    private Texture2D tex;

	// Use this for initialization
	void Start ()
	{
        tex = new Texture2D(InputTex.width, InputTex.height);

	    Color[,] image = GetPixels2D(InputTex);
	    SetPixels2D(image, tex);

	    this.GetComponent<Renderer>().material.mainTexture = tex;
	}

    public Color[,] GetPixels2D(Texture2D t)
    {
        Color[,] texture2d = new Color[t.width,t.height];
        Color[] texture1d = t.GetPixels();

        for (int w = 0; w < t.width; w++)
        {
            for (int h = 0; h < t.height; h++)
            {
                texture2d[w, h] = texture1d[h*t.height + w];
            }
        }
        return texture2d;
    }

    public void SetPixels2D(Color[,] i, Texture2D t)
    {
        Color[] texture1d =new Color[i.Length];

        int width = i.GetLength(1);
        int height = i.GetLength(0);

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                texture1d[h*height + w] = i[w,h];
     
            }
        }
        t.SetPixels(texture1d);
        t.Apply();
    }
}
