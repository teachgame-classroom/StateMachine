using UnityEngine;
using System.Collections;

/*
    DA_AnimatorControl is script to control the characters in demoscene.
    will move character , play animation , the position of the weapon , play effect , reaction of key input.
    
	DA_AnimatorControlはデモシーンに配置されたキャラクターを制御するスクリプトです。.
    キャラクターの移動、アニメーションの再生、武器の表示位置、エフェクトの発生、キー入力に対するリアクションを行います。.

    DA_AnimatorControl은 데모신에 배치된 캐릭터를 제어하기위한 스크립트 입니다.
    캐릭터의 이동, 애니메이션의 재생, 무기의 위치, 이펙트의 발생, 키 입력에 대한 반응을 합니다. 
	
	2017.12.23
*/

public class DA_AnimatorControl : MonoBehaviour {

	// required Object or component
	//　制御に必要なオブジェクトなど。
	//　필요한 컴포넨트, 오브젝트 등등.

	public Animator chrAnimator;    // Animator component of character.
	public RuntimeAnimatorController[] chrAnimatorController;// AnimatorController for viewer and interactive
	public CharacterController chrController;    // CharacterController component.
	[Space(20)]
	public Transform weaponLeft;	// root object of weapon for left hand.
	public Transform weaponRight;	// root object of weapon for right hand.
	public Transform[] weaponPoint_L = new Transform[2]; // Where weapon object placed. loc_wb_L(R) for spine , loc_Weapon_L(R) for handheld)
	public Transform[] weaponPoint_R = new Transform[2];
	public bool OffsetSD = false; // SD Character need offset of weapon when she equip axe, and climb ladder
	private Weapon_AnimatorControl weaponControl_L;
	private Weapon_AnimatorControl weaponControl_R;	// Animator of weapon.

	[Space(20)]
	public Transform sideBagLeft;	// sideBag Model.
	public Transform sideBagRight;
	public Transform sideBagPoint_L; // locator of sideBag
	public Transform sideBagPoint_R;

	// List of gameobject which have meshrenderer component or skinnedmeshrenderer component.
	// This used to calculate of vertices, triangles, bone count.
	private GameObject[] characterMesh;
	private GameObject[] weaponMesh_L;
	private GameObject[] weaponMesh_R;
	private GameObject[] sideBagMesh_L;
	private GameObject[] sideBagMesh_R;


	// to control movement of characters , such as jumps.
	//　キャラクターの移動や、アニメータをコントロールするもの。.
	//　캐릭터의 이동, 점프등을 제어하기 위해서 필요한것.
	[Space(20)]
	public float jumpSpeed = 8.0f;
	public float moveAbilityInAir = 4.0f;
	private float jumpAmount = 0.0f;
	[Space(20)]
	public float blowOffPow = 8.0f; // when character blowed off.
	private bool isBlowOff = false; // when character blowed off.
	private float runParam = 0.0f;
	private bool fpsLikeMoving = false; // change move mode to free run or fps like.
	private Vector3 moveDirection = Vector3.zero;
	private float gravity = 10.0f;

	private bool isClimbLadder = false;
	private bool isLadderTop = false;
	private float laddarEndHeightTop = 0.0f;
	private float laddarEndHeightBottom = 0.0f;

	// Save the state in playing now.
	private AnimatorStateInfo stateInfo; 

	void Awake() 
	{
		// Make list of gameobject which have meshrenderer component or skinnedmeshrenderer component.
		// vertices, triangles, bone count will appear in right bottom UI.
		for (int i = 0; i < transform.childCount; i++) {
			GameObject go = transform.GetChild(i).gameObject;
			if(go.name.StartsWith("mdl_body")){
				characterMesh = CollectMeshRenderer (go);
				break;
			}
		}
		if (weaponLeft) {
			weaponMesh_L = CollectMeshRenderer (weaponLeft.gameObject);
			// Weapon_AnimatorControl will control weapon's animator.
			if(weaponLeft.gameObject.GetComponent<Weapon_AnimatorControl> ())
				weaponControl_L = weaponLeft.gameObject.GetComponent<Weapon_AnimatorControl> ();
		}
		if (weaponRight) {
			weaponMesh_R = CollectMeshRenderer (weaponRight.gameObject);
			if(weaponRight.gameObject.GetComponent<Weapon_AnimatorControl> ())
				weaponControl_R = weaponRight.gameObject.GetComponent<Weapon_AnimatorControl> ();
		}

		if (sideBagLeft) {
			sideBagMesh_L = CollectMeshRenderer (sideBagLeft.gameObject);
			// Attach SideBag.
			sideBagLeft.SetParent (sideBagPoint_L, false);
		}
		if (sideBagRight) {
			sideBagMesh_R = CollectMeshRenderer (sideBagRight.gameObject);
			// Attach SideBag.
			sideBagRight.SetParent (sideBagPoint_R, false);
		}
	}

