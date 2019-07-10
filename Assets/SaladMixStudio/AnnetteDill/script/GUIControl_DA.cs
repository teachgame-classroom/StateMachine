using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/*
    GUIControl_DA is script to display and control UI elements on the screen.

    GUIControl_DAはデモシーンの画面上に各UI要素やボタンなどを配置し、.
    ユーザーの入力によりコントロールするスクリプトです。.

    GUIControl_DA은 화면에 각종 UI요소를　배치하고,
    플레이어의 입력에 대응하여 컨트롤하기위한 스크립트입니다.
	
	2017.09.02
*/

public class GUIControl_DA : MonoBehaviour {

	// required Object or information
	// 必要なオブジェクトや、情報。.
	// 필요한 오브젝트 혹은 정보.
	public GameObject[] chrModel; // Characte prehab.
	private string[] chrName;
	private OutfitGroupData outfitGrpData;
	private DA_AnimatorControl chrCtrl; // Characte prehab.
	private int activeOutfixGrpIdx = 0; // Index of active outfit group.
	private int activeLodIdx = 0; // Index of active object in chrModel.
	public GameObject sampleLadder; // sample ladder Object For Viewr Mode.
	public Transform sampleLadderTopPos; // sample ladder Object For Viewr Mode.
	public Transform sampleLadderBottomPos; // sample ladder Object For Viewr Mode.
	public GameObject sampleLadderIM; // sample ladder Object For Viewr Mode.



	// txt file used to property of UI element.
	public TextAsset stateList;
	public TextAsset IMInfo;
	private string[] stateName; // state name in animator
	private int[] weaponPos; // 0 : animation which don't  need to equip weapon.  1 : animation which need to equip weapon.
	private string[] stateLabelName; // state name in UI
	public GameObject[] lightObj; // light object.
	
	// GUI Object
	public GameObject AnimationSelectUI;
	public GameObject InteractiveModeUI;
	public GameObject ModelInformationUI;
	public Text modelChangeButtonlabel;
	private GameObject[] ASelectBtn = new GameObject[10];
	private Text[] ASelectLabel = new Text[10];
	private Text ASelectPage;
	private Text IMText;
	public Animator FadePanel;

	private bool viewerMode = true; // is playing viewer mode or interactive mode?
	private string meshInfoMsg; // information of 3D model object, such as number of polygons,  number of joint.
	private string[] iModeMsg = new string[3]; // Information text on the left side of the screen, in Interactive mode.

	private GameObject noticeWindow;

	void Start () {
		outfitGrpData = GetComponent<OutfitGroupData>();
		chrModel = outfitGrpData.GetChrModelList (0);
		chrName = outfitGrpData.GetChrNameList (0);

		// Target to control of animator control script.
		chrCtrl = chrModel[activeLodIdx].GetComponent<DA_AnimatorControl>();
		// set hold position of weapon to spine.
		chrCtrl.AttachWeapon(0);

		// set 3D model information.
		// meshInfoMsg = chrCtrl.MeshData();
		ModelInformationUI.GetComponentInChildren<Text>().text = outfitGrpData.outfitGrp[0].outfitName + " " + chrName[0]
			+ "\n\n" + chrCtrl.MeshData();

		TextReaderState ();
		GameObject ASelectGrid = GameObject.Find ("Window_AnimationSelect/gridLayout");
		for (int i = 0; i < ASelectBtn.Length; i++) {
			ASelectBtn[i] = ASelectGrid.transform.GetChild (i).gameObject;
			ASelectLabel[i] = ASelectBtn[i].GetComponentInChildren<Text>();
		}
		ASelectPage = GameObject.Find ("Window_AnimationSelect/pageNumber").GetComponentInChildren<Text>();

		TextReaderIMinfo ();

		MotionControlBtn (1);

		ChangeAnimator (0);

		sampleLadder.SetActive (false);
		sampleLadderIM.SetActive (false);

		noticeWindow = GameObject.Find("Canvas_MainUI/Window_Notice");
		#if UNITY_WEBPLAYER || UNITY_WEBGL
		noticeWindow.SetActive(true);
		#else
		noticeWindow.SetActive(false);
		#endif
	}

	// Toggle On/off UI group.
	public void ToggleUIVisibility (GameObject uiGrp) {
		uiGrp.SetActive (!uiGrp.activeSelf);
	}


