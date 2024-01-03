using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Vector3 lastTilePosition;


    void Start()
    {
        
            theStack = new GameObject[transform.childCount];
            stackMat = new Material(Shader.Find("Standard")); // Malzeme atamasý yapýlýyor.

            for (int i = 0; i < transform.childCount; i++)
            {
                theStack[i] = transform.GetChild(i).gameObject;
                ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
            }
            stackIndex = transform.childCount - 1;
        


        ///sorun için çözüm deniyorum
        theStack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
        }
        stackIndex = transform.childCount - 1;

        if (stackMat == null)
        {
            Debug.Log("bossss");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceTile())
            {

                SpawnTile();
                scoreCount++;
            }
            else
            {
                // end of game
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
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
    } // MOVE TÝLE

    bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;

        if (isMovingX)
        {
            float deltaX = lastTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_Margin)
            {
                combo = 0;

                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                {
                    return false;
                }

                float middle = lastTilePosition.x + t.localPosition.x / 2f;
                t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);

                // part fall down
                CreateRubble(
                    new Vector3((t.position.x > 0) ? t.position.x + (t.localScale.x / 2f) : t.position.x - (t.localScale.x / 2f), t.position.y, t.position.z),
                                    new Vector3(Mathf.Abs(deltaX), 1f, t.localScale.z));

                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2f), scoreCount, lastTilePosition.z);

            }
            else
            {
                if (combo > COMBO_Start_Gain)
                {
                    stackBounds.x += STACK_Bounds_Gain;
                    if (stackBounds.x > BOUND_Size)
                        stackBounds.x = BOUND_Size;
                    float middle = lastTilePosition.x + t.localPosition.x / 2f;

                    t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);
                    t.localPosition = new Vector3(middle - (lastTilePosition.x / 2f), scoreCount, lastTilePosition.z);

                }

                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);

            }

            
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.x;
            if (Mathf.Abs(deltaZ) > ERROR_Margin)
            {
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTilePosition.z + t.localPosition.z / 2f;
                t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);

                CreateRubble(
                new Vector3(t.position.x, t.position.y, (t.position.z > 0) ? t.position.z + (t.localScale.z / 2f) : t.position.z - (t.localScale.z / 2f)),
                new Vector3(t.localScale.x, 1f, Mathf.Abs(deltaZ)));

                t.localPosition = new Vector3(lastTilePosition.x / 2f, scoreCount, middle - (lastTilePosition.z / 2f));



            }
            else
            {
                if (combo > COMBO_Start_Gain)
                {
                    stackBounds.y += COMBO_Start_Gain;
                    if (stackBounds.y > BOUND_Size)
                    {
                        stackBounds.y = BOUND_Size;
                    }
                    float middle = lastTilePosition.z + t.localPosition.z / 2f;
                    t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x / 2f, scoreCount, middle - (lastTilePosition.z / 2f));
                }

                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);

            }



        } //  ELSE if moving on Axis


        secondaryPosition = (isMovingX ? lastTilePosition.x : lastTilePosition.z);
        isMovingX = !isMovingX;


        return true;
    } // place tile

    void CreateRubble(Vector3 pos, Vector3 scale)
    { /*
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.AddComponent<MeshRenderer>().material = stackMat;
        ColorMesh(go.GetComponent<MeshFilter>().mesh);
        */

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        // MeshRenderer'ý kontrol et ve eðer yoksa ekle
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = go.AddComponent<MeshRenderer>();
        }

        meshRenderer.material = stackMat; // stackMat malzemesini atadýk

        ColorMesh(go.GetComponent<MeshFilter>().mesh);

    } // Create Rubble

    void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;

        if (stackIndex < 0)
        {
            stackIndex = transform.childCount - 1;
        }

        desiredPosition = Vector3.down * scoreCount;

        theStack[stackIndex].transform.localPosition = new Vector3(0f, scoreCount, 0f);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x ,1f, stackBounds.y);

        ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);

    }



} // CLASS
