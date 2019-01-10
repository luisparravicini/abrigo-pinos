using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Cave : MonoBehaviour
{
    public Vector2Int size;
    public float stepTime;
    public int iterations;
    public float rockProbability;
    public int horizontalBlankingHeight;
    public int horizontalBlankingWidth;
    public int maxIterationsForR2Cutoff;
    public GameObject rockPrefab;
    public GameObject floorPrefab;
    public Button btnStart;
    public Button btnReset;
    public Button btnWalk;
    public Movement walker;
    public GameObject mainCam;
    Coroutine generator;
    YieldInstruction stepWait;
    GameObject[,] blocks;
    bool walking;
    Vector3 baseDelta;

    private void Start()
    {
        Reset();
    }

    public void OnBtnStart()
    {
        btnStart.interactable = false;
        Generate();
    }

    public void OnBtnReset()
    {
        btnStart.interactable = true;
        Reset();
    }

    public void OnBtnWalk()
    {
        walking = !walking;

        if (walking)
        {
            var delta = transform.TransformPoint(baseDelta);
            walker.Setup(delta, new Vector2Int(size.x / 2, size.y / 2));
        }
        btnStart.interactable = btnReset.interactable = !walking;
        btnWalk.GetComponentInChildren<Text>().text = (walking ? "Exit" : "Walk");

        walker.gameObject.SetActive(walking);
        mainCam.SetActive(!walking);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 1, size.y));
    }

    private void Reset()
    {
        btnWalk.interactable = false;

        if (generator != null)
        {
            StopCoroutine(generator);
            generator = null;
        }

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        blocks = new GameObject[size.x, size.y];
        baseDelta = new Vector3(-size.x / 2 + 0.5f, 0, -size.y / 2 + 0.5f);
        CreateUI();
    }

    public void Generate()
    {
        Reset();
        stepWait = new WaitForSeconds(stepTime);
        generator = StartCoroutine(GenerateSteps());
    }

    void CreateUI()
    {
        var pos = baseDelta + new Vector3(size.x / 2, -0.5f, size.y / 2);
        var floor = Instantiate(floorPrefab, pos, floorPrefab.transform.rotation);
        floor.transform.SetParent(transform, false);
        floor.transform.localScale = new Vector3(size.x, size.y, 0);

        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
            {
                pos = baseDelta + new Vector3(x, 0, y);
                var block = Instantiate(rockPrefab, pos, rockPrefab.transform.rotation);
                block.transform.SetParent(transform, false);
                blocks[x, y] = block;
            }
    }

    void Redraw(bool[,] data)
    {
        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
                blocks[x, y].SetActive(data[x, y]);
    }

    IEnumerator GenerateSteps()
    {
        var automata = new CellularAutomataCaves(size, rockProbability, horizontalBlankingHeight, horizontalBlankingWidth, maxIterationsForR2Cutoff);

        int n = 0;
        automata.Start();

        Redraw(automata.data);
        yield return stepWait;

        while (n < iterations)
        {
            automata.Step();
            Redraw(automata.data);
            n += 1;
            yield return stepWait;
        }

        generator = null;
        btnWalk.interactable = true;
        btnStart.interactable = true;
    }

}
