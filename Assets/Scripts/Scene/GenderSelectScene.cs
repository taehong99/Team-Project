using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderSelectScene : BaseScene
{
    public void ChooseMale()
    {
        Manager.Scene.LoadScene("Stage1-1");
    }

    public void ChooseFemale()
    {

    }

    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }
}
