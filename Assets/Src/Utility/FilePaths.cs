using UnityEngine;
using System.Collections;

public class FilePaths {

	//TODO: These should be given generic names and encryption to prevent tampering.
	internal static string StarMapPath { get { return Application.persistentDataPath + "/starmap.dat"; } }
	internal static string GameStatePath { get { return Application.persistentDataPath + "/gamestate.dat"; } }
	internal static string PlayerStatePath { get { return Application.persistentDataPath + "/playerstate.dat"; } }
}
 