using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonesFinder
{
    const int NotVisited = 0;

    Vector2Int size;
    bool[,] data;
    public int[,] zones { get; private set; }
    int curZoneId;

    public ZonesFinder(bool[,] data)
    {
        size = new Vector2Int(data.GetLength(0), data.GetLength(1));
        this.data = data;
        zones = new int[size.x, size.y];
    }

    public int NumberOfZones { get { return curZoneId; } }

    public void Find()
    {
        curZoneId = NotVisited;

        for (var y = 0; y < size.y; y++)
        {
            for (var x = 0; x < size.x; x++)
            {
                if (zones[x, y] != NotVisited || data[x, y])
                    continue;

                curZoneId++;
                FindZone(x, y);
            }
        }
    }

    private void FindZone(int x, int y)
    {
        var toVisit = new Queue<Vector2Int>();
        toVisit.Enqueue(new Vector2Int(x, y));

        var deltas = new Vector2Int[]
        {
            new Vector2Int(0,1),
            new Vector2Int(-1,0),
            new Vector2Int(1,0),
            new Vector2Int(0,-1),
        };
        while (toVisit.Count != 0)
        {
            var pos = toVisit.Dequeue();

            zones[pos.x, pos.y] = curZoneId;

            foreach (var delta in deltas)
            {
                var neighbour = pos + delta;

                if (neighbour.x < 0 || neighbour.x >= size.x ||
                neighbour.y < 0 || neighbour.y >= size.y)
                    continue;

                if (zones[neighbour.x, neighbour.y] != NotVisited)
                    continue;

                if (data[neighbour.x, neighbour.y])
                    continue;

                if (!toVisit.Contains(neighbour))
                    toVisit.Enqueue(neighbour);
            }
        }
    }
}
