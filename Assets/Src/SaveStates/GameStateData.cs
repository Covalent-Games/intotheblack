using UnityEngine;
using System.Collections;
using System;
using Covalent.Data;


/// <summary>
/// Data container for anything that doesn't fall under the player category or star system category.
/// Access the data via GameStateData.State.
/// </summary>
[Serializable]
public class GameStateData {

	// Add values to save between these lines.
	// ----------------------------------
	public Guid PlayerOccupiedSystem;
	public int TurnCount;

	// ----------------------------------

	/// <summary>
	/// The GameStateData object that is saved to disk
	/// </summary>
	private static GameStateData _state;
	/// <summary>
	/// The GameStateData static property used to globally access the game state data.
	/// </summary>
	public static GameStateData State {
		get {
			if (_state == null) {
				_state = new GameStateData();
			}
			return _state;
		}
		set {
			_state = value;
		}
	}

	public static void Save() {

		DataSerializer.SerializeData(_state, FilePaths.GameStatePath);
	}

	public static GameStateData Load() {

		_state = DataSerializer.DeserializeData<GameStateData>(FilePaths.GameStatePath);
		return _state;
	}
}
