using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    public void StartGame()
    {
        Manager.Scene.LoadScene("2.SelectScene");
    }

    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }
}
