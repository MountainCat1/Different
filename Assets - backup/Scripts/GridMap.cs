using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public static GridMap Instance { get; private set; }

    public static event Action<GridMap> MapChanged;

    public Dictionary<Vector2Int, GridTile> Tiles { get; set; }
    public HashSet<Vector2Int> StaticlyBlockedTiles { get; set; }
    public List<TriggerCollider> TriggerColliders { get; set; }
    public List<TCASystem.TriggerObject> SingleTileTriggers { get; set; }
    public List<GridCollider> DynamicGridColliders { get; set; }

    public Vector2Int mapSize;

    public bool drawCollisions = true;
    public bool autoGenerateMap = true;

#if (UNITY_EDITOR)
    private void OnDrawGizmos()
    {
        if (autoGenerateMap && !Application.isPlaying)
            GenerateMap();
        if (drawCollisions)
            DrawMapGizmos();
        
    }
#endif

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            Debug.LogError("Singeleton duplicated!");
        }

        Instance = this;
    }

    private void Start()
    {
        GenerateMap();
    }

    public bool CanWalk(Vector2Int pos)
    {
        if(!Tiles.ContainsKey(pos))
            return false;

        return CheckCollision(pos);
    }
    public List<TriggerCollider> GetTriggers(Vector2Int gridPosition)
    {
        List<TriggerCollider> ret = new List<TriggerCollider>();
        foreach (TriggerCollider triggerCollider in TriggerColliders)
        {
            List<Vector2Int> collider = triggerCollider.GetCollisions().ToList();

            if (collider.Any(x => x == gridPosition))
            {
                ret.Add(triggerCollider);
            }
        }
        return ret;
    }
    public List<Vector2Int> GetBlockedTiles()
    {
        List<Vector2Int> blockedTiles = new List<Vector2Int>();

        foreach (GridCollider gridCollider in DynamicGridColliders)
        {
            blockedTiles.AddRange(gridCollider.GetCollisions());
        }

        blockedTiles.AddRange(StaticlyBlockedTiles);

        return blockedTiles;
    }
    private void DrawMapGizmos() {
        List<Vector2Int> triggerTiles = TriggerColliders.Select(x => x.GetCollisions())
            .SelectMany(x => x)
            .ToList();

        List<Vector2Int> singleTriggerTilesTiles = SingleTileTriggers
            .Select(x => Vector2Int.RoundToInt(x.transform.position))
            .ToList();

        foreach (Vector2Int pos in Tiles.Keys.Except(GetBlockedTiles()))
        {
            DrawXGizmo((Vector2)pos * GridCharacterController.GridCellWidth, Color.green);
        }
        foreach (Vector2Int pos in triggerTiles)
        {
            DrawXGizmo((Vector2)pos * GridCharacterController.GridCellWidth, Color.blue);
        }
        foreach (Vector2Int pos in singleTriggerTilesTiles)
        {
            DrawXGizmo((Vector2)pos * GridCharacterController.GridCellWidth, Color.cyan);
        }
    }
    private void GenerateMap()
    {
        Tiles = new Dictionary<Vector2Int, GridTile>();
        StaticlyBlockedTiles = new HashSet<Vector2Int>();
        TriggerColliders = new List<TriggerCollider>();
        SingleTileTriggers = new List<TCASystem.TriggerObject>();

        var staticColliders = Resources.FindObjectsOfTypeAll(typeof(StaticGridCollider));
        var dynamicColliders = Resources.FindObjectsOfTypeAll(typeof(GridCollider))
            .Where(x => !(x is StaticGridCollider))
            .Where(x => !(x is TriggerCollider))
            .Where(x => !(x is MapExtension));
        var triggerColliders = Resources.FindObjectsOfTypeAll(typeof(TriggerCollider));
        var extensions = Resources.FindObjectsOfTypeAll(typeof(MapExtension));
        var singleTileTriggers = Resources.FindObjectsOfTypeAll(typeof(TCASystem.TriggerObject))
            .Where(x => x is TCASystem.OnInteractTrigger);

        // Create map
        for (int x = -mapSize.x / 2; x < mapSize.x - mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y - mapSize.y / 2; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                GridTile tile = new GridTile(pos);


#if UNITY_EDITOR
                if(Application.isPlaying)
#endif
                    AddTileNeighbours(tile, pos);

                Tiles.Add(pos, tile);
                
            }
        }
        // Add map extensions
        foreach (MapExtension exit in extensions)
        {
            foreach (Vector2Int pos in exit.GetCollisions())
            {
                if (Tiles.ContainsKey(pos))
                    continue;

                GridTile tile = new GridTile(pos);
                Tiles.Add(pos, tile);
            }
        }

        // Save static colliders
        foreach(StaticGridCollider collider in staticColliders)
        {
            StaticlyBlockedTiles.UnionWith(collider.GetCollisions());
        }

        // Save trigger colliders
        TriggerColliders.AddRange(triggerColliders.Cast<TriggerCollider>());

        // Save singe tile triggers
        SingleTileTriggers.AddRange(singleTileTriggers.Cast<TCASystem.TriggerObject>());

        // Save dynamic colliders
        DynamicGridColliders = dynamicColliders
            .Cast<GridCollider>()
            .ToList();


        // Fire event
        MapChanged?.Invoke(this);
    }
    private void DrawXGizmo(Vector2 pos, Color color)
    {
        Debug.DrawLine(pos - new Vector2(0.1f, 0.1f), pos + new Vector2(0.1f, 0.1f), color);
        Debug.DrawLine(pos + new Vector2(-0.1f, 0.1f), pos + new Vector2(0.1f, -0.1f), color);
    }
    public static void DrawDebugMark(Vector2Int pos, Color color)
    {
#if (UNITY_EDITOR)
        Instance.DrawXGizmo(pos, color);
#endif
    }
    private bool CheckCollision(Vector2Int pos)
    {
        // Is tile blocked by static collider?
        foreach (var blockedTile in StaticlyBlockedTiles)
        {
            if (pos == blockedTile)
                return false;
        }
        // Is tile blocked by dynamic collider
        foreach (var dynamicCollider in DynamicGridColliders)
        {
            if (!dynamicCollider.enabled || !dynamicCollider.gameObject.activeInHierarchy)
                continue;

            if (dynamicCollider.GetCollisions().Contains(pos))
                return false;
        }

        return true;
    }

    private void AddTileNeighbours(GridTile tile, Vector2Int pos)
    {
        if(Tiles.TryGetValue(pos + new Vector2Int(1, 0), out GridTile tile1))
        {
            AddNeighbours(tile, tile1);
        }
        if (Tiles.TryGetValue(pos + new Vector2Int(-1, 0), out GridTile tile2))
        {
            AddNeighbours(tile, tile2);
        }
        if (Tiles.TryGetValue(pos + new Vector2Int(0, 1), out GridTile tile3))
        {
            AddNeighbours(tile, tile3);
        }
        if (Tiles.TryGetValue(pos + new Vector2Int(0, -1), out GridTile tile4))
        {
            AddNeighbours(tile, tile4);
        }
    }

    private void AddNeighbours(GridTile a, GridTile b)
    {
        a.Neighbours.Add(b);
        b.Neighbours.Add(a);
    }
}
public class GridTile
{
    public GridTile(Vector2Int position)
    {
        Position = position;
        RealPosition = (Vector2)position * GridCharacterController.GridCellWidth;
    }

    public List<GridTile> Neighbours { get; set; } = new List<GridTile>();
    public bool Walkable { get => GridMap.Instance.CanWalk(Position); }

    public Vector2Int Position { get; private set; }
    public Vector2 RealPosition { get; private set; }
}
