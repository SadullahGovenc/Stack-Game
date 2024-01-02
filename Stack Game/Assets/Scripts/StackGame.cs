using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackGame : MonoBehaviour
{
    public Color32[] gameColors = new Color32[4];
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
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
        }
        stackIndex = transform.childCount - 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;
        if (Input.GetMouseButtonDown(0))
        {

        }
        MoveTile();

        // move The Stack

        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_Moving_Speed * Time.deltaTime);
    }

    void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];

        float f = Mathf.Sin(scoreCount * 0.25f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        }
        mesh.colors32 = colors;

    }  //COLORMESH

    Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
    {
        if (t < 0.33f)
        {
            return Color.Lerp(a, b, t / 0.33f);
        }
        else if (t < 0.66f)
        {
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        }
        else
        {
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
        }
    } // LERP4

    void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingX)
        {
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUND_Size, scoreCount, secondaryPosition);
        }
        else
        {
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUND_Size);
        }
    }

} // CLASS
