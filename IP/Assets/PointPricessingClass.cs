using System.Linq;
using UnityEngine;
using System.Collections;

public class PointPricessingClass : MonoBehaviour
{

    public Texture2D InputImage;
    private Texture2D tex;

    void Start()
    {

        tex = new Texture2D(InputImage.width, InputImage.height);
        Color[,] image = GetPixels2D(InputImage);
        //
        //image = Invert(image);
        //image = Rgb2greyscale(image);
        //image = Treshold(Rgb2greyscale(image), 0.5f);
        //image = Treshold(image, 0.5f);
        //image = Brightness(image, -100);
        //image = Contrast(image, 2f);
        //image = Gamma(image, 2.22f);
        //image = Logarithmic(image);
        //
        SetPixels2D(image, tex);

        this.GetComponent<Renderer>().material.mainTexture = tex;
    }

    public Color[,] Invert(Color[,] i)
    {
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r = 1 - i[w, h].r;
                i[w, h].g = 1 - i[w, h].g;
                i[w, h].b = 1 - i[w, h].b;
            }
        }
        return i;
    }

    public Color[,] Rgb2greyscale(Color[,] i)
    {
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                float grey = (i[w, h].r + i[w, h].g + i[w, h].b) / 3;

                i[w, h].r = grey;
                i[w, h].g = grey;
                i[w, h].b = grey;
            }
        }
        return i;
    }

    public Color[,] Treshold(Color[,] i, float t)
    {
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r = i[w, h].r < t ? 0f : 1f;
                i[w, h].g = i[w, h].g < t ? 0f : 1f;
                i[w, h].b = i[w, h].b < t ? 0f : 1f;
            }
        }
        return i;
    }

    public Color[,] Brightness(Color[,] i, int b)
    {
        float c = b / 255f;

        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r += c;
                i[w, h].g += c;
                i[w, h].b += c;
            }
        }
        return i;
    }

    public Color[,] Contrast(Color[,] i, float a)
    {
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r *= a;
                i[w, h].g *= a;
                i[w, h].b *= a;
            }
        }
        return i;
    }

    public Color[,] Gamma(Color[,] i, float c)
    {
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r = Mathf.Pow(i[w, h].r, c);
                i[w, h].g = Mathf.Pow(i[w, h].g, c); 
                i[w, h].b = Mathf.Pow(i[w, h].b, c); 
            }
        }
        return i;
    }

    public Color[,] Logarithmic(Color[,] i)
    {
        //Get max value
        int width = i.GetLength(1);
        int height = i.GetLength(0);

        float[] texture1d = new float[width*height];

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                texture1d[w*width + h] = (i[w, h].r + i[w, h].g + i[w, h].b)/3;
            }
        }

        float max = texture1d.Max()*255f;
        float c = 255/(Mathf.Log(1 + max));

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                i[w, h].r = (c*Mathf.Log(1 + i[w, h].r*255))/255;
                i[w, h].g = (c*Mathf.Log(1 + i[w, h].g*255))/255;
                i[w, h].b = (c*Mathf.Log(1 + i[w, h].b*255))/255;
            }
        }
        return i;
    }









    //---------------------
    public Color[,] GetPixels2D(Texture2D i)
    {

        Color[,] texture2d = new Color[i.width, i.height];
        Color[] texture1d = i.GetPixels();

        for (int w = 0; w < i.width; w++)
        {
            for (int h = 0; h < i.height; h++)
            {
                texture2d[w, h] = texture1d[w * i.width + h];
            }
        }
        return texture2d;
    }

    public void SetPixels2D(Color[,] i, Texture2D t)
    {
        Color[] texture1d = new Color[i.Length];

        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                texture1d[w * i.GetLength(1) + h] = i[w, h];
            }
        }
        t.SetPixels(texture1d);
        t.Apply();
    }
}
