using System.Linq;
using UnityEngine;
using System.Collections;

public class _Filters : Singleton<_Filters>
{
    public int[,] kernel = new int[3, 3] {{1,1,1},
                                          {1,1,1},
                                          {1,1,1}};

    public int[,] sobelX = new int[3, 3] {{+1,+2,+1},
                                          {+0,+0,+0},
                                          {-1,-2,-1}};

    public int[,] sobelY = new int[3, 3] {{-1,+0,+1},
                                          {-2,+0,+2},
                                          {-1,+0,+1}};

    #region Lecture01 PointProcessing
    /// <summary>
    /// Converts an RGB image into a Greyscale Image
    /// </summary>
    /// <param name="i">input image</param>
    /// <returns>converted greyscale image</returns>
    public Color[,] Rgb2Grayscale(Color[,] i)
    {
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                float grey = (i[w, h].r + i[w, h].g + i[w, h].b) / 3;
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
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
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
    public Color[,] Brightness(Color[,] i, int brightness)
    {
        float b = brightness / 255.0f;

        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
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
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
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
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
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
        int width = i.GetLength(0);
        int height = i.GetLength(1);

        float[] texture1d = new float[width * height];

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                texture1d[h * height + w] = (i[w, h].r + i[w, h].g + i[w, h].b) / 3;
            }
        }

        float max = texture1d.Max() * 255;
        float c = 255.0f / Mathf.Log(1 + max);

        //Map image
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                i[w, h].r = (c * Mathf.Log(i[w, h].r * 255 + 1)) / 255;
                i[w, h].g = (c * Mathf.Log(i[w, h].g * 255 + 1)) / 255;
                i[w, h].b = (c * Mathf.Log(i[w, h].b * 255 + 1)) / 255;
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
        int width = i.GetLength(0);
        int height = i.GetLength(1);

        float[] texture1d = new float[width * height];

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                texture1d[h * height + w] = (i[w, h].r + i[w, h].g + i[w, h].b) / 3;
            }
        }

        float max = texture1d.Max() * 255;
        float c = 255.0f / (Mathf.Pow(k, max) - 1);

        //Map image
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                i[w, h].r = (c * (Mathf.Pow(k, i[w, h].r * 255) - 1)) / 255;
                i[w, h].g = (c * (Mathf.Pow(k, i[w, h].g * 255) - 1)) / 255;
                i[w, h].b = (c * (Mathf.Pow(k, i[w, h].b * 255) - 1)) / 255;
            }
        }
        return i;
    }
    #endregion

    #region Lecture02 NeighborhoodOperations
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
                        neighborhood[count] = (tmp[w + j, h + l].r + tmp[w + j, h + l].g + tmp[w + j, h + l].b) / 3;
                        count++;
                    }
                }
                //sort neighborhood
                float result = 0f;
                switch (type)
                {
                    //median
                    case 1:
                        neighborhood = _Functions.Instance.BubbleSort(neighborhood);
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
                        result = neighborhood.Max() - neighborhood.Min();
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
        tmp = Rgb2Grayscale(i);

        Color[,] result = new Color[i.GetLength(0), i.GetLength(1)];

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
                        sumH += tmp[w + j, h + l].r * kx[j + kw, l + kh];
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

        int kw = k / 2;
        int kh = k / 2;

        //Generate weighted Gaussian kernel
        float[,] kernel = new float[k, k];
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
                kernel[j, l] = kernel[j, l] / kernelSum;
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
        float e = Mathf.Exp(-(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f)) / (2 * Mathf.Pow(sig, 2)));
        float result = e / (Mathf.Sqrt(2 * Mathf.PI * Mathf.Pow(sig, 2)));
        return result;
    }
    #endregion

    #region Lecture03
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
      // tex.SetPixels(image1d);
      //  tex.Apply();
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

        //tex.SetPixels(output);
        //tex.Apply();
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
        i = Treshold(i, 0.5f);

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
        i = Treshold(i, 0.5f);

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
    #endregion

    public Color[,] NormalizedRgb(Color[,] i)
    {
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                Color pix = i[w, h];
                float sum = pix.r + pix.g + pix.b;
                if (sum == 0f)
                {
                    i[w,h] = Color.black;
                }
                else
                {
                    i[w, h] = new Color(i[w, h].r / sum, i[w, h].g / sum, i[w, h].b / sum);
                }
            }
        }
        return i;
    }

    public Color[,] DetectColor(Color[,] i, Color targetColor, int spread)
    {
        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                Color pix = i[w, h];
                if(Mathf.Abs(pix.r - targetColor.r)*255f < spread && Mathf.Abs(pix.g - targetColor.g)*255f < spread && Mathf.Abs(pix.b - targetColor.b)*255 < spread)
                {
                    i[w,h] = Color.white;
                }
                else
                {
                    i[w, h] = Color.black;  
                }
            }
        }
        return i;
    }

}

