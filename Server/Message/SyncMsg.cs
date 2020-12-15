
public class MsgSyncTank : MsgBase
{
	public MsgSyncTank() { msgName = "MsgSyncTank"; }
	//pos and rot of tank, rot of turret
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;
	public float ex = 0f;
	public float ey = 0f;
	public float ez = 0f;
	public float turretY = 0f;
	//from server
	public string id = "";      //which tank to sync
}


public class MsgFire : MsgBase
{
	public MsgFire() { msgName = "MsgFire"; }
	//initial pos and rot of bullet
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;
	public float ex = 0f;
	public float ey = 0f;
	public float ez = 0f;
	//from server
	public string id = "";      //which tank fire
}

//
public class MsgHit : MsgBase
{
	public MsgHit() { msgName = "MsgHit"; }
	//hit which tank
	public string targetId = "";
	//hit point
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;
	// from server
	public string id = "";      //hit tank's id
	public int hp = 0;          //hit tank's hp
	public int damage = 0;      //got damage
}