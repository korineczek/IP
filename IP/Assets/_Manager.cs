using System;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using System.Collections;

public class _Manager : Singleton<_Manager>
{
    public Transform Target;
    public Texture2D InputTex;
    private Texture2D tex;

    [NonSerialized]
    public Color[,] Image;

    private _Filters filters;
    private _Functions functions;

    // get filter names here (MUST BE SAME ORDER AS SWITCH BELOW
    [NonSerialized]
    public List<String> FilterNames = new List<string>
    {
        "threshold",
        "rgb2grayscale",
        "invert",
        "simple brightness",
        "simple contrast",
        "gamma",
        "logarithmic",
        "exponential",
        "median 3x3",
        "median 5x5",
        "edge sobel",
        "gaussian blur",
        "histogram equalization",
        "erosion",
        "dilation",

    };

    public void ExecuteFilter(int filterId)
    {
        switch (filterId)
        {
            case 0:
                Image = filters.Treshold(Image, 0.5f);
                break;
            case 1:
                Image = filters.Rgb2Grayscale(Image);
                break;
            case 2:
                Image = filters.Invert(Image);
                break;
            case 3:
                Image = filters.Brightness(Image, 10);
                break;
            case 4:
                Image = filters.Contrast(Image, 2f);
                break;
            case 5:
                Image = filters.Gamma(Image, 2.22f);
                break;
            case 6:
                Image = filters.Logarithmic(Image);
                break;
            case 7:
                Image = filters.Exponential(Image, 1.1f);
                break;
            case 8:
                Image = filters.Rank(Image,3, 1);
                break;
            case 9:
                Image = filters.Rank(Image, 5, 1);
                break;
            case 10:
                Image = filters.Edge(Image, filters.sobelX, filters.sobelY, 0.5f);
                break;
            case 11:
                Image = filters.GaussianBlur(Image, 5, 5);
                break;
            case 12:
                Image = filters.HistogramEqualization(Image);
                break;
            case 13:
                Image = filters.Erosion(Image, 3);
                break;
            case 14:
                Image = filters.Dilation(Image, 3);
                break;

        }

        functions.SetPixels2D(Image, tex);
        Target.GetComponent<Renderer>().material.mainTexture = tex;
    }

    public void ResetTexture()
    {
        Image = functions.GetPixels2D(InputTex);
        functions.SetPixels2D(Image, tex);
        Target.GetComponent<Renderer>().material.mainTexture = tex;

    }


    void Awake()
    {
        //get references on startup
        filters = _Filters.Instance;
        functions = _Functions.Instance;

    }

	void Start () {
	    //initialization setup
        tex = new Texture2D(InputTex.width, InputTex.height);
        Image = functions.GetPixels2D(InputTex);
        functions.SetPixels2D(Image, tex);
        //display input on target
        Target.GetComponent<Renderer>().material.mainTexture = tex;

	}

}