	void Update() 
	{
		// Save the state in playing now.
		//　再生中のステートの情報を入れる。.
		// 재생중인 스테이트를 저장.
		stateInfo = chrAnimator.GetCurrentAnimatorStateInfo(0);

		// Integer parameter reset to 0. 
		if(stateInfo.IsTag("Idle"))
			chrAnimator.SetInteger("AttackIdx", 0);
		chrAnimator.SetInteger("DamageIdx", 0);
		
		// reaction of key input.
		// for Attack
		if(Input.GetButtonDown("Fire1") && !Input.GetMouseButton(0))	SetAttack();
		
		// style change
		if(Input.GetButton("Fire2") && Input.GetButtonDown("Fire3"))	SetWeapon(0);
		if(Input.GetButton("Fire2") && Input.GetKeyDown("z"))	SetWeapon(1);
		if(Input.GetButton("Fire2") && Input.GetKeyDown("x"))	SetWeapon(2);

		// for SideStep
		if(Input.GetKeyDown("v"))	chrAnimator.SetTrigger("Step_L");
		if(Input.GetKeyDown("b"))	chrAnimator.SetTrigger("Step_R");
		if(Input.GetKeyDown("n"))	chrAnimator.SetTrigger("Step_B");

		// for Sliding
		if(Input.GetKeyDown("h"))	chrAnimator.SetTrigger("Sliding");

		// for Talking
		if(Input.GetKeyDown("j"))	chrAnimator.SetBool("Talk", !chrAnimator.GetBool("Talk"));

		// for Spell
		if(Input.GetKeyDown("x"))	chrAnimator.SetBool("CastSpell", true);
		if(Input.GetKeyUp("x"))	chrAnimator.SetBool("CastSpell", false);

		// for Guard
		if(Input.GetKeyDown("c"))	chrAnimator.SetBool("Guard", true);
		if(Input.GetKeyUp("c"))	chrAnimator.SetBool("Guard", false);


		// Damage Motion
		// random one out of three .
		if(Input.GetKeyDown("f")){
			int damageIdx = Random.Range(1, 3);
			chrAnimator.SetInteger("DamageIdx", damageIdx);
		}
		// Blow off
		if(Input.GetKeyDown("g")){
			chrAnimator.SetTrigger("BlowOff");
		}


		// for Stun
		if(Input.GetKeyDown("k"))	chrAnimator.SetBool("Stun", !chrAnimator.GetBool("Stun") );

		// for Dead
		if(Input.GetKeyDown("m"))	chrAnimator.SetBool("Dead", !chrAnimator.GetBool("Dead"));
		
		
		// movement.
		// Input of character moves	
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		
		Vector3 axisInput = new Vector3(h, 0, v);
		float axisInputMag = axisInput.magnitude;
		if(axisInputMag > 1){
			axisInputMag = 1;
			axisInput.Normalize();
		}
		runParam = 0f;
		// When climbing ladder, do not control character rotation.
		// In fbs like move mode, Camera rotate by character movement.
		if (stateInfo.IsTag ("Ladder") || stateInfo.IsTag ("LadderStartEnd")) {
			if (fpsLikeMoving) {
				Camera.main.transform.root.rotation = transform.rotation;
				Camera.main.transform.root.Rotate (0, 180, 0);
			}
		}
		// Usual movement is control character direction and camera by camera orientation.
		else if(axisInputMag != 0 && !isClimbLadder){
			// for run
			if(Input.GetKey("z"))
				runParam = 0.7f;

			axisInput = Camera.main.transform.rotation * axisInput;
			axisInput.y = 0;
			// character rotate by scipt
			// free move
			if (!fpsLikeMoving && axisInput != Vector3.zero) {
				transform.forward = axisInput;
			}
			// fps like move
			else {
				// character rotate by animation. camera rotate by script
				chrAnimator.SetFloat ("Rotate", h);
				Camera.main.transform.root.rotation = transform.rotation;
				Camera.main.transform.root.Rotate (0, 180, 0);
			}
		}
		chrAnimator.SetFloat ("Speed", (axisInputMag * 0.3f + runParam));

		// Climp Ladder
		// When climb ladder operates the LadderMove parameter.
		// 梯子を登る時はLadderMoveパラメータを操作し、梯子の上の高さと下の高さより外に出ないようにします。.
		// 사다리를 타는 동안은 LadderMove를 조작하고, 사다리의 상단, 하단 좌표를 넘어 가지 않도록합니다.	`
		if ( stateInfo.IsTag("Ladder") || stateInfo.IsTag ("LadderStartEnd")) {
			// fall down from ladder
			if ( Input.GetButtonDown("Jump") ) {
				isClimbLadder = false;
				chrAnimator.SetBool("ClimbLadder", false);
				chrAnimator.SetInteger ("JumpIdx", 1);
				chrAnimator.SetFloat("JumpVelocity", -10f );
				chrAnimator.SetTrigger ("Fall");
				return;
			}

			Vector3 currentClimbHeight = transform.position;
			// Input of Vertical Axis to switch Movement.
			if (isClimbLadder) {
				if (v > 0 && stateInfo.IsTag("Ladder"))
					chrAnimator.SetInteger ("LadderMove", 1);
				else if (v < 0 && stateInfo.IsTag("Ladder"))
					chrAnimator.SetInteger ("LadderMove", -1);
				else
					chrAnimator.SetInteger ("LadderMove", 0);
			}

			if (currentClimbHeight.y >= laddarEndHeightTop && stateInfo.IsTag("Ladder") && isClimbLadder ) {
				transform.position = new Vector3(currentClimbHeight.x, laddarEndHeightTop, currentClimbHeight.z);
			}
			if (currentClimbHeight.y <= laddarEndHeightBottom && stateInfo.IsTag("Ladder") && isClimbLadder ) {
				transform.position = new Vector3(currentClimbHeight.x, laddarEndHeightBottom, currentClimbHeight.z);
			}
			return;
		}
		// in ground.
		else if(CheckGrounded()){
			if( stateInfo.IsTag("Fall")){
				// jump parameter set to 0.
				chrAnimator.SetInteger("JumpIdx", 0);
				jumpAmount = 0;
				isBlowOff = false;
			}

			if (chrAnimator.GetInteger("JumpIdx") == 0){
				// moveDirection set 0, to prevent to move by Character controller.
				// moveDirectionはゼロにして、キャラクターコントローラがキャラクターを動かさないように。.
				// moveDirection은 0으로 돌려서, 캐릭터 컨트롤러가 캐릭터를 움직이지 않도록한다.
				moveDirection = Vector3.zero;
			}

			// press Jump button. make jump
			// if Animator parameter "JumpIdx" is 1, 
			// animator will play state of "Jump_00_start"
			// when play state of "Jump_00_up", animation event will call SetJump()
			// Jumpパラメータからアニメーションが遷移し、.
			// "Jump_00_up"のときにイベントでSetJump()ファンクションを呼ぶ。.
			// Jump파라메터를 통해 스테이트가 점프애니메이션을 재생하고,
			// "Jump_00_up"스테이트를 재생할때 SetJump()를 부른다.
			if(Input.GetButtonDown("Jump"))
				chrAnimator.SetInteger("JumpIdx", 1);

		}
		// While in Air
		else if(!CheckGrounded() && !isBlowOff){
			// fall down
			if (chrAnimator.GetInteger("JumpIdx") == 0 && !isClimbLadder) {
				chrAnimator.SetInteger ("JumpIdx", 1);
				chrAnimator.SetFloat("JumpVelocity", -10f );
				chrAnimator.SetTrigger ("Fall");
			}

			// press Jump button. can jump once more.
			if(Input.GetButtonDown("Jump") && chrAnimator.GetInteger("JumpIdx") == 1){
				chrAnimator.SetInteger("JumpIdx", 2);
			}

			// It is moved with Character Controller while in the air,
			// moveDirection is use Axis Input.
			// 空中にいるときはmoveDirectionを使って移動するので、.
			// 方向キーの入力を渡しておく。.
			// 공중에 있는 동안은 캐릭터 컨트롤러를 사용하여 이동하기때문에.
			// 방향키의 입력을 moveDirection에게 전달해준다.
			moveDirection = new Vector3(axisInput.x * moveAbilityInAir, moveDirection.y, axisInput.z * moveAbilityInAir);
			moveDirection.y -= gravity * Time.deltaTime;
			
			// JumpVelocity change the state to while in the air,
			// JumpVelocityは空中でのポーズの制御につかいます。.
			// JumpVelocity는 공중에서의 포즈를 제어합니다.
			chrAnimator.SetFloat("JumpVelocity", (moveDirection.y - (jumpAmount * 0.5f)) );

		}
		// character Blowed off
		else if(!CheckGrounded() && isBlowOff){
			moveDirection.y -= gravity * Time.deltaTime;
			
			// JumpVelocity change the state to while in the air,
			// JumpVelocityは空中でのポーズの制御につかいます。.
			// JumpVelocity는 공중에서의 포즈를 제어합니다.
			chrAnimator.SetFloat("JumpVelocity", (moveDirection.y - (jumpAmount * 0.5f)) );
		}

		// character is move by moveDirection.
		chrController.Move(moveDirection * Time.deltaTime);

	}
	
