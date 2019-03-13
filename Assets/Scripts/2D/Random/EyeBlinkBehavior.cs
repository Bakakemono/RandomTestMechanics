////////////////// HOW USE IT //////////////////
// Setup this script in a game object
// Give it two child with a sprite for each
// Give the tag "Globe" to the bigger one
// Give the tag "Iris" to the smaller one
////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EyeBlinkBehavior : MonoBehaviour
{
    enum BlinkingState
    {
        IS_OPEN,
        IS_CLOSE,
        IS_RECOVERY
    }

    private BlinkingState blinkingState = BlinkingState.IS_OPEN;

    [Header("Transform")]
    [SerializeField] private Transform globeTransform;
    private Vector2 globeStartingScale;
    private Vector2 globeCurrentScale;

    [SerializeField] private Transform irisTransform;
    private Vector2 irisStartingScale;
    private float irisWidthClose;

    [Header("State")]
    [Range(0, 100)]
    [SerializeField] private int closingPercentage = 20;

    [Range(0, 100)]
    [SerializeField] private int extensionPercentage = 20;

    [Range(0, 90)]
    [SerializeField] private int randomePercentage = 20;

    [SerializeField] private float blinkingTimeInSecond = 0.5f;
    [SerializeField] private float waitingTimeInSecond = 0.3f;

    private int coolDown = 0;
    private float recoveryEndTime;

    private bool isOpening = false;
    private const int GAME_FRAME_RATE = 50;

    void Start()
    {
        blinkingTimeInSecond = Random.Range(blinkingTimeInSecond - blinkingTimeInSecond * randomePercentage / 100,
                                            blinkingTimeInSecond + blinkingTimeInSecond * randomePercentage / 100);

        waitingTimeInSecond = Random.Range( waitingTimeInSecond - waitingTimeInSecond * randomePercentage / 100,
                                            waitingTimeInSecond + waitingTimeInSecond * randomePercentage / 100);

        globeStartingScale = globeTransform.localScale;
        irisStartingScale = irisTransform.localScale;
    }

    void FixedUpdate()
    {
        Blink();
    }

    private void Blink()
    {
        switch (blinkingState)
        {
            case BlinkingState.IS_OPEN:
                globeTransform.localScale = new Vector2(    globeStartingScale.x + (((globeStartingScale.x * extensionPercentage / 100) / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown),
                                                            globeStartingScale.y - ((globeStartingScale.y * closingPercentage / 100) / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown);

                irisTransform.localScale = new Vector2(     irisStartingScale.x + (((irisStartingScale.x * extensionPercentage / 100) / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown),
                                                            irisStartingScale.y - (irisStartingScale.y / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown);

                coolDown++;

                if (coolDown > blinkingTimeInSecond * GAME_FRAME_RATE)
                {
                    coolDown = 0;
                    globeCurrentScale = globeTransform.localScale;
                    irisWidthClose = irisTransform.localScale.x;
                    blinkingState = BlinkingState.IS_CLOSE;
                }
                break;

            case BlinkingState.IS_CLOSE:
                globeTransform.localScale = new Vector2(    globeCurrentScale.x - (((globeStartingScale.x * extensionPercentage / 100) / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown),
                                                            globeCurrentScale.y + ((globeStartingScale.y * closingPercentage / 100) / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown);

                irisTransform.localScale = new Vector2(     irisWidthClose - (((irisStartingScale.x * extensionPercentage / 100) / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown),
                                                            0 + (irisStartingScale.y / (blinkingTimeInSecond * GAME_FRAME_RATE)) * coolDown);

                coolDown++;

                if (coolDown > blinkingTimeInSecond * GAME_FRAME_RATE)
                {
                    coolDown = 0;
                    globeCurrentScale = globeTransform.localScale;
                    blinkingState = BlinkingState.IS_RECOVERY;
                    recoveryEndTime = Time.time + waitingTimeInSecond;
                }

                break;

            case BlinkingState.IS_RECOVERY:
                if (recoveryEndTime <= Time.time)
                {
                    blinkingState = BlinkingState.IS_OPEN;
                }
                break;
        }
    }
}

[CustomEditor(typeof(BSP))]
public class BSP_CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BSP myTarget = (BSP)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            myTarget.ResetBSP();
        }
        if (GUILayout.Button("Start"))
        {
            myTarget.DoBSP();
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();
    }
}
