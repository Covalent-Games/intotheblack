using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {

	public Vector3 EndLocation;
	public ParticleSystem BeaconLight;
	public GameManager Manager;
	public OverlayUI GameUI;

	private void OnTriggerEnter2D(Collider2D collider) {

		// If objective is not at EndLocation, put it there. Otherwise the objective has been
		// reached and we can disabled the objective.
		if (collider.CompareTag("Player")) {
			//TODO: Display a message telling the player they got it.
			if (transform.position != EndLocation) {
				transform.position = EndLocation;
				BeaconLight.startColor = Color.green;
				BeaconLight.Clear(true);
				GameUI.DisplayMessage("Nice! Now get it to the drop zone!");
			} else {
				//TODO: That same message...
				gameObject.SetActive(false);
				Manager.ObjectivesCollected++;
				Manager.ObjectiveDistance += 25;
				GameUI.UpdateObjectiveText(Manager.ObjectivesCollected);
				GameUI.UpdateObjectiveText(Manager.ObjectivesCollected);
				GameManager.PlayerExperience += 65;
				GameUI.DisplayMessage("Well done! Await the next beacon's location.");
			}
			AudioSource.PlayClipAtPoint(SoundHolder.Instance.SuccessTone, Camera.main.transform.position);
		}
	}
}