	void LateUpdate() 
	{
		// Set offset of weapon when SD Character equip axe
		if (OffsetSD) {
			// stateInfo.IsTag("AXE") is for Viewer Mode only.
			if (chrAnimator.GetBool ("WeaponState_Axe") || stateInfo.IsTag("AXE")) {
				if(weaponLeft)
					weaponLeft.localPosition = Vector3.Lerp (weaponLeft.localPosition, new Vector3 (-0.008f, 0.015f, 0.134f), Time.deltaTime * 4f);
				if(weaponRight)
					weaponRight.localPosition = Vector3.Lerp (weaponRight.localPosition, new Vector3 (0.008f, 0.015f, 0.134f), Time.deltaTime * 4f);
			}
			else {
				if(weaponLeft)
					weaponLeft.localPosition = Vector3.Lerp (weaponLeft.localPosition, Vector3.zero, Time.deltaTime * 4f);
				if(weaponRight)
					weaponRight.localPosition = Vector3.Lerp (weaponRight.localPosition, Vector3.zero, Time.deltaTime * 4f);
			}
		}
	}

	// check for ground. combine isGrounded and raycast
	public bool CheckGrounded(){
		if (chrController.isGrounded)
			return true;
		var ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
		var tolerance = 0.3f;
		// Debug.Log(Physics.Raycast(ray, tolerance));
		return Physics.Raycast(ray, tolerance);
	}


