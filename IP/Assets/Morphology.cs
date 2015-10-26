using UnityEngine;
using System.Collections;

public class Morphology : MonoBehaviour {

    public Texture2D InputTex;
    private Texture2D tex;

	void Start () {

        tex = new Texture2D(InputTex.width, InputTex.height);
        Color[,] image = PointProcessing.Instance.GetPixels2D(InputTex);

        //filters here
	    //image = HistogramEqualization(image);
	    //image = Dilation(image, 3);
	    //image = Erosion(image, 3);
	    image = Dilation(Erosion(image, 3), 3);


        PointProcessing.Instance.SetPixels2D(image, tex);

        //ImageMirror(InputTex);

        this.GetComponent<Renderer>().material.mainTexture = tex;

	}

    public Color[,] HistogramEqualization(Color[,] i)
    {
        //Histograms
        int[] histR = new int[256];
        int[] histG = new int[256];
        int[] histB = new int[256];

        //Cumulative Histograms
        float[] cHistR = new float[256];
        float[] cHistG = new float[256];
        float[] cHistB = new float[256];

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

        for (int j = 0; j < 256; j++)
        {
            sumR += (float)histR[j] / (i.GetLength(0) * i.GetLength(1));
            sumG += (float)histG[j] / (i.GetLength(0) * i.GetLength(1));
            sumB += (float)histB[j] / (i.GetLength(0) * i.GetLength(1));

            cHistR[j] = sumR;
            cHistG[j] = sumG;
            cHistB[j] = sumB;
        }

        for (int w = 0; w < i.GetLength(0); w++)
        {
            for (int h = 0; h < i.GetLength(1); h++)
            {
                i[w, h].r = cHistR[(int)(i[w, h].r * 255f)];
                i[w, h].g = cHistG[(int)(i[w, h].g * 255f)];
                i[w, h].b = cHistB[(int)(i[w, h].b * 255f)];
            }
        }

        return i;
    }

    public void ImageMirror(Texture2D i)
    {
        Color[] image1d;

        int w = i.width;
        int h = i.height;

        image1d = i.GetPixels();

        Color[] output = new Color[w*h];

        for (int j = 0; j < w*h; j++)
        {
            //do something
            output[j] = image1d[(j/w)*w + ((w - 1) - j%w)];
        }
        tex.SetPixels(output);
        tex.Apply();
    }

    public void ImageWrap1D(Texture2D i, int thickness)
    {
        Color[] image1d = i.GetPixels();

        int w = i.width;
        int h = i.height;

        for (int j = 0; j < w * h; j++)
        {
            if ((j + 1) % w <= thickness || (j + 1) % w == 0 || (j + 1) % w > (w - thickness)
                || (j + 1) <= thickness * w || (j + 1) >= (image1d.Length - (w * thickness)))
            {
                //Do something with selected pixels
                image1d[j] = new Color(0, 0, 0);
            }
        }
        tex.SetPixels(image1d);
        tex.Apply();
    }

    public Color[,] Dilation(Color[,] i, int k)
    {
        Color[,] result = new Color[i.GetLength(0),i.GetLength(1)];

        for (int w = 0 + k; w < i.GetLength(0) - k; w++)
        {
            for (int h = 0 + k; h < i.GetLength(1) - k; h++)
            {
                float sum = 0f;
                // correlation here
                for (int j = -k/2; j <= +k/2; j++)
                {
                    for (int l = -k/2; l <= +k/2; l++)
                    {
                        //and here
                        sum += i[w + j, h + l].r;
                    }
                }
                //

                result[w, h].r = sum > 0f ? 1f : 0f;
                result[w, h].g = sum > 0f ? 1f : 0f;
                result[w, h].b = sum > 0f ? 1f : 0f;
            }
        }
        return result;
    }

    public Color[,] Erosion(Color[,] i, int k)
    {
        Color[,] result = new Color[i.GetLength(0), i.GetLength(1)];

        for (int w = 0 + k; w < i.GetLength(0) - k; w++)
        {
            for (int h = 0 + k; h < i.GetLength(1) - k; h++)
            {
                float sum = 0f;
                int kernelsum = 0;
                // correlation here
                for (int j = -k / 2; j <= +k / 2; j++)
                {
                    for (int l = -k / 2; l <= +k / 2; l++)
                    {
                        //and here
                        sum += i[w + j, h + l].r;
                        kernelsum++;
                    }
                }
                //
                result[w, h].r = sum >= k*k ? 1f : 0;
                result[w, h].g = sum >= k*k ? 1f : 0;
                result[w, h].b = sum >= k*k ? 1f : 0;
            }
        }
        return result;
    }

}
