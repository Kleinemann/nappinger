using Godot;
using System;
using System.Drawing;

public partial class Chunk : MeshInstance3D
{
	public Map map => Map._Map;
	public enum ChunkStateEnum { NONE, REMOVE, NEW}

	public ChunkStateEnum ChunkState = ChunkStateEnum.NEW;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Generata();
	}

	void Generata()
	{
		Mesh = new PlaneMesh();
        Mesh.Size = new Vector2(map.ChunkWidth, map.ChunkHeigth);
        Mesh.SubdivideDepth = map.ChunkHeigth * map.ChunkResolution;
        Mesh.SubdivideWidth = map.ChunkWidth * map.ChunkResolution;

        Mesh.Material = GD.Load<OrmMaterial3D>("res://test/terrain.tres");
		var surface = new SurfaceTool();
		surface.CreateFrom(mesh, 0);

		this.Mesh = surface.Commit();
		
	}
}
