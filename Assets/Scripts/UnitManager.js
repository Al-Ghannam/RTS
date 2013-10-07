private static var instance : UnitManager;
private var allUnits = new Array();
private var selectedUnits = new Array();

//Returns the only instance of the unit manager.
//Use: UnitManager.getInstance();
static function getInstance(){
	if(instance == null){
		instance = FindObjectOfType(UnitManager);
	}
	return instance;
}



function Start () {

}

function Update () {

}