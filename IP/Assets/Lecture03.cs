using UnityEngine;
using System.Collections;

public class Lecture03 : MonoBehaviour {

    public Texture2D InputTex;
    private Texture2D tex;

    void Start()
    {
        tex = new Texture2D(InputTex.width, InputTex.height);
        Color[,] image = PointProcessing.Instance.GetPixels2D(InputTex);
        //
        //image = HistogramEqualization(image);
        //image = Erosion(image, 3);
        //image = Dilation(image, 3);
        image = BlobExtraction(image);
        //
        PointProcessing.Instance.SetPixels2D(image, tex);

        //ImageWrap1D(InputTex,5);
        //ImageMirror1D(InputTex);
        this.GetComponent<Renderer>().material.mainTexture = tex;
    }

    public Color[,] HistogramEqualization(Color[,] i)
    {
        //regular histograms
        float[] histR = new float[256];
        float[] histG = new float[256];
        float[] histB = new float[256];

        //cumulative histogram
        float[] cHistR = new float[256];
        float[] cHistG = new float[256];
        float[] cHistB = new float[256];

        //run through image and generate histograms
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                histR[(int)(i[w, h].r * 255f)]++;
                histG[(int)(i[w, h].g * 255f)]++;
                histB[(int)(i[w, h].b * 255f)]++;
            }
        }

        float sumR = 0f;
        float sumG = 0f;
        float sumB = 0f;

        //run through histograms and generate cumulative histograms
        for (int j = 0; j < 256; j++)
        {
            sumR += (histR[j]) / (i.GetLength(0) * i.GetLength(1));
            sumG += (histG[j]) / (i.GetLength(0) * i.GetLength(1));
            sumB += (histB[j]) / (i.GetLength(0) * i.GetLength(1));

            cHistR[j] = sumR;
            cHistG[j] = sumG;
            cHistB[j] = sumB;
        }

        //run through image again and remap values
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                i[w, h].r = cHistR[(int) (i[w, h].r * 255f)];
                i[w, h].g = cHistG[(int) (i[w, h].g * 255f)];
                i[w, h].b = cHistB[(int) (i[w, h].b * 255f)];
            }
        }

        return i;
    }

    /// <summary>
    /// Wrapping 1Dimage in one loop
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="thickness">thickness of the wrap</param>
    public void ImageWrap1D(Texture2D i, int thickness)
    {
        Color[] image1d = i.GetPixels();

        int w = i.width;
        int h = i.height;

        for (int j = 0; j < w*h; j++)
        {
            if ((j + 1) % w <= thickness || (j + 1) % w == 0 || (j + 1) % w > (w-thickness) 
                || (j + 1) <= thickness*w || (j + 1) >= (image1d.Length - (w*thickness)))
            {
                //Do something with selected pixels
                image1d[j] = new Color(0,0,0);
            }
        }
        tex.SetPixels(image1d);
        tex.Apply();
    }

    /// <summary>
    /// Flip image in one loop
    /// </summary>
    /// <param name="i"></param>
    public void ImageMirror1D(Texture2D i)
    {
        Color[] image1d = i.GetPixels();
        
        int w = i.width;
        int h = i.height;

        Color[] output = new Color[w*h];

        for (int j = 0; j < w * h; j++)
        {
            //this is where the magic happens
            output[j] = image1d[(j/w) * w + ((w - 1) - j%w)];
        }

        tex.SetPixels(output);
        tex.Apply();
        // DO VERTICAL FLIP PLEASE
    }

    /// <summary>
    /// Erode an image using a k by k box kernel
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="k">kernel thickness</param>
    /// <returns>modified image</returns>
    public Color[,] Erosion(Color[,] i, int k)
    {
        //set source image to grayscale
        i = PointProcessing.Instance.Treshold(i, 0.5f);

        Color[,] result = new Color[i.GetLength(0), i.GetLength(1)];

        for (int w = 0 + k; w < i.GetLength(0) - k; w++)
        {
            for (int h = 0 + k; h < i.GetLength(1) - k; h++)
            {
                float sum = 0f;
                for (int j = -k / 2; j <= +k / 2; j++)
                {
                    for (int l = -k / 2; l <= +k / 2; l++)
                    {
                        sum += i[w + j, h + l].r;
                    }
                }

                float res = sum < k*k ? 0f : 1f;
                result[w, h].r = res;
                result[w, h].g = res;
                result[w, h].b = res;
            }
        }
        return result;
    }

    /// <summary>
    /// Dilate image by a k by k kernel
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="k">kernel size</param>
    /// <returns>modified image</returns>
    public Color[,] Dilation(Color[,] i, int k)
    {
        //set source image to grayscale
        i = PointProcessing.Instance.Treshold(i, 0.5f);

        Color[,] result = new Color[i.GetLength(0), i.GetLength(1)];

        for (int w = 0 + k; w < i.GetLength(0) - k; w++)
        {
            for (int h = 0 + k; h < i.GetLength(1) - k; h++)
            {
                float sum = 0f;
                for (int j = -k / 2; j <= +k / 2; j++)
                {
                    for (int l = -k / 2; l <= +k / 2; l++)
                    {
                        sum += i[w + j, h + l].r;
                    }
                }

                float res = sum > 0 ? 1f : 0f;
                result[w, h].r = res;
                result[w, h].g = res;
                result[w, h].b = res;
            }
        }
        return result;
    }


    /// <summary>
    /// Extract blobs and mark them with different colors
    /// </summary>
    /// <param name="i">input image</param>
    /// <returns></returns>
    public Color[,] BlobExtraction(Color[,] i)
    {
        int label = 0;

        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                if (i[w, h].r == 1f)
                {
                    label++;
                    Grassfire(i,w,h,label*35); //change number in case there is a lot of blobs
                }
            }   
        }
        return i;
    }

    /// <summary>
    /// Grassfire at position x, y
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="x">x coord</param>
    /// <param name="y">y coord</param>
    /// <param name="label">index of current blob</param>
    public void Grassfire(Color[,] i, int x, int y, int label)
    {
        int width = i.GetLength(0);
        int height = i.GetLength(1);

        i[x, y] = new Color(label/255f,0,0);

        if (x + 1 < width && i[x + 1, y].r == 1f)
        {
            Grassfire(i, x+1, y, label);
        }
        if (x -1 > 0 && i[x - 1, y].r == 1f)
        {
            Grassfire(i, x - 1, y, label);
        }
        if (y + 1 < height && i[x, y + 1].r == 1f)
        {
            Grassfire(i, x, y + 1, label);
        }
        if (y - 1 > 0 && i[x, y - 1].r == 1f)
        {
            Grassfire(i, x, y - 1, label);
        }
    }
}