	// Button for exchange Game mode.
	// モード変更ボタン.
	// ボタンを押すたびに、キャラクターアニメーターが切り替わります。.
	// 게임 모드를 변경.
	// 버튼을 누를때 마다 게임모드와 캐릭터 애니메이터를 전환 합니다.
	public void ModeChangeBtn (Text modeChangelabel) {
		if (viewerMode == true){
			viewerMode = false;
			modeChangelabel.text = "Change to\nViewer Mode";
			AnimationSelectUI.SetActive(false);
			InteractiveModeUI.SetActive(true);
			if(IMText == null){
				IMText = GameObject.Find ("Window_InteractiveMode/InformationText").GetComponent<Text>();
				IMText.text = iModeMsg [0];
			}
			ChangeAnimator (1);
			sampleLadder.SetActive (false);
		}
		else if (viewerMode == false){
			viewerMode = true;	
			modeChangelabel.text = "Change to\nInteractive Mode";
			AnimationSelectUI.SetActive(true);
			InteractiveModeUI.gameObject.GetComponentInChildren<Toggle>().isOn =false;
			InteractiveModeUI.SetActive(false);
			ChangeAnimator (0);
			chrCtrl.SetMoveMode(false);
			sampleLadderIM.SetActive (false);
		}
	}
	
	// animation play button, page in viewer mode.
	// ビューアモード時のアニメーションを再生するボタン、左右のページ送り、現在のページ表記のUIを表示します。.
	// 뷰어모드에서 사용하는 개별 애니메이션 버튼, 좌우로 페이지를 넘기는 버튼, 현재 페이지 수를 표시합니다.
	public void MotionControlBtn(int currentPage) {
		int maxBtn = 10; // The maximum number of buttons.
		int maxPage = Mathf.CeilToInt((float)stateName.Length / (float)maxBtn); // The maximum number of pages.
		if(currentPage == 0)
			currentPage = maxPage;
		else if(currentPage > maxPage)
			currentPage = 1;

		ASelectPage.name = currentPage.ToString();
		ASelectPage.text = currentPage.ToString("D3") + " / " + maxPage.ToString("D3");

		// Animation play buttons.
		// Name of the button is display state name.
		// gameObject name of the button is set to Index of state number.
		// 以下のルーフ分では、アニメーション再生ボタンを並べます。.
		// ボタンの名前には、ステート名を当てます。.
		// ボタンのゲームオブジェクト名は、ステートの番号を割り当てます。.
		// 개별 애니메이션 버튼을 나열한다.
		// 버튼의 이름은 스테이트 이름을 그대로 표시,.
		// 버튼의 게임오브젝트명은 스테이트의 번호를 입력한다.
		for (int i = 0; i < maxBtn; i++) {
			int stateNum = (currentPage - 1) * maxBtn + i;
			if(stateNum >= stateName.Length){
				ASelectBtn[i].SetActive(false);
			}
			else{
				ASelectBtn[i].SetActive(true);
				ASelectBtn[i].name = stateNum.ToString();
				ASelectLabel[i].text = stateLabelName[stateNum];
			}
		}
	}

	// Name of gameobject as number, play state of corresponding number.
	public void PlayClipBtn (GameObject self) {
		int idx = int.Parse(self.name);
		// ladder Bottom
		if (weaponPos [idx] == 2) {
			sampleLadder.SetActive (true);
			chrCtrl.gameObject.transform.position = sampleLadderBottomPos.position;
			chrCtrl.gameObject.transform.rotation = sampleLadderBottomPos.rotation;
			chrCtrl.PlayClip (stateName [idx], 0);
			Debug.Log (stateName [idx]);
		} 
		// ladder Middle1
		else if (weaponPos [idx] == 3) {
			sampleLadder.SetActive (true);
			Vector3 LadderMiddleBottomPos = new Vector3 (sampleLadderBottomPos.position.x, (sampleLadderBottomPos.position.y + 0.45f), sampleLadderBottomPos.position.z);
			chrCtrl.gameObject.transform.position = LadderMiddleBottomPos;
			chrCtrl.gameObject.transform.rotation = sampleLadderBottomPos.rotation;
			chrCtrl.PlayClip (stateName [idx], 0);
			Debug.Log (stateName [idx]);
		} 
		// ladder Middle2
		else if (weaponPos [idx] == 4) {
			sampleLadder.SetActive (true);
			Vector3 LadderMiddleTopPos = new Vector3 (sampleLadderBottomPos.position.x, (sampleLadderTopPos.position.y- 1.20f), sampleLadderBottomPos.position.z);
			chrCtrl.gameObject.transform.position = LadderMiddleTopPos;
			chrCtrl.gameObject.transform.rotation = sampleLadderBottomPos.rotation;
			chrCtrl.PlayClip (stateName [idx], 0);
			Debug.Log (stateName [idx]);
		} 
		// ladder Top
		else if (weaponPos [idx] == 5) {
			sampleLadder.SetActive (true);
			chrCtrl.gameObject.transform.position = sampleLadderTopPos.position;
			chrCtrl.gameObject.transform.rotation = sampleLadderTopPos.rotation;
			chrCtrl.PlayClip (stateName [idx], 0);
			Debug.Log (stateName [idx]);
		} 
		else {
			sampleLadder.SetActive (false);
			chrCtrl.PlayClip(stateName[idx], weaponPos[idx]);
		}
	}

