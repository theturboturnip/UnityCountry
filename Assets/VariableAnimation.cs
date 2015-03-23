using UnityEngine;
using System.Collections;

public class VariableAnimation : MonoBehaviour {
	public GameObject leftArm,rightArm;
	// Use this for initialization
	void Start (){
	}
	
	// Update is called once per frame
	void Update () {
		leftArm.transform.Rotate(Vector3.right,Time.deltaTime*20);
	}
}