	// when pressed attack button
	// control AttackIdx parameter to play attack animation.
	void SetAttack(){
		if(chrAnimator.GetInteger("AttackIdx") == 0){
			chrAnimator.SetInteger("AttackIdx", 1);
		}
		else{
			// When pressed attack button rapidly, 
			// check state tag in this time playing,
			// use last character of state tag, to set AttackIdx value.
			for(int i = 0; i < 4; i++){
				if(stateInfo.IsTag("Combo_0" + i.ToString())){
					chrAnimator.SetInteger("AttackIdx", (i + 1));
					break;
				}
			}
		}
	}
	
	// On , Off weapon parameter
	void SetWeapon(int mode){
		if(stateInfo.IsTag("Idle")){
			int weaponCondition = 1; // select weapon animator state.
			// mode 1 for axe mode
			chrAnimator.SetBool("Talk", false );
			chrAnimator.SetBool("Stun", false );
			chrAnimator.SetBool("Dead", false );
			if(mode == 1){
				chrAnimator.SetBool("WeaponState_Axe", true);
				chrAnimator.SetBool("WeaponState_Gun", false);
			}
			// mode 2 for gun mode
			else if(mode == 2){
				chrAnimator.SetBool("WeaponState_Axe", false);
				chrAnimator.SetBool("WeaponState_Gun", true);
				weaponCondition = 0;
			}
			else{
				chrAnimator.SetBool("WeaponState_Axe", false);
				chrAnimator.SetBool("WeaponState_Gun", false);
			}
			if(weaponControl_L)
				weaponControl_L.SaftyLock(weaponCondition);
			if(weaponControl_R)
				weaponControl_R.SaftyLock(weaponCondition);
		}
	}