	// page turn left or right.
	public void TurnPage (bool isNextPage) {
		int pageIdx = int.Parse(ASelectPage.name);
		if(isNextPage)
			pageIdx++;
		else
			pageIdx--;
		MotionControlBtn (pageIdx);
	}

	// Button for camera control.
	// カメラのズームの切り替え、回転を許容するかどうかを制御するボタンを作成.
	// 카메라 회전, 줌을 제어하는 버튼을 만든다.
	public void CameraZoomControlBtn () {
		gameObject.GetComponent<CamControl>().CamZoom();
	}
	public void CameraRotationControlBtn (Toggle tgle) {
		gameObject.GetComponent<CamControl>().RotateOption(tgle.isOn);
	}

	// Button for light object.
	// 照明をコントロールするボタンを作成。.
	// 조명의 온, 오프를 변경하는 버튼.
	public void BackLightControlBtn (Toggle tgle) {
		foreach(GameObject light in lightObj)
			light.SetActive(tgle.isOn);
	}

	// Toggle Button for Fpslike move.
	public void MoveModeControlBtn (Toggle tgle) {
		chrCtrl.SetMoveMode(tgle.isOn);
	}
	// Toggle Visibility of Laddar Group in Interactive Mode.
	public void ToggleLadderGroupVisibility (Toggle tgle) {
		sampleLadderIM.SetActive (tgle.isOn);
		if (tgle.isOn) {
			chrModel [activeLodIdx].transform.position = Vector3.zero;
		}
	}



	// Toggle On/off Weapon.
	public void ToggleWeaponVisibility (Toggle tgle) {
		chrCtrl.SetWeaponVisibility(tgle.isOn);
	}
	// Toggle On/off SideItem.
	public void ToggleSideBaVisibility (Toggle tgle) {
		chrCtrl.SetSideBagVisibility(tgle.isOn);
	}


	// Button for exchange character.
	// use ChangeLOD().
	public void OutfitGroupControlBtn (Text label) {
		GameObject currentActiveChr = chrModel [activeLodIdx];

		// disable all character model;
		for(int i = 0; i < chrModel.Length; i++)
			chrModel[i].SetActive(false);


		activeOutfixGrpIdx++;
		if(activeOutfixGrpIdx == outfitGrpData.outfitGrp.Length)
			activeOutfixGrpIdx = 0;

		// change list.
		chrModel = outfitGrpData.GetChrModelList (activeOutfixGrpIdx);
		chrName = outfitGrpData.GetChrNameList (activeOutfixGrpIdx);

		// Set Animator Controller
		int animatorIdx = 0;
		if(!viewerMode)
			animatorIdx = 1;
		ChangeAnimator (animatorIdx);

		activeLodIdx--;
		if (activeLodIdx < 0)
			activeLodIdx = chrModel.Length - 1;
		else if (activeLodIdx >= chrModel.Length)
			activeLodIdx = chrModel.Length - 1;
		int aIdx = activeLodIdx;
		aIdx++;
		if(aIdx == chrModel.Length)
			aIdx = 0;

		// change Button label.
		label.text = "Change Outfit\n" + outfitGrpData.outfitGrp [activeOutfixGrpIdx].outfitName;
		modelChangeButtonlabel.text = "Model Change\n" + chrName[aIdx];

		string labelName = outfitGrpData.outfitGrp [activeOutfixGrpIdx].outfitName + " " + chrName [aIdx];
		StartCoroutine( ChangeLOD(labelName, currentActiveChr) );
	}

	// Button for exchange character.
	// use ChangeLOD().
	public void LODControlBtn () {
		GameObject currentActiveChr = chrModel [activeLodIdx];
		int aIdx = activeLodIdx;
		aIdx++;
		if(aIdx == chrModel.Length)
			aIdx = 0;

		// change Buttonlabel.
		modelChangeButtonlabel.text = "Model Change\n" + chrName[aIdx];


		string labelName = outfitGrpData.outfitGrp [activeOutfixGrpIdx].outfitName + " " + chrName [aIdx];

		StartCoroutine( ChangeLOD(labelName, currentActiveChr) );
	}
	
