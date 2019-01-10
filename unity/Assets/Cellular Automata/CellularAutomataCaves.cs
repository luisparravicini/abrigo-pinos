﻿using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataCaves
{
    Vector2Int size;
    float rockProbability;
    int horizontalBlankingHeight;
    int horizontalBlankingWidth;
    public MazeSpec Maze { get; private set; }
    public bool[,] data { get; private set; }
    bool[,] newData;

    public CellularAutomataCaves(Vector2Int size, float rockProbability, int horizontalBlankingHeight, int horizontalBlankingWidth)
    {
        this.size = size;
        this.rockProbability = rockProbability;
        this.horizontalBlankingHeight = horizontalBlankingHeight;
        this.horizontalBlankingWidth = horizontalBlankingWidth;
    }

    public void Start()
    {
        Maze = new MazeSpec(size);

        data = new bool[size.x, size.y];
        newData = new bool[size.x, size.y];
        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
                data[x, y] = newData[x, y] = (x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1 || Random.value <= rockProbability);

        if (horizontalBlankingHeight > 0)
        {
            var y = (size.y - horizontalBlankingHeight) / 2;
            for (var dy = 0; dy < horizontalBlankingHeight; dy++)
            {
                var x = (size.x - horizontalBlankingWidth) / 2;
                for (var dx = 1; dx < horizontalBlankingWidth; dx++)
                    data[x + dx, y + dy] = false;
            }
        }
    }

    public void Step()
    {
        for (var y = 1; y < size.y - 1; y++)
            for (var x = 1; x < size.x - 1; x++)
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