	// SetJump() is called from Animation event.
	// Set jumpInput. jumpInput value is used by moveDirection.y in next Update() . 
	void SetJump(){
		if (chrAnimator.GetInteger ("JumpIdx") <= 1) {
			moveDirection = new Vector3(0, jumpSpeed, 0);
			// when in ground.
			jumpAmount += jumpSpeed;
			chrAnimator.SetInteger ("JumpIdx", 1);
			chrAnimator.SetFloat("JumpVelocity", jumpAmount * 0.5f );
		}
		else if (chrAnimator.GetInteger ("JumpIdx") == 2) {
			// jump in air
			// moveDirection.y += jumpSpeed * 1.5f;
			// jumpAmount += jumpSpeed * 1.5f;
			float velo = chrAnimator.GetFloat("JumpVelocity") * -2.0f;
			if (velo <= 0) {
				velo = 0;
			}
			moveDirection.y = moveDirection.y + jumpSpeed + velo;
			jumpAmount = jumpAmount + jumpSpeed + velo;
			chrAnimator.SetInteger ("JumpIdx", 3);
		}
	}
	// SetJumpSC() is called from Animation event.
	// some animation use this to jump without input of jump button. 
	void SetJumpSC(float jumpPow){
		if (chrAnimator.GetInteger ("JumpIdx") == 0) {
			moveDirection = new Vector3(0, jumpPow, 0);
			// when in ground.
			jumpAmount += jumpPow;
			chrAnimator.SetInteger ("JumpIdx", 1);
			chrAnimator.SetFloat("JumpVelocity", jumpAmount * 0.5f );
		}
		else{
			// while in air
			moveDirection.y += jumpPow;
			jumpAmount += jumpPow;
			chrAnimator.SetInteger ("JumpIdx", 3);
		}
	}
	// SetBlowOff is called from Animation event.
	//  Character Blowed off.
	void SetBlowOff(){
		// jump parameter set to 2.
		isBlowOff = true;
		chrAnimator.SetInteger("JumpIdx", 2);
		moveDirection = transform.forward * -1 * blowOffPow;
		moveDirection.y += blowOffPow;
		chrAnimator.SetFloat("JumpVelocity", blowOffPow * 0.5f );
	}

	// ClimbLadder is called from LadderControl.
	//  Character climb up ladder.
	// public void ClimbLaddar(float laddarTop, float laddarBottom, bool isTop) {
	public void LadderState(Vector3 state) {
		isLadderTop = true;
		if (state.z <= 0.5f)
			isLadderTop = false;
		if (OffsetSD) {
			laddarEndHeightTop = state.x - 0.64f;
			laddarEndHeightBottom = state.y + 0.24f;
		}
		else {
			laddarEndHeightTop = state.x - 1.20f;
			laddarEndHeightBottom = state.y + 0.45f;
		}
	}

	public void ClimbLadder(Transform LPos) {
		if (!isClimbLadder) {
			transform.position = LPos.position;
			transform.rotation = LPos.rotation;
		}
		if (!stateInfo.IsTag("LadderStartEnd")) {
			if (isClimbLadder) {
				Vector3 currentClimbHeight = transform.position;
				isClimbLadder = false;
				if (isLadderTop == true) {
					chrAnimator.SetInteger("LadderMove", 1);
					transform.position = new Vector3(currentClimbHeight.x, laddarEndHeightTop, currentClimbHeight.z);
				}
				else if (isLadderTop == false) {
					chrAnimator.SetInteger("LadderMove", -1);
					transform.position = new Vector3(currentClimbHeight.x, laddarEndHeightBottom, currentClimbHeight.z);
				}
			}
			else {
				isClimbLadder = true;
				if (isLadderTop) {
					chrAnimator.SetInteger("LadderMove", -1);
					if (OffsetSD)
						chrController.Move(transform.forward * 0.26f);
				}
				else {
					chrAnimator.SetInteger("LadderMove", 1);
					if (OffsetSD)
						chrController.Move(transform.forward * 0.13f);
				}
			}

			chrAnimator.SetBool("ClimbLadder", isClimbLadder);
		}
	}


