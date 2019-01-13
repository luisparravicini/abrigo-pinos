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
    public GameObject mainCam;
    Coroutine generator;
    YieldInstruction stepWait;
    GameObject[,] blocks;
    MeshRenderer[,] floorTiles;
    Vector3 baseDelta;
    Color floorStartColor;

    private void Start()
    {
        SetFloorColor();
        Reset();
    }

    private void SetFloorColor()
    {
        var floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
        floorStartColor = floor.GetComponent<MeshRenderer>().material.color;
        Destroy(floor);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 1, size.y));
    }

    private void Reset()
    {
        if (generator != null)
        {
            StopCoroutine(generator);
            generator = null;
        }

        baseDelta = new Vector3(-size.x / 2 + 0.5f, 0, -size.y / 2 + 0.5f);

        if (blocks == null || size != new Vector2Int(blocks.GetLength(0), blocks.GetLength(1)))
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            blocks = new GameObject[size.x, size.y];
            floorTiles = new MeshRenderer[size.x, size.y];
            CreateUI();
        }
        else
            ResetBlocksAndFloor();
    }

    private void ResetBlocksAndFloor()
    {
        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
            {
                blocks[x, y].SetActive(true);
                floorTiles[x, y].material.color = floorStartColor;
            }
    }

    public void Generate()
    {
        Reset();
        stepWait = new WaitForSeconds(stepTime);
        generator = StartCoroutine(GenerateSteps());
    }

    void CreateUI()
    {
        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
            {
                var pos = baseDelta + new Vector3(x, 0, y);

                var block = Instantiate(rockPrefab, pos, rockPrefab.transform.rotation);
                block.transform.SetParent(transform, false);
                blocks[x, y] = block;

                pos.y -= 0.5f;
                var floor = Instantiate(floorPrefab, pos, floorPrefab.transform.rotation);
                floor.transform.SetParent(transform, false);
                floorTiles[x, y] = floor.GetComponent<MeshRenderer>();
                floorTiles[x, y].material.color = floorStartColor;
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
        #region Cave creation
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
        #endregion

        #region Zones detection
        var zones = new ZonesFinder(automata.data);

        zones.Find();
        Debug.Log("Number of zones: " + zones.NumberOfZones);

        var colors = new Color?[zones.NumberOfZones + 1];
        for (var y = 0; y < size.y; y++)
        {
            for (var x = 0; x < size.x; x++)
            {
                var zoneId = zones.zones[x, y];
                if (colors[zoneId] == null)
                    colors[zoneId] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                floorTiles[x, y].material.color = colors[zoneId].Value;
            }
        }
        #endregion

        generator = null;
        btnStart.interactable = true;
    }

}