	// change character model.
	private IEnumerator ChangeLOD (string modelName, GameObject currentActiveChr) {
		activeLodIdx++;
		if(activeLodIdx == chrModel.Length)
			activeLodIdx = 0;

		// play Idle 0.1 second before change character model.
		// It is prevent error of transform like weapon_point_hand
		// 0.1秒、IDLEを再生してからモデルを切り替える。.
		// 현재 표시 중인 모델에게 IDLE모션을 0.초간 재생시킨 후 처리를 시작한다.
		chrCtrl.PlayClip(stateName[0], weaponPos[0]);
		yield return new WaitForSeconds(0.1f);

		// disable ladder
		sampleLadder.SetActive (false);

		// disable all other character model.
		// 表示されるIdxではないキャラクターモデルは非表示にする。.
		// 표시되어야하는 Idx이외의 캐릭터 모델은 꺼준다.
		for(int i = 0; i < chrModel.Length; i++){
			if(i != activeLodIdx)
				chrModel[i].SetActive(false);
		}

		// Active new chacter model and replace animator control script.
		// 次のキャラクターを表示し、アニメーター制御スクリプトを交換する。.
		// 현재 Idx의 캐릭터를 표시한 후 애니메이터 스크립트를 교환해준다.
		chrModel[activeLodIdx].SetActive(true);
		chrCtrl = chrModel[activeLodIdx].GetComponent<DA_AnimatorControl>();

		// to display same place.
		chrModel[activeLodIdx].transform.position = currentActiveChr.transform.position;
		chrModel[activeLodIdx].transform.rotation = currentActiveChr.transform.rotation;

		// play Idle.
		chrCtrl.PlayClip(stateName[0], weaponPos[0]);

		// set 3D model infomation newly.
		// meshInfoMsg = chrCtrl.MeshData();
		ModelInformationUI.GetComponentInChildren<Text>().text = modelName + "\n\n" + chrCtrl.MeshData();
		// replace target of camera.
		gameObject.GetComponent<CamControl>().target = chrModel[activeLodIdx].transform;
	}
	
	// replace animator controller.
	// when start game, or change game mode.
	// キャラクターアニメーターを切り替える処理.
	// 開始時、モードに切り替え時に呼ばれる。 .
	// 캐릭터 애니메이터를 교체하는 명령을 내린다.
	// 게임을 개시할 때, 게임모드를 변경할때 불려진다.
	void ChangeAnimator (int idx) {
		for(var i = 0; i < chrModel.Length; i++){
			chrModel[i].GetComponent<DA_AnimatorControl>().ControllerChange(idx);
		}
	}

	// Get system language to select UI lanuage.
	// OSの言語設定を確認して、UIの言語を変える。.
	// OS의 언어 설정을 취득하여, 버튼등에 쓰이는 언어를 변경한다.
	int GetSystemLanguage(){
		int labelIdx;
		if (Application.systemLanguage == SystemLanguage.Japanese)
			labelIdx = 1;
		else if (Application.systemLanguage == SystemLanguage.Korean)
			labelIdx = 2;
		else
			labelIdx = 0;
        #if UNITY_WEBPLAYER || UNITY_WEBGL
        labelIdx = 0;
        #endif
        return labelIdx;
    }

    // Viewer mode Button labels and Weapon position select index.
    // will load from txt file.
    void TextReaderState() {
		string[] stateProp = stateList.text.Split ('\n');
		stateName = new string[stateProp.Length]; // state name in animator
		weaponPos = new int[stateProp.Length]; //
		stateLabelName = new string[stateProp.Length]; // state name in UI
		
		for(int i = 0; i < stateProp.Length; i++){
			string[] property = stateProp[i].Split(',');
			stateName[i] = property[0];
			weaponPos[i] = int.Parse(property[1]);
			stateLabelName[i] = property[ (2 + GetSystemLanguage()) ];
		}
	}

	// Description of interactive mode.
	// will load from txt file.
	void TextReaderIMinfo() {
		string[] infoProp = IMInfo.text.Split ('/');
		iModeMsg[0] = infoProp[ GetSystemLanguage() ];
	}	

	public void LoadScene(int idx) {
		#if UNITY_WEBPLAYER || UNITY_WEBGL
			noticeWindow.SetActive(true);
		#else
			FadePanel.SetTrigger ("Fade");
			StartCoroutine( LoadSceneAfterFade(idx) );
		#endif
	}
	private IEnumerator LoadSceneAfterFade(int idx) {
		bool isFadeEnd = false;
		yield return new WaitForSeconds(0.01f);
		while (!isFadeEnd) {
			yield return null;
			if (FadePanel.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1f) {
				isFadeEnd = true;
			}
		}
		string sceneName = "DemoScene";
		if (idx == 1) {
			sceneName = "ColorEdit_DemoScene";
		}
		else if (idx == 2) {
			sceneName = "ColorEdit_DemoScene_00";
		}
		SceneManager.LoadScene (sceneName);
	}
}