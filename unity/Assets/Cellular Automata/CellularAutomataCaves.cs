using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataCaves
{
    Vector2Int size;
    float rockProbability;
    public MazeSpec Maze { get; private set; }
    public bool[,] data { get; private set; }
    bool[,] newData;

    public CellularAutomataCaves(Vector2Int size, float rockProbability)
    {
        this.size = size;
        this.rockProbability = rockProbability;
    }

    public void Start()
    {
        Maze = new MazeSpec(size);

        data = new bool[size.x, size.y];
        newData = new bool[size.x, size.y];
        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
                data[x, y] = ( Random.value <= rockProbability);
    }

    public void Step()
    {
        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
            {
                var n = RocksAround(x, y);
                newData[x, y] = (n >= 5);
                if (data[x, y] && n < 2) newData[x, y] = false;
            }

        var aux = data;
        data = newData;
        newData = aux;
    }

    private int RocksAround(int x, int y)
    {
        var n = 0;
        Vector2Int pos = Vector2Int.zero;

        for (var dy = -1; dy <= 1; dy++)
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                pos.x = x + dx;
                pos.y = y + dy;
                if (pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y)
                    continue;

                if (data[pos.x, pos.y])
                    n += 1;
            }

        return n;
    }
}
