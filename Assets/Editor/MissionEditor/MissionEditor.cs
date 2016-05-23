using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MissionEditor : EditorWindow {

	private List<Rect> _windows = new List<Rect>();

	//[MenuItem("Missions/Mission Editor")]
	static void ShowEditor() {

		MissionEditor editor = EditorWindow.GetWindow<MissionEditor>();
		editor.minSize = new Vector2(1000f, 600f);
	}

	private void OnGUI(){

		if (GUI.Button(new Rect(10, 10, 120, 40), "Add Node")) {
			Debug.Log("Click");
		}

		BeginWindows();
		Rect windowRect = new Rect(10, 60, 100, 100);
		Rect window = GUI.Window(0, windowRect, DragNode, "Node " + 0);
		EndWindows();

	}

	private void DragNode(int id) {
		GUI.DragWindow();
	}

}
