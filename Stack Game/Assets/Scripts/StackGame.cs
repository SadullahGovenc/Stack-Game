using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackGame : MonoBehaviour
{
    public Color32[] gamColors = new Color32[4];
    public Material stackMat;

    private const float BOUND_Size = 3.5f;
    private const float STACK_Moving_Speed = 5f;
    private const float ERROR_Margin = 0.1f;
    private const float STACK_Bounds_Gain = 0.25f;
    private const float COMBO_Start_Gain = 3f;



    private GameObject[] theStack;

    private Vector2 stackBounds = new Vector2(BOUND_Size, BOUND_Size);

    private int stackIndex;
    private int scoreCount = 0;
    private int combo = 0;

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private bool isMovingX = true;
    private bool gameOver = false;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition2;


    void Start()
    {
        theStack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
