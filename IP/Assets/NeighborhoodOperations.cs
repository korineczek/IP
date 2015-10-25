using System.CodeDom.Compiler;
using System.Linq;
using UnityEngine;
using System.Collections;

public class NeighborhoodOperations : Singleton<NeighborhoodOperations>
{

    public Texture2D InputTex;
    private Texture2D tex;

    private int[,] kernel = new int[3, 3] {{1,1,1},
                                          {1,1,1},
                                          {1,1,1}};

    private int[,] sobelX = new int[3, 3] {{+1,+2,+1},
                                          {+0,+0,+0},
                                          {-1,-2,-1}};

    private int[,] sobelY = new int[3, 3] {{-1,+0,+1},
                                          {-2,+0,+2},
                                          {-1,+0,+1}};


    // Use this for initialization
    void Start()
    {
        /*
        tex = new Texture2D(InputTex.width, InputTex.height);
        Color[,] image = PointProcessing.Instance.GetPixels2D(InputTex);

        //image = Rank(image, 9, 1);
        //image = Edge(image, sobelX, sobelY, 0.25f);
        image = GaussianBlur(image, 2f, 7);
        //image = PointProcessing.Instance.Rgb2Grayscale(image);

        PointProcessing.Instance.SetPixels2D(image, tex);
        this.GetComponent<Renderer>().material.mainTexture = tex;
         */
    }

    /// <summary>
    /// Median filtering
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="k">kernel size</param>
    /// /// <param name="type">type of filter 1-median, 2-min, 3-max, 4-diff</param>
    /// <returns></returns>
    public Color[,] Rank(Color[,] i, int k, int type)
    {
        Color[,] tmp = i;

        int kw = k / 2;
        int kh = k / 2;

        for (int w = 0 + kw; w < i.GetLength(0) - kw; w++)
        {
            for (int h = 0 + kh; h < i.GetLength(1) - kh; h++)
            {
                int count = 0;
                // do median
                float[] neighborhood = new float[k * k];
                // get neighborhood
                for (int j = -kw; j <= +kw; j++)
                {
                    for (int l = -kh; l <= +kh; l++)
                    {
                        neighborhood[count] = (tmp[w + j, h + l].r + tmp[w + j, h + l].g + tmp[w + j, h + l].b)/3;
                        count ++;
                    }
                }
                //sort neighborhood
                float result = 0f;
                switch (type)
                {
                    //median
                    case 1:
                        neighborhood = PointProcessing.Instance.BubbleSort(neighborhood);
                        result = neighborhood[(int)neighborhood.Length / 2];
                        break;

                    //max
                    case 2:
                        result = neighborhood.Max();
                        break;

                    //min
                    case 3:
                        result = neighborhood.Min();
                        break;

                    //difference
                    case 4:
                        result = neighborhood.Max()-neighborhood.Min();
                        break;

                }
                //set pixels
                i[w, h].r = result;
                i[w, h].g = result;
                i[w, h].b = result;
            }
        }
        return i;
    }

    /// <summary>
    /// Edge detection
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="kx">horizontal kernel</param>
    /// <param name="ky">vertical kernel</param>
    /// <param name="t">treshold</param>
    /// <returns></returns>
    public Color[,] Edge(Color[,] i, int[,] kx, int[,] ky, float t)
    {
        Color[,] tmp = i;
        //set source image to grayscale
        tmp = PointProcessing.Instance.Rgb2Grayscale(i);

        Color[,] result = new Color[i.GetLength(0),i.GetLength(1)]; 

        int kw = kx.GetLength(1) / 2;
        int kh = ky.GetLength(0) / 2;


        for (int w = 0 + kw; w < i.GetLength(0) - kw; w++)
        {
            for (int h = 0 + kh; h < i.GetLength(1) - kh; h++)
            {
                //perform horizontal correlation
                float sumH = 0f;
                for (int j = -kw; j <= +kw; j++)
                {
                    for (int l = -kh; l <= +kh; l++)
                    {
                        sumH += tmp[w + j, h + l].r*kx[j+kw,l+kh];
                    }
                }
                sumH = (Mathf.Abs(sumH) < t ? 0f : 1f);

                //perform vertical correlation
                float sumV = 0f;
                for (int j = -kw; j <= +kw; j++)
                {
                    for (int l = -kh; l <= +kh; l++)
                    {
                        sumV += tmp[w + j, h + l].r * ky[j + kw, l + kh];
                    }
                }
                sumV = (Mathf.Abs(sumV) < t ? 0f : 1f);
                
                result[w, h].r = sumH + sumV;
                result[w, h].g = sumH + sumV;
                result[w, h].b = sumH + sumV;
            }
        }
        return result;
    }


    /// <summary>
    /// Blur filter that follows the values of a gaussian function.
    /// </summary>
    /// <param name="i">input image</param>
    /// <param name="sig">sigma (width)</param>
    /// <param name="k">kernel size</param>
    /// <returns>modified image</returns>
    public Color[,] GaussianBlur(Color[,] i, float sig, int k)
    {
        Color[,] tmp = i;

        int kw = k/ 2;
        int kh = k/ 2;

        //Generate weighted Gaussian kernel
        float [,] kernel = new float[k,k];
        float kernelSum = 0f;
        for (int j = 0; j < k; j++)
        {
            for (int l = 0; l < k; l++)
            {
                float gVal = Gaussian2D(sig, j - kw, l - kh);
                kernel[j, l] = gVal;
                kernelSum += gVal;
            }
        }
        //weight the kernel so that mass is 1
        for (int j = 0; j < k; j++)
        {
            for (int l = 0; l < k; l++)
            {
                kernel[j, l] = kernel[j, l]/kernelSum;
            }
        }

        //Apply kernel to image
        for (int w = 0 + kw; w < i.GetLength(0) - kw; w++)
        {
            for (int h = 0 + kh; h < i.GetLength(1) - kh; h++)
            {
                //Blurring
                float sumR = 0f;
                float sumG = 0f;
                float sumB = 0f;
                for (int j = -kw; j <= +kw; j++)
                {
                    for (int l = -kh; l <= +kh; l++)
                    {
                        sumR += tmp[w + j, h + l].r * kernel[j + kw, l + kh];
                        sumG += tmp[w + j, h + l].g * kernel[j + kw, l + kh];
                        sumB += tmp[w + j, h + l].b * kernel[j + kw, l + kh];
                    }
                }

                i[w, h].r = sumR;
                i[w, h].g = sumG;
                i[w, h].b = sumB;
            }
        }
        return i;
    }

    /// <summary>
    /// Return a value from 2d gaussian distribution.
    /// </summary>
    /// <param name="sig">sigma(width)</param>
    /// <param name="x">x position</param>
    /// <param name="y">y position</param>
    /// <returns>result following the gaussian distribution</returns>
    public float Gaussian2D(float sig, float x, float y)
    {
        float e = Mathf.Exp(-(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f))/(2*Mathf.Pow(sig,2)));
        float result = e/(Mathf.Sqrt(2*Mathf.PI*Mathf.Pow(sig, 2)));
        return result;
    }


}
