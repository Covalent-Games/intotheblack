public class Player {

	public PlayerData PlayerData;
	public Destructable PlayerShip;

	public PlayerData GenerateNewPlayerData() {

		PlayerData newPlayer = new PlayerData();
		newPlayer.ResetPlayerData();

		return newPlayer;
	}

}
