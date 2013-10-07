
public var allUnits: GameObject[];
var selectables = new Array();
private var ctrlPressed:boolean = false;
var selectionBox:Texture;
private var boxSelecting:boolean = false;
private var dragStart:Vector2;
private var dragEnd:Vector2;
private var selectionPointStart:Vector3;
private var selectionPointEnd:Vector3;

function OnGUI () {
	if(boxSelecting){
		var selectionWidth = dragEnd.x - dragStart.x;
		var selectionHeight = (Screen.height - dragEnd.y) - (Screen.height - dragStart.y);
		var box:Rect = Rect(dragStart.x, Screen.height - dragStart.y, selectionWidth, selectionHeight);
		GUI.DrawTexture(box,selectionBox,ScaleMode.StretchToFill,true);
	}
}

function Update () {
	if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftShift)){
		ctrlPressed = true;
	}
	if(Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftShift)){
		ctrlPressed = false;
	}
	if(Input.GetMouseButtonDown(0)){
		shootRay();
		dragStart = Input.mousePosition;
	}
	//Update box drawing while the user is still holding LMB
	if(Input.GetMouseButton(0)){
		dragEnd = Input.mousePosition;
		getSelectionEndPoint();
	}
	if(Vector2.Distance(dragStart, dragEnd) > 10){
		boxSelecting = true;
	}
	if(Input.GetMouseButtonUp(0)){
		dragStart = Input.mousePosition;
		boxSelecting = false;
		getSelectionEndPoint();
	}
	if(boxSelecting){
		multiSelect();
	}
}

function shootRay(){
	var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	var hit: RaycastHit;
	var unitLayerMask = 1<<8;
	var rayGround = Camera.main.ScreenPointToRay(Input.mousePosition);
	var hitGround: RaycastHit;
	var groundLayerMask = 1<<9;
	
	if(Physics.Raycast(ray, hit, Mathf.Infinity, unitLayerMask)){
		if(!ctrlPressed){
			clearSelected();
		}
		var isSelected = hit.collider.GetComponent(Selectable).selected;
		if(isSelected && selectables.length >= 1 && ctrlPressed){
			hit.collider.GetComponent(Selectable).selected = false;
			selectables.remove(hit.collider);
		}
		else{
			hit.collider.GetComponent(Selectable).selected = true;
			selectables.Add(hit.collider);
		}
	}
	else if(Physics.Raycast(rayGround, hitGround, Mathf.Infinity, groundLayerMask)){
		clearSelected();
		selectionPointStart = hitGround.point;
	}
}

function getSelectionEndPoint(){
	var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	var hit:RaycastHit;
	var groundLayerMask = 1<<9;
	if(Physics.Raycast(ray,hit,Mathf.Infinity,groundLayerMask)){
		selectionPointEnd = hit.point;
	}
}

function multiSelect(){
	//Debug.Log("selectionPointStart.x: " + selectionPointStart.x + "selectionPointEnd.x: " + selectionPointEnd.x + "selectionPointStart.z: " + selectionPointStart.z + "selectionPointEnd.z: " + selectionPointEnd.z);
	var point1:Vector3;
	var point2:Vector3;
	
	if(selectionPointStart.x > selectionPointEnd.x){
		point1.x = selectionPointEnd.x;
		point2.x = selectionPointStart.x;
	}
	else{
		point1.x = selectionPointStart.x;
		point2.x = selectionPointEnd.x;
	}
	if(selectionPointStart.z < selectionPointEnd.z){
		point1.z = selectionPointEnd.z;
		point2.z = selectionPointStart.z;
	}
	else{
		point1.z = selectionPointStart.z;
		point2.z = selectionPointEnd.z;
	}
	for(var unit : GameObject in allUnits){
		var unitPos : Vector3 = unit.transform.position;
		if(unitPos.x > point1.x && unitPos.x < point2.x &&
			unitPos.z < point1.z && unitPos.z > point2.z){
				unit.GetComponent(Selectable).selected = true;
				selectables.Add(unit.collider);
			}
	}
}


function clearSelected(){
	for(var i=0; i<selectables.length; i++){
		selectables[i].GetComponent(Selectable).selected = false;
	}
	selectables.clear();
}