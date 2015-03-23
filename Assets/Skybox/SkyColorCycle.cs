using UnityEngine;
using System.Collections;
using System;

public class SkyColorCycle : MonoBehaviour {
	//Color variables
	float progress=0.0f,progressPerSecond,seconds=0f;
	public float secondsPerCycle;
	public AnimationCurve r,g,b,a;
	//Star variables
	GameObject starHolder,sun,moon;
	public Color sunColor=Color.white;
	float secondsPerRotation,rotationPerSecond; 
	public Flare SunFlare;
	Vector3 centre;
	// Use this for initialization
	void Start () {
		progressPerSecond=1f/secondsPerCycle;
		secondsPerRotation=secondsPerCycle;
		rotationPerSecond=360f/secondsPerRotation;
	}
	
	// Update is called once per frame
	void Update () {
		seconds+=Time.deltaTime;
		//updateColor();
		updateLights();
	}
	public void StartSunAndMoon(Vector3 centre_, float radius){
		centre=centre_;
		starHolder=new GameObject();
		starHolder.name="Star Holder";
		starHolder.transform.position=centre;
		sun=new GameObject();
		sun.name="Sun";
		sun.transform.parent=starHolder.transform;
		sun.transform.localPosition=Vector3.right*(radius+1f);
		sun.AddComponent("Light");
		Light sunLight=sun.GetComponent<Light>();
		sunLight.type=LightType.Directional;
		sunLight.color=sunColor;
		sunLight.flare=SunFlare;
	}
	void updateColor(){
		progress+=Time.deltaTime*progressPerSecond;
		float r_=r.Evaluate(progress),g_=g.Evaluate(progress),b_=b.Evaluate(progress),a_=a.Evaluate(progress);
		Camera.main.backgroundColor=new Color(r_,g_,b_,a_);
	}
	void updateLights(){
		if(sun)
			sun.transform.LookAt(centre);
		if(starHolder)
			starHolder.transform.Rotate(Vector3.forward*Time.deltaTime*rotationPerSecond);
	}
}
