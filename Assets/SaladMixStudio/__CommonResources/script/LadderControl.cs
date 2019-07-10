using UnityEngine;

public class LadderControl : MonoBehaviour {

	public Transform LadderPosTop;
	public Transform LadderPosBottom;
	public bool isLadderTop;
	private Transform StartPos;
	private Vector3 LadderState;

	// Use this for initialization
	void Awake () {
		if (isLadderTop) {
			StartPos = LadderPosTop;
		} 
		else {
			StartPos = LadderPosBottom;
		}
		float topState = 0f;
		if (isLadderTop)
			topState = 1.0f;
		LadderState = new Vector3(LadderPosTop.position.y, LadderPosBottom.position.y, topState);
	}

	void OnTriggerStay(Collider other){
		if (Input.GetButtonUp ("Fire1") && !Input.GetMouseButtonUp (0)) {
			other.gameObject.SendMessage("LadderState", LadderState);
			other.gameObject.SendMessage("ClimbLadder", StartPos);
		}
	}
}
