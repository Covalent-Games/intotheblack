using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Data container for anything that doesn't fall under the player category or star system category.
/// </summary>
[Serializable]
public class GameStateData {

	public static Guid SelectedSystem;

	public void Save() {

		DataSerializer.SerializeData(this, FilePaths.GameStatePath);
	}

	public static GameStateData Load() {

		return DataSerializer.DeserializeData<GameStateData>(FilePaths.GameStatePath);
	}
}
