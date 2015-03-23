using CoherentNoise;
using CoherentNoise.Generation.Combination;
using CoherentNoise.Generation.Fractal;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFoliage : MonoBehaviour {

    public static float waterLevel { get; set; }
    public static float maxSteepness { get; set; }
    public static int grassDensity { get; set; }
    public static Texture2D grass,grass2;
    public static GameObject BigTree,Tree;

    public static void GenerateFoliage(float size)
    {
        for(int i=0;i<50;i++){
            Vector3 v=new Vector3(Random.value*size,0.0f,Random.value*size);//+new Vector3(size/2,0f,size/2);
            GenerateTrees2(v,Random.value*30f,(int)(Random.value*10));
        }
        GenerateTrees2(Vector3.zero,1f,1);
    }

    private static void GenerateTrees()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;

        TreePrototype[] treeprototypes = new TreePrototype[] { new TreePrototype() { prefab = BigTree }, new TreePrototype() { prefab = Tree } };

        td.treePrototypes = treeprototypes;

        //float[, ,] splatmaps = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
        td.treeInstances = new TreeInstance[0];

        List<Vector3> treePos = new List<Vector3>();
        print(td.alphamapWidth+","+td.alphamapHeight);
        float[,] noisemap = new float[td.alphamapWidth, td.alphamapHeight];
        Generator noise_tree = new Max(
            new PinkNoise((int)UnityEngine.Random.Range(0, int.MaxValue)) { Frequency = 0.01f, OctaveCount = 6, Persistence = 0.66f, Lacunarity = 0.1f },
            new PinkNoise((int)UnityEngine.Random.Range(0, int.MaxValue)) { Frequency = 0.015f, OctaveCount = 2, Persistence = 0.66f, Lacunarity = 0.2f });
        for (int ny = 0; ny < noisemap.GetLength(1); ny++)
        {
            for (int nx = 0; nx < noisemap.GetLength(0); nx++)
            {
                noisemap[nx, ny] = noise_tree.GetValue(nx, ny, 0);
            }
        }
        if (maxSteepness == 0) { maxSteepness = 70.0f; }
        if (waterLevel == 0) { waterLevel = 0.0f; }
        float x = 0.0f;
        while (x < td.alphamapWidth)
        {
            float y = 0.0f;
            while (y < td.alphamapHeight)
            {
                float height = td.GetHeight((int)x, (int)y);
                float heightScaled = height / td.size.y;
                float xScaled = (x + Random.Range(-1f, 1f)) / td.alphamapWidth;
                float yScaled = (y + Random.Range(-1f, 1f)) / td.alphamapHeight;
                float steepness = td.GetSteepness(xScaled, yScaled);

                if (Random.Range(0f, 1f) > 1f - noisemap[(int)x, (int)y] * 2f && steepness < maxSteepness && height > waterLevel)
                {
                    treePos.Add(new Vector3(xScaled, heightScaled, yScaled));
                }

                y++;
            }
            x++;
        }

        TreeInstance[] treeInstances = new TreeInstance[treePos.Count];

        for (int ii = 0; ii < treeInstances.Length; ii++)
        {
            treeInstances[ii].position = treePos[ii];
            treeInstances[ii].prototypeIndex = Random.Range(0, treeprototypes.Length);
            treeInstances[ii].color = Color.white;//new Color(Random.Range(200, 255), Random.Range(200, 255), Random.Range(200, 255));
            treeInstances[ii].lightmapColor = Color.white;
            treeInstances[ii].heightScale = 1.0f + Random.Range(-0.25f, 0.5f);
            treeInstances[ii].widthScale = 1.0f + Random.Range(-0.5f, 0.25f);
        }
        td.treeInstances = treeInstances;
    }
    public static void GenerateTrees2(Vector3 center,float radius,int treeNum){
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;

        TreePrototype[] treeprototypes = new TreePrototype[] { new TreePrototype() { prefab = (GameObject)Resources.Load("BigTree") }, new TreePrototype() { prefab = (GameObject)Resources.Load("Tree") } };

        td.treePrototypes = treeprototypes;
        List<Vector3> treePos = new List<Vector3>();

        int i=0;
        while(i<treeNum){
            Vector2 point=Random.insideUnitCircle*radius;
            float x=point.x+center.x;
            float y=point.y+center.z;
            float height = td.GetHeight((int)x, (int)y);
            float heightScaled = height / td.size.y;
            float xScaled = (x + Random.Range(-1f, 1f)) / td.alphamapWidth;
            float yScaled = (y) / td.alphamapHeight;
            float steepness = td.GetSteepness(xScaled, yScaled);
            treePos.Add(new Vector3(xScaled, heightScaled, yScaled));
            i++;
        }

        TreeInstance[] treeInstances = new TreeInstance[treePos.Count+td.treeInstances.Length];
        for (i=0;i<td.treeInstances.Length;i++){
            treeInstances[i]=td.treeInstances[i];
        }
        for (i=i+1; i < treeInstances.Length; i++)
        {
            treeInstances[i].position = treePos[i-td.treeInstances.Length];
            treeInstances[i].prototypeIndex = Random.Range(0, treeprototypes.Length);
            treeInstances[i].color = Color.white;
            treeInstances[i].lightmapColor = Color.white;
            treeInstances[i].heightScale = 1.0f;
            treeInstances[i].widthScale = 1.0f + Random.Range(-0.5f, 0.25f);
        }
        td.treeInstances = treeInstances;
    }
    public static void ClearTrees()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;
        TreePrototype[] treeprototypes = new TreePrototype[0];
        td.treePrototypes = treeprototypes;
        
    }

    public static void GenerateGrass()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;
        td.SetDetailResolution(256,8);

        if (grassDensity == 0)
        {
            grassDensity = 5;
        }

        DetailPrototype[] detailPrototypes = new DetailPrototype[2];

        detailPrototypes[0] = new DetailPrototype() { prototypeTexture = grass };
        detailPrototypes[1] = new DetailPrototype() { prototypeTexture = grass2 };

        td.detailPrototypes = detailPrototypes;
        for (int i = 0; i < td.detailPrototypes.Length; i++)
        {
            int[,] detailLayer=td.GetDetailLayer(0, 0, td.detailWidth, td.detailHeight, i);
            int x = 0;
            while (x < td.detailWidth)
            {
                int y = 0;
                while (y < td.detailHeight)
                {
                    detailLayer[x, y] = (int)Random.Range(0.0f,10.0f);
                    y++;
                }
                x++;
            }
            td.SetDetailLayer(0, 0, i, detailLayer);
        }   
    }

    public static void ClearGrass()
    {
        Terrain t = Terrain.activeTerrain;
        TerrainData td = t.terrainData;
        DetailPrototype[] detailPrototypes = new DetailPrototype[0];
        td.detailPrototypes = detailPrototypes;
    }
}
