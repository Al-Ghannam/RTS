#pragma strict
var selected: boolean = false;

function Start () {

}

function Update () {
	if(selected){
		transform.Find("Selection").GetComponent(MeshRenderer).enabled = true;
	}
	else{
		transform.Find("Selection").GetComponent(MeshRenderer).enabled = false;
	}
}