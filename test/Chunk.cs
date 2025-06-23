using Godot;
using System;
using System.Data;

public partial class Chunk : Node3D
{
    public Map map => Map._Map;

    public enum ChunkStateEnum { NONE, REMOVE, NEW }

    public ChunkStateEnum ChunkState = ChunkStateEnum.NEW;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Generate();
    }

    void Generate()
    {
        PlaneMesh planeMesh = new PlaneMesh();
        planeMesh.Size = new Vector2(map.ChunkWidth, map.ChunkHeigth);
        planeMesh.SubdivideDepth = map.ChunkHeigth * map.ChunkResolution;
        planeMesh.SubdivideWidth = map.ChunkWidth * map.ChunkResolution;
        planeMesh.Material = GD.Load<ShaderMaterial>("res://test/Materials/TerrainMaterial.tres");

        SurfaceTool surface = new SurfaceTool();
        MeshDataTool data = new MeshDataTool();
        surface.CreateFrom(planeMesh, 0);

        ArrayMesh arrayPlane = surface.Commit();
        data.CreateFromSurface(arrayPlane, 0);

        for(int i = 0; i < data.GetVertexCount(); i++)
        {
            Vector3 vertex = data.GetVertex(i);
            vertex.Y = GetNoise(vertex.X, vertex.Z);
            data.SetVertex(i, vertex);
            data.SetVertexColor(i, getColor(vertex.Y));
        }

        arrayPlane.ClearSurfaces();
        
        data.CommitToSurface(arrayPlane);
        surface.Begin(Mesh.PrimitiveType.Triangles);
        surface.CreateFrom(arrayPlane, 0);
        surface.GenerateNormals();

        MeshInstance3D mesh = new MeshInstance3D();
        mesh.Mesh = surface.Commit();
        mesh.CreateTrimeshCollision();
        mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        mesh.AddToGroup("NavSource");
        AddChild(mesh);
    }


    void AddItems()
    {

    }

 

    Color getColor(float high)
    {
        Color c;
        Random rand = new Random();
        int i = rand.Next(0, 3);

        switch (i)
        {
            case 0: c = Colors.WhiteSmoke; break;
            case 1: c = Colors.Yellow; break;
            case 2: c = Colors.LightBlue; break;
            case 3: c = Colors.LightCoral; break;
            default: c = Colors.Black; break;
        }

        return c;
    }

    float GetNoise(float x, float z)
    {
        Vector3 offset = Position;

        float value = map.Noise.GetNoise2D(x + offset.X, z + offset.Z) * map.VertexMulti;
        return value;
    }
}
