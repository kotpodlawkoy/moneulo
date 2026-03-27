using UnityEngine;
//using System.Collections.Generic;

public class PlayerVisibilityController : BaseVisionController
{
    public bool DebugShowVisibleTargets = true;

    void OnGUI ()
    {
        //Debug.Log("OnGUI is running");

        if ( !DebugShowVisibleTargets ) return;

        GUIStyle labelStyle = new GUIStyle ( GUI.skin.label );
        labelStyle.fontSize = 40;
        labelStyle.normal.textColor = Color.white;

        GUI.Label ( new Rect ( 10, 10, 1000, 200 ), $"Visible targets: {visibleTargets.Count}", labelStyle );

        int y = 90;
        foreach ( GameObject target in visibleTargets )
        {
            GUI.Label ( new Rect ( 10, y, 500, 90 ), $" - {target.name}", labelStyle );
            y += 90;
        }
    }
}
