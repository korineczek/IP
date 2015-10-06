using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PointProcessing : MonoBehaviour
{

    public Texture2D InputTex;
    private Texture2D tex;

	void Start ()
	{
        //Sort testing
	    int[] test = new[] {1, 1, 50, 1, 2};
	    test = BubbleSort(test);
	    for (int i = 0; i < test.Length; i++)
	    {
	        Debug.Log(test[i]);
	    }


        tex = new Texture2D(InputTex.width, InputTex.height);
	    Color[,] image = GetPixels2D(InputTex);

	    //image = Invert(image);
	    //image = Treshold(image, 0.5f);
	    //image = Rgb2Grayscale(image);
	    //image = Brightness(image, +50);
	    //image = Contrast(image, 2f);
	    //image = Gamma(image, 2.22f);
	    //image = Logarithmic(image);
	    //image = Exponential(image, 1.02f);

	    SetPixels2D(image, tex);

	    this.GetComponent<Renderer>().material.mainTexture = tex;
	}

    /// <summary>
    /// Get pixels from an image into a 2D Color array
    /// </summary>
    /// <param name="t">input texture</param>
    /// <returns>2D Color Array</returns>
    public Color[,] GetPixels2D(Texture2D t)
    {
        Color[,] texture2d = new Color[t.width, t.height];
        Color[] texture1d = t.GetPixels();

        for (int w = 0; w < t.width; w++)
        {
            for (int h = 0; h < t.height; h++)
            {
                texture2d[w, h] = texture1d[h * t.height + w];
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

        int width = i.GetLength(1);
        int height = i.GetLength(0);

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                texture1d[h * height + w] = i[w, h];
            }
        }
        t.SetPixels(texture1d);
        t.Apply();
    }

    /// <summary>
    /// Converts an RGB image into a Greyscale Image
    /// </summary>
    /// <param name="i">input image</param>
    /// <returns>converted greyscale image</returns>
    public Color[,] Rgb2Grayscale(Color[,] i)
    {
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                float grey = (i[w, h].r + i[w, h].g + i[w, h].b)/3;
                i[w, h].r = grey;
                i[w, h].g = grey;
                i[w, h].b = grey;
            }
        }
        return i;
    }

    /// <summary>
    /// Convert image to binary
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="t">treshold value</param>
    /// <returns>binary image</returns>
    public Color[,] Treshold(Color[,] i, float t)
    {
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                float bw = (i[w, h].r + i[w, h].g + i[w, h].b) / 3;
                bw = bw < t ? 0f : 1f;
                i[w, h].r = bw;
                i[w, h].g = bw;
                i[w, h].b = bw;
            }
        }
        return i;
    }

    /// <summary>
    /// Invert image
    /// </summary>
    /// <param name="i">input image</param>
    /// <returns>Inverted image</returns>
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

    /// <summary>
    /// Increase or decrease brightness
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="brightness">amount to increase/decrease brightness by</param>
    /// <returns>modified image</returns>
    public Color[,] Brightness (Color[,] i, int brightness)
    {
        float b = brightness/255.0f;

        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r += b;
                i[w, h].g += b;
                i[w, h].b += b;
            }
        }
        return i;
    }
    /// <summary>
    /// Simple contrast
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="a">contrast amount (1 = default contrast)</param>
    /// <returns>modified image</returns>
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

    /// <summary>
    /// Gamma Mapping
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="c">gamma value</param>
    /// <returns>modified image</returns>
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

    /// <summary>
    /// Logarithmic mapping
    /// </summary>
    /// <param name="i">input image</param>
    /// <returns></returns>
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
                texture1d[h * height + w] = (i[w, h].r + i[w, h].g + i[w, h].b)/3;
            }
        }

        float max = texture1d.Max()*255;
        float c = 255.0f/Mathf.Log(1 + max);
        
        //Map image
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r = (c*Mathf.Log(i[w, h].r*255 + 1))/255;
                i[w, h].g = (c*Mathf.Log(i[w, h].g*255 + 1))/255;
                i[w, h].b = (c*Mathf.Log(i[w, h].b*255 + 1))/255;
            }
        }
        return i;
    }

    /// <summary>
    /// Exponential mapping
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="k">curve shape coefficient, select just above 1.0f</param>
    /// <returns></returns>
    public Color[,] Exponential(Color[,] i, float k)
    {
        //Get max value
        int width = i.GetLength(1);
        int height = i.GetLength(0);

        float[] texture1d = new float[width * height];

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                texture1d[h * height + w] = (i[w, h].r + i[w, h].g + i[w, h].b) / 3;
            }
        }

        float max = texture1d.Max()*255;
        float c = 255.0f / (Mathf.Pow(k, max) - 1);

        //Map image
        for (int w = 0; w < i.GetLength(1); w++)
        {
            for (int h = 0; h < i.GetLength(0); h++)
            {
                i[w, h].r = (c * (Mathf.Pow(k, i[w, h].r * 255) - 1)) / 255;
                i[w, h].g = (c * (Mathf.Pow(k, i[w, h].g * 255) - 1)) / 255;
                i[w, h].b = (c * (Mathf.Pow(k, i[w, h].b * 255) - 1)) / 255;
            }
        }
        return i;
    }

    /// <summary>
    /// Descending sort
    /// </summary>
    /// <param name="input">input 1d array</param>
    /// <returns>sorted 1d array</returns>
    public int[] BubbleSort(int[] input)
    {
        for (int i = 0; i < input.Length; i++) {
            for (int j = 0; j < input.Length - 1 - i; j++)
            {
                int tmp;
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
