namespace Rythmify.Core.Replay;
public class Input {
	public int Timestamp;
	public int Keys;
	public int HoldTime;

	public Input (int timestamp, int keys, int hold) {
		Timestamp = timestamp;
		Keys = keys;
		HoldTime = hold;
	}

	public int GetNbKeys() {
		int keyID = 1;
		int nbKeys = 0;

		for (int i = 0; i < 10; i++) {
			if ((Keys & keyID) != 0)
				nbKeys++;
			keyID *= 2;
		}
		return nbKeys;
	}

	public override string ToString() {
		return $"Keys: {Keys}, Timestamp: {Timestamp}, HoldTime: {HoldTime}";
	}

	public string ToString(int nbKeys) {
		string keysStr = "";
		int keyID = 1;

		for (int i = 0; i < nbKeys; i++) {
			keysStr += (Keys & keyID) == 0 ? "| " : "|X";
			keyID <<= 1;
		}
		keysStr += '|';

		return $"Keys: {keysStr}, Timestamp: {Timestamp}, HoldTime: {HoldTime}";
	}
}
