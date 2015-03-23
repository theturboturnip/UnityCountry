using UnityEngine;
using System.Collections;
//using System;
using System.Collections.Generic;

public class MakeWorld : MonoBehaviour {
	//Terrain
    public float MountainFreq = 25.0f;
    public float BumpMultiplier = 1f;
    public float HeightMultiplier = 1f;
    public float Roughness = 2.5f;
    public float BumpRoughness = 0f;
    public string seed = null;
    public bool randomSeed = true;
    public bool useWater = false;
    public bool ClipEdges=false;
    public float WaterLevel = 0.1f;
    public Vector3 dimensions=new Vector3(1024,256,1024);

    public Texture2D[] textures;
    public Texture2D grass;

    public GameObject BigTree,Tree;

    public Transform person,waterplane,city;

    public AnimationCurve heightCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

 	public AnimationCurve angleCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

	void generateWater(){
		Transform w;
		if(useWater)
			w=Instantiate(waterplane,new Vector3(0f,WaterLevel,0.0f),Quaternion.identity) as Transform;
		else
			w=new GameObject().transform;
		w.gameObject.name="Water";
		w.localScale*=100.0f;

	} 	

	public void generate(){
		generateWater();
		TerrainGeneration TG = new TerrainGeneration();
		TerrainGeneration.current=TG;
		TerrainData tData = new TerrainData();
 		tData.size = dimensions;
 		Terrain.CreateTerrainGameObject(tData);
        TG.SetMountainFreq = MountainFreq;
        TG.SetWaterlevel = WaterLevel;
        TG.BumpMultiplier = BumpMultiplier;
        TG.BumpRoughness = BumpRoughness;
        TG.HeightMultiplier = HeightMultiplier;
        TG.Roughness = Roughness;
        TG.terrain = Terrain.activeTerrain;
        TG.waterplane = GameObject.Find("Water");  
        TG.ClipEdges=ClipEdges;
        TerrainFoliage.waterLevel = WaterLevel; 
       	TG.editor = true;
       	if (randomSeed){
            TG.TerrainSeed = "" + (int)UnityEngine.Random.Range(0, int.MaxValue);
            seed = TG.TerrainSeed;
        } else
           	TG.TerrainSeed = seed;
        TG.makeHeightmap();
        generateTextures();
        generatePerson(TG);
        generateSun2();
        findCity(TG);
        TerrainFoliage.grass=grass;
        TerrainFoliage.grass2=grass;
        TerrainFoliage.BigTree=BigTree;
        TerrainFoliage.Tree=Tree;
        TerrainFoliage.GenerateFoliage(dimensions.x);
        TerrainFoliage.GenerateGrass();
	} 
	void generateSun2(){
		GetComponent<SkyColorCycle>().StartSunAndMoon(new Vector3(dimensions.x/2,0f,dimensions.z/2),dimensions.x/2);
	}
	void generateSun(){
		GameObject sun=new GameObject();
		sun.name="Sun";
		sun.transform.position=new Vector3(dimensions.x/2,4000f,dimensions.z/2);
		sun.transform.rotation=Quaternion.Euler(new Vector3(90f,0f,0f));
		sun.AddComponent("Light");
		Light sunl=sun.GetComponent<Light>();
		sunl.range=1000000f;
	}
	void generatePerson(TerrainGeneration TG){
		Vector3 point=new Vector3(dimensions.x/2,0f,dimensions.z/2);
		point.y=TG.terrain.SampleHeight(point)+2.1f;
		Instantiate(person,point,Quaternion.identity);
	}
	void findCity(TerrainGeneration TG){
		Vector2 point=Random.insideUnitCircle*(dimensions.x*0.15f);
		Vector3 start=new Vector3(point.x,0.0f,point.y);
		start+=dimensions*0.15f;
		start.y=TG.terrain.SampleHeight(start);
		/*for(int i=0;i<5;i++){
			placeBuilding(TG,start,50.0f);
		}*/
		Transform t=Instantiate(city,start,Quaternion.identity) as Transform;
		t.localScale=new Vector3(25.0f,0.0f,25.0f);
	}
	void placeBuilding(TerrainGeneration TG, Vector3 cityCentre, float cityRadius){
		Vector3 point=Random.insideUnitSphere*cityRadius;
		point+=cityCentre;
		point.y=TG.terrain.SampleHeight(point);
		//Instantiate(city,point,Quaternion.identity);
	}
	void generateTextures(){
		List<TTexture> Textures = new List<TTexture>();
		for(int i=0;i<textures.Length;i++){
			Texture2D t=textures[i];
			TTexture TTex = new TTexture();
			TTex.texture=t;
			TTex.tilesize=new Vector2(16,16);
			TTex.index=i;
			TTex.heightCurve=heightCurve;
			TTex.angleCurve=angleCurve;
			Textures.Add(TTex);
		}
		TerrainTexturing.GenerateTexture(Textures);
	}
	void Start () {
		generate();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