	// WpnPullTrigerLeft(Right) is called from Animation event.
	// this will play animation of weapon to pull trigger or return trigger.
	void WpnPullTrigerLeft(int isPull){
		if(weaponControl_L)
			weaponControl_L.PullTrigger(isPull);
	}
	void WpnPullTrigerRight(int isPull){
		if(weaponControl_R)
			weaponControl_R.PullTrigger(isPull);
	}

	// change Animator Controller.
	// this function is called from GUIControl.
	// アニメータコントローラを変更する。 .
	// GUIControlスクリプトから呼ばれる。 .
	// ビューアモード、インタラクティブモードが切り替わるときに、各モード用にアニメータコントローラを差し替える。.
	// 애니메이터 컨트롤러를 변경한다.
	// GUIControl 스크립트로부터 불려진다.
	// 뷰어모드, 인터렉티브 모드 사이를 오갈때, 각각의 모드에 맞는 애니메이터를 설정한다.
	public void ControllerChange(int idx){
		if(this.gameObject.activeSelf)
			StartCoroutine (AnimControllerChange (idx));
		else
			chrAnimator.runtimeAnimatorController = chrAnimatorController[idx];
	}
	private IEnumerator AnimControllerChange(int idx){
		// play Idle 0.1 second before change contorller
		// It is prevent error of transform
		PlayClip("cmm_idle" , 0);
		yield return new WaitForSeconds(0.1f);
		chrAnimator.runtimeAnimatorController = chrAnimatorController[idx];
		PlayClip("cmm_idle" , 0);
	}

	// index of weaponPoint, which Weapon object will follow.
	// this function is called from GUIControl or event of animation clip.
	public void AttachWeapon(int idx){
		if(weaponLeft)
			weaponLeft.SetParent(weaponPoint_L[idx], false);
		if(weaponRight)
			weaponRight.SetParent(weaponPoint_R[idx], false);
	}

	// play animation state.
	// for viewer mode
	public void PlayClip(string stateName , int weaponPoint){
		AttachWeapon(weaponPoint);
		chrAnimator.CrossFade(stateName, 0.05f);
	}

	// read 3D model information.
	// vertex count, triangles, and joint of character and weapon.
	// this function is called from GUIControl.
	public string MeshData(){
		string mdlInfo; // text.
		int[] charData = new int[3]; // vertex.
		int[] weaponData_L = new int[3]; // vertex.
		int[] weaponData_R = new int[3]; // vertex.
		int[] bagData_L = new int[3]; // vertex.
		int[] bagData_R = new int[3]; // vertex.
		int[] total = new int[3]; // vertex.

		charData = GetMeshProperty (characterMesh);
		if (weaponLeft) 
			weaponData_L = GetMeshProperty (weaponMesh_L);
		if (weaponRight) 
			weaponData_R = GetMeshProperty (weaponMesh_R);
		if (sideBagLeft) 
			bagData_L = GetMeshProperty (sideBagMesh_L);
		if (sideBagRight) 
			bagData_R = GetMeshProperty (sideBagMesh_R);
		for(int i = 0; i < total.Length; i++){
			total[i] = charData[i] + weaponData_L[i] + weaponData_R[i] + bagData_L[i] + bagData_R[i];
		}
		mdlInfo = "Character\n      Vertex : " + charData[0].ToString() + ", Tris : " + charData[1].ToString() + ", Bones : " + charData[2].ToString();
		mdlInfo += "\nWeapon x 2\n      Vertex : " + (weaponData_L[0] + weaponData_R[0]).ToString() + ", Tris : " + (weaponData_L[1] + weaponData_R[1]).ToString() + ", Bones : " + (weaponData_L[2] + weaponData_R[2]).ToString();
		mdlInfo += "\nSide Bag x 2\n      Vertex : " + (bagData_L[0] + bagData_R[0]).ToString() + ", Tris : " + (bagData_L[1] + bagData_R[1]).ToString() + ", Bones : " + (bagData_L[2] + bagData_R[2]).ToString();
		mdlInfo += "\nTotal Amount\n      Vertex : " + total[0].ToString() + ", Tris : " + total[1].ToString() + ", Bones : " + total[2].ToString();

		return mdlInfo;
	}

