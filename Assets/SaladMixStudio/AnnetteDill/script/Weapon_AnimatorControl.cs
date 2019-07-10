using UnityEngine;
using System.Collections;

/*
    Weapon_AnimatorControl is script to control the Weapon Animator in demoscene.

	Weapon_AnimatorControlはデモシーンに配置されたキャラクターの武器を制御するスクリプトです。.

    Weapon_AnimatorControl은 데모신에 배치된 캐릭터의 무기를 제어하기위한 스크립트 입니다.

	2015.08.01
*/

public class Weapon_AnimatorControl : MonoBehaviour {

	public Animator wpnAnimator;

	//　for play effect
	// public Transform bladeTrail; // trail of blade.
	public Transform muzzlePoint; // location of muzzle effect.
	public GameObject muzzleEffect; // muzzle flash effect.


	
	// Weapon object Play the animation to pull the trigger. (pull == true)
	// or return trigger. (pull == false)
	// this function is called from event of animation clip.
	public void PullTrigger(int pull){
		bool isPull = (pull != 0);
		if (wpnAnimator) {
			wpnAnimator.SetBool ("PullTrigger", isPull);
			// In playing [pullTrigger_hammerNotSetted] state, we need to rotate Cylinder.
			if(isPull && !wpnAnimator.GetBool ("HammerSet"))
				wpnAnimator.SetTrigger ("CylinderRotate");
			wpnAnimator.SetBool ("HammerSet", false);
		}
		// if pull trigger is true, wait 5 frame, and make fire.
		if (isPull) {
			StartCoroutine( SetFire() );
		}
	}

	// play muzzle flash effect.
	private IEnumerator SetFire(){
		yield return new WaitForSeconds(0.16f);
		Instantiate (muzzleEffect, muzzlePoint.position, muzzlePoint.rotation);
	}

	// Lock or Unlock Safty of Weapon
	public void SaftyLock(int lockState){
		bool isLock = (lockState != 0);
		if(wpnAnimator)
			wpnAnimator.SetBool("SaftyLock", isLock);
	}

	// Pull hammer for prepare to fire.
	public void SetHammer(int hammerState){
		bool isHammerSet = (hammerState != 0);
		if (wpnAnimator) {
			wpnAnimator.SetBool ("HammerSet", isHammerSet);
			// In playing [hammer_set] state, we need to rotate Cylinder.
			wpnAnimator.SetTrigger ("CylinderRotate");
		}
	}

}
