using UnityEngine;
using System.Collections;

public class FilterButton : MonoBehaviour
{

    public int Index = 0;

    public void CallFilter()
    {
       _Manager.Instance.ExecuteFilter(Index);
    }

}
