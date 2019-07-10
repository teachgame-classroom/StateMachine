using UnityEngine;
using System;

public class OutfitGroupData : MonoBehaviour{

	public OutfitGrp[] outfitGrp;

	[Serializable]
	public class OutfitGrp
	{
		public string outfitName;
		public ObjGrp[] chrList;
	}

	[Serializable]
	public class ObjGrp
	{
		public string chrName;
		public GameObject chrModel;

	}

	public GameObject[] GetChrModelList(int idx){
		int chrLength = outfitGrp [idx].chrList.Length;
		#if UNITY_WEBPLAYER || UNITY_WEBGL
			chrLength--; // Do not use Color Edit Model when build for Web
		#endif

		GameObject[] list = new GameObject[chrLength];
		for(int i = 0; i < chrLength; i++){
			list [i] = outfitGrp [idx].chrList [i].chrModel;
		}
		return list;
	}	

	public string[] GetChrNameList(int idx){
		int chrLength = outfitGrp [idx].chrList.Length;
		#if UNITY_WEBPLAYER || UNITY_WEBGL
			chrLength--;
		#endif

		string[] list = new string[chrLength];
		for(int i = 0; i < chrLength; i++){
			list [i] = outfitGrp [idx].chrList [i].chrName;
		}
		return list;
	}
}

