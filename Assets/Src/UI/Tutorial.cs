using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

	public Text TutorialText;

	void Start () {

		Time.timeScale = 0f;
		if (!PlayerPrefs.HasKey("tutorial_complete")) {
			PlayerPrefs.SetInt("tutorial_complete", 0);
			PlayerPrefs.Save();
		}
		if (PlayerPrefs.GetInt("tutorial_complete") == 0) {
			TutorialText.enabled = true;
		} else {
			Time.timeScale = 1f;
		}

	}

	void Update() {

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Time.timeScale = 1f;
			TutorialText.enabled = false;
			MarkTutorialComplete();
			Destroy(gameObject);
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			TutorialText.enabled = false;
			MarkTutorialComplete();
			Destroy(gameObject);
		}
	}

	private void MarkTutorialComplete() {

		PlayerPrefs.SetInt("tutorial_complete", 1);
		PlayerPrefs.Save();
	}
}
