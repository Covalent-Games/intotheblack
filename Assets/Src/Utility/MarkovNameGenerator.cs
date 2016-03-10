using UnityEngine;
using System;
using System.Collections.Generic;

namespace Covalent.Generators {
	public class MarkovNameGenerator {

		/// <summary>
		/// The lower this number, the more random and "gibberish" the results will be. If it's too high,
		/// the results could return exact replicas from the sample set. 2 seems to be a good number. Change
		/// at your own risk.
		/// </summary>
		public int Order = 2;

		private Dictionary<string, List<string>> _markovTable = new Dictionary<string, List<string>>();
		private List<string> _vowelNameBeginnings;
		private System.Random rand = new System.Random();

		public MarkovNameGenerator(List<string> sampleSet) {

			LoadSampleData(sampleSet);
		}

		public void LoadSampleData(List<string> sampleSet) {

			string name = "";
			for (int x = 0; x < sampleSet.Count; x++) {
				name = sampleSet[x];
				for (int y = 0; y < name.Length - Order; y++) {
					List<string> table;
					if (_markovTable.TryGetValue(name.Substring(y, Order).ToLower(), out table)) {
						table.Add(name[y + Order].ToString().ToLower());
					} else {
						_markovTable.Add(name.Substring(y, Order).ToLower(),
										 new List<string>() { name[y + Order].ToString().ToLower() });
					}
				}
			}
			_vowelNameBeginnings = new List<string>();
			foreach (var key in _markovTable.Keys) {
				if (key.Contains("a") || key.Contains("e") || key.Contains("i") || key.Contains("o") || key.Contains("u")) {
					_vowelNameBeginnings.Add(key);
				}
			}
		}

		public string GenerateName(int maxLength = 12) {

			int length = rand.Next(4, maxLength);
			// Get first 2 characters
			string name = _vowelNameBeginnings[rand.Next(0, _vowelNameBeginnings.Count)];

			List<string> followingValues;
			while (--length > 0) {
				if (_markovTable.TryGetValue(name.Substring(name.Length - Order, Order), out followingValues)) {
					name += followingValues[rand.Next(0, followingValues.Count)];
				} else {
					continue;
				}
			}

			return name;
		}

		public static List<string> TempSampleData = new List<string>() {
			"Andromeda",
			"Antlia",
			"Apus",
			"Aquarius",
			"Aquila",
			"Ara",
			"Aries",
			"Auriga",
			"Bootes",
			"Caelum",
			"Camelopardalis",
			"Cancer",
			"Canes Venatici",
			"Canis Major",
			"Canis Minor",
			"Capricornus",
			"Carina",
			"Cassiopeia",
			"Centaurus",
			"Cepheus",
			"Cetus",
			"Chamaeleon",
			"Circinus",
			"Columba",
			"Coma Berenices",
			"Corona Australis",
			"Corona Borealis",
			"Corvus",
			"Crater",
			"Crux",
			"Cygnus",
			"Delphinus",
			"Dorado",
			"Draco",
			"Eridanus",
			"Fornax",
			"Gemini",
			"Grus",
			"Hercules",
			"Horologium",
			"Hydra",
			"Hydrus",
			"Indus",
			"Lacerta",
			"Leo",
			"Leo Minor",
			"Lepus",
			"Libra",
			"Lupus",
			"Lyra",
			"Mensa",
			"Microscopium",
			"Monoceros",
			"Musca",
			"Norma",
			"Octans",
			"Ophiuchus",
			"Orion",
			"Pavo",
			"Pegasus",
			"Perseus",
			"Phoenix",
			"Pictor",
			"Pisces",
			"Piscis Austrinus",
			"Puppis",
			"Pyxis",
			"Reticulum",
			"Sagitta",
			"Sagittarius",
			"Scorpius",
			"Sculptor",
			"Scutum",
			"Serpens",
			"Sextans",
			"Taurus",
			"Telescopium",
			"Triangulum",
			"Triangulum Australe",
			"Tucana",
			"Ursa Major",
			"Ursa Minor",
			"Vela",
			"Virgo",
			"Volans",
			"Vulpecula",

			"Auriga",
			"Alphecca",
			"Mira",
			"Menkab",
			"Nusakan",
			"Kraz",
			"Minkar",
			"Garnet Star",
			"Kuma",
			"Rana",
			"Vega",
			"Yed Prior",
			"Yed Posterior",
			"Bellatrix",
			"Betelgeuse",
			"Thabit",
			"Peacock",

			"Apollo",
			"Alastor",
			"Achelois",
			"Aether",
			"Amphitrite",
			"Aphrodite",
			"Artemis",
			"Astraea",
			"Athena",
			"Boreas",
			"Calypso",
			"Kalypso",
			"Castor",
			"Cerus",
			"Ceto",
			"Chaos",
			"Chronos",
			"Enyo",
			"Eos",
			"Eris",
			"Gaia",
			"Hermes",
			"Hypnos",
			"Iris",
			"Kratos",
			"Maia",
			"Mania",
			"Oceanus",
			"Pan",
			"Phosphorus",
			"Poseidon",
			"Proteus",
			"Selene",
			"Sterope",
			"Rhea",
			"Typhon",
			"Tyche",
			"Triton",
			"Thetis",
			"Tartarus",
			"Urania",
			"Uranis",
			"Zephyrus",
		};

	}
}
