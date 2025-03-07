using Godot;
using System;

public partial class Chunk : Node3D
{
    public Map map => Map._Map;

    public enum ChunkStateEnum { NONE, REMOVE, NEW }

    public ChunkStateEnum ChunkState = ChunkStateEnum.NEW;

    MeshInstance3D Mesh;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Generata();
    }

    void Generata()
    {
        PlaneMesh plane = new PlaneMesh();
        plane.Size = new Vector2(map.ChunkWidth, map.ChunkHeigth);
        plane.SubdivideDepth = map.ChunkHeigth * map.ChunkResolution;
        plane.SubdivideWidth = map.ChunkWidth * map.ChunkResolution;

        plane.Material = GD.Load<OrmMaterial3D>("res://test/terrain.tres");

        var surface = new SurfaceTool();
        MeshDataTool data = new MeshDataTool();
        surface.CreateFrom(plane, 0);

        ArrayMesh arrayPlane = surface.Commit();
        data.CreateFromSurface(arrayPlane, 0);

        int verCount = data.GetVertexCount();
        for(int i = 0; i < verCount; i++) 
        {
            Vector3 vertex = data.GetVertex(i);
            float y = GetNoise(vertex.X, vertex.Z);
            vertex.Y = y;
            data.SetVertex(i, vertex);
        }

        arrayPlane.ClearSurfaces();
        data.CommitToSurface(arrayPlane);
        surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        surface.CreateFrom(arrayPlane, 0);
        surface.GenerateNormals();


        Mesh = new MeshInstance3D();
        Mesh.Mesh = surface.Commit();
        Mesh.CreateTrimeshCollision();
        Mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        Mesh.AddToGroup("NacSource");
        AddChild(Mesh);
    }

    float GetNoise(float x, float z)
    {
        Vector3 offset = Position;

        float value = map.Noise.GetNoise2D(x + offset.X, z + offset.Z) * 10;
        return value;
    }
}
