public static class DepthMapBuilder
{
    public static DepthMap Build(TileMap map, (int x, int y) start)
    {
        // Entfernung pro Kachel, -1 = noch nicht besucht
        int[,] distance = new int[map.Width, map.Height];
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                distance[x, y] = -1;
            }
        }
        

        Queue<(int x, int y)> queue = new();

        // KORRIGIERT: Start ist die übergebene Startkachel, mit Entfernung 0
        distance[start.x, start.y] = 0;
        queue.Enqueue(start);
        int maxDistance = 0;

        while (queue.Count > 0)
        {
            (int x, int y) current = queue.Dequeue();
            int currentDistance = distance[current.x, current.y];

            foreach (Direction dir in Enum.GetValues<Direction>())
            {
                (int dx, int dy) = dir.Offset();
                (int x, int y) neighbor = (current.x + dx, current.y + dy);

                if (map.IsWall(neighbor.x, neighbor.y)) continue;      // Wand (auch außerhalb der Karte)
                if (distance[neighbor.x, neighbor.y] != -1) continue;  // schon besucht

                // NEU: Entfernung SOFORT eintragen. Das ist zugleich die "besucht"-Markierung,
                // dadurch landet jede Kachel nur einmal in der Schlange.
                distance[neighbor.x, neighbor.y] = currentDistance + 1;

                // KORRIGIERT: die echte größte Entfernung
                maxDistance = Math.Max(maxDistance, currentDistance + 1);

                queue.Enqueue(neighbor);
            }
        }

        // NEU (Schritt 4 der Aufgabe): Kontrolle auf abgetrennte Bereiche
        int unreachable = 0;
        for (int x = 0; x < map.Width; x++)
            for (int y = 0; y < map.Height; y++)
                if (!map.IsWall(x, y) && distance[x, y] == -1) unreachable++;

        if (unreachable > 0)
            Console.WriteLine($"WARNUNG: {unreachable} Boden-Kacheln sind vom Start aus nicht erreichbar");

        return new DepthMap(distance, maxDistance);
    }
}