using System.Net.WebSockets;

public static class RoomTemplateParser
{
    public static RoomTemplate Parse(string name, string text)
    {
        string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if(lines.Length == 0) throw new InvalidDataException($"Room {name} is empty");

        int w = lines[0].Length;
        int h = lines.Length;

        for (int i = 1; i < h; i++)
        {
            string line = lines[i];
            if(line.Length != w)
            {
                throw new InvalidDataException($"Room {name} got invalid line {i}");
            }
        }

        Tile[,] tiles = new Tile[w, h];
        List<Connector> connectors = [];
        List<LootSpot> lootSpots = [];
        List<LightSource> lightSources = [];
        ExtractionPoint extractionPoint = new(-1, -1);

        for(int y = 0; y < lines.Length; y++)
        {
            string line = lines[y];
            for(int x = 0; x < line.Length; x++)
            {
                switch (line[x]){
                    case '#':
                        tiles[x, y] = Tile.Wall;
                        break;
                    case '.':
                        tiles[x, y] = Tile.Floor;
                        break;
                    case 'D':
                        tiles[x, y] = Tile.Floor;
                        Connector c = new(x, y, GetDoorDirectionInRoom(name, x, y, w, h));
                        connectors.Add(c);
                        break;
                    case 'L':
                    case 'C':
                        if(x == 0 || y == 0 || x == w - 1 || y == h - 1) throw new InvalidDataException("Loot not be on the edge");
                        tiles[x, y] = Tile.Floor;
                        LootSpotKind kind = lines[x][y] == 'C' ? LootSpotKind.Container : LootSpotKind.Loose;
                        LootSpot lootSpot = new(x, y, kind);
                        lootSpots.Add(lootSpot);
                        break;
                    case 'E':
                        extractionPoint = extractionPoint with {TileX = x, TileY = y};
                        break;
                    case 'H':
                        // LightSource light = new(6, 100f, 120, x, y);
                        LightSource light = new(x, y, 192f, 0.9f);
                        lightSources.Add(light);
                        break;
                    default:
                        throw new InvalidDataException($"Room {name}: unkwon character '{lines[x][y]}' at ({x},{y})");
                }
            }
        }
        return new RoomTemplateBuilder(name, w, h)
                .AddTiles(tiles)
                .AddConnector(connectors)
                .AddLootSpot(lootSpots)
                .AddLightSource(lightSources)
                .AddExtractionPoint(extractionPoint)
                .Build();
        // return new(name, w, h, tiles, connectors, lootSpots, extractionPoint);
    }

    public static Direction GetDoorDirectionInRoom(string roomName, int x, int y, int width, int height)
    {
        bool north = y == 0;
        bool south = y == height - 1;
        bool west = x == 0;
        bool east = x == width - 1;

        int edgeCount = (north ? 1 : 0) + (south ? 1 : 0) + (west ? 1 : 0) + (east ? 1 : 0);
        if(edgeCount != 1)
        {
            throw new InvalidDataException($"Room {roomName}: door at ({x},{y}) must be on one edge (not corner)");
        }
        if(north) return Direction.North;
        if(south) return Direction.South;
        if(west) return Direction.West;
        return Direction.East;
    }
}