	// collect child of rootObject which have meshrenderer component or skinnedmeshrenderer component.
	GameObject[] CollectMeshRenderer(GameObject rootObject){
		SkinnedMeshRenderer[] skinned = rootObject.GetComponentsInChildren<SkinnedMeshRenderer> ();
		MeshRenderer[] nonSkin = rootObject.GetComponentsInChildren<MeshRenderer> ();
		GameObject[] list;
		if (skinned.Length + nonSkin.Length == 0) {
			list = new GameObject[1];
			list[0] = null;
		}
		else{
			list = new GameObject[skinned.Length + nonSkin.Length];
			for(int i = 0; i < skinned.Length; i++ ){
				list[i] = skinned[i].gameObject;
			}
			for(int i = 0; i < nonSkin.Length; i++ ){
				list[(i + skinned.Length)] = nonSkin[i].gameObject;
			}
		}

		return list;
	}

	// get vertices, triangles, bone count from gameObject list.
	int[] GetMeshProperty(GameObject[] mesh){
		int[] property = new int[3]; // 0 : vertex, 1 : triangle, 2 : joint.
		Transform[] boneList = null;
		if (mesh [0] != null) {
			for (int i = 0; i < mesh.Length; i++) {
				SkinnedMeshRenderer skinnedMesh = mesh [i].GetComponent<SkinnedMeshRenderer> ();
				// skinned model.
				if (skinnedMesh) {
					property [0] = property [0] + skinnedMesh.sharedMesh.vertices.Length;
					property [1] = property [1] + (skinnedMesh.sharedMesh.triangles.Length / 3);
					if(i == 0){
						boneList = skinnedMesh.bones;
					}
					else{
						boneList = RejectDoubledBones(boneList, skinnedMesh.bones);
					}
					property [2] = boneList.Length;
				}
				// mesh only.
				else {
					property [0] = property [0] + mesh [i].GetComponent<MeshFilter> ().sharedMesh.vertices.Length;
					property [1] = property [1] + (mesh [i].GetComponent<MeshFilter> ().sharedMesh.triangles.Length / 3);
					property [2] = property [2] + 0;
				}
			}
		}
		return property;
	}

	// compare bone list and make new bone list that does not overlapped.
	Transform[] RejectDoubledBones(Transform[] boneListA, Transform[] boneListB ){
		Transform[] newList = new Transform[boneListB.Length];
		int idx = 0;

		for (int i = 0; i < boneListB.Length; i++) {
			bool check = false;
			for (int j = 0; j < boneListA.Length; j++) {
				if (boneListB[i] == boneListA[j]) {
					check = true;
					break;
				}
			}
			if(!check){
				newList[idx] = boneListB[i];
				idx++;
			}
		}

		Transform[] returnList = new Transform[boneListA.Length + idx];
		for (int i = 0; i < boneListA.Length; i++)
			returnList[i] = boneListA[i];
		for (int i = 0; i < idx; i++)
			returnList[i + boneListA.Length] = newList[i];

		return returnList;
	}

	// Reset Camera rotation when activate fpsLike Move mode.
	// this function is called from GUIControl.
	public void SetMoveMode(bool moveModeState){
		fpsLikeMoving = moveModeState;
		if (fpsLikeMoving) {
			Camera.main.transform.root.rotation = transform.rotation;
			Camera.main.transform.root.Rotate (0, 180, 0);
		}
	}

	public void SetWeaponVisibility(bool isVisible){
		weaponLeft.gameObject.SetActive(isVisible);
		weaponRight.gameObject.SetActive(isVisible);
	}

	public void SetSideBagVisibility(bool isVisible){
		sideBagLeft.gameObject.SetActive(isVisible);
		sideBagRight.gameObject.SetActive(isVisible);
	}
}
