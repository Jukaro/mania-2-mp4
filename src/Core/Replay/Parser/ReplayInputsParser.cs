using System;
using System.IO;
using System.Linq;

namespace Rythmify.Core.Replay;

public static partial class ReplayParser {

	private static MemoryStream DecompressFileLZMA(byte[] buffer) {
		SevenZip.Compression.LZMA.Decoder coder = new();
		MemoryStream input = new(buffer);
		MemoryStream output = new();

		// Read the decoder properties
		byte[] properties = new byte[5];
		input.Read(properties, 0, 5);

		// Read in the decompress file size.
		byte [] fileLengthBytes = new byte[8];
		input.Read(fileLengthBytes, 0, 8);
		long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

		coder.SetDecoderProperties(properties);
		coder.Code(input, output, input.Length, fileLength, null);
		output.Flush();
		output.Seek(0, SeekOrigin.Begin);
		return output;
	}

	private static int GetStartDelay(string[] inputsArray) {
		if (inputsArray.Length < 2)
			return -1;

		string[] skipTimeSplittedInputs = inputsArray[1].Split('|');
		if (skipTimeSplittedInputs.Length != 4)
			throw new ArgumentException("Invalid input format: " + inputsArray[1]);

		string[] startDelaySplittedInputs = inputsArray[2].Split('|');
		if (startDelaySplittedInputs.Length != 4)
			throw new ArgumentException("Invalid input format: " + inputsArray[2]);

		int parsedSkipTime = int.Parse(skipTimeSplittedInputs[0]);
		int skipTime = parsedSkipTime > 0 ? parsedSkipTime : -1;

		int parsedStartDelayTime = int.Parse(startDelaySplittedInputs[0]);

		if (skipTime == -1) return -parsedStartDelayTime;
		return -(skipTime + parsedStartDelayTime);
	}

	private static int SkipNonGameplayInputs(string[] inputsArray) {
		int to_skip = 0;

		for (; to_skip < inputsArray.Length; to_skip++) {
			string[] splitted = inputsArray[to_skip].Split('|');
			if (int.Parse(splitted[1]) != 256)
				break;

			// Logger.LogDebug("Skipping non-initialized input: " + inputsArray[to_skip]);
		}

		for (; to_skip < inputsArray.Length; to_skip++) {
			string[] splitted = inputsArray[to_skip].Split('|');
			if (int.Parse(splitted[0]) >= 0)
				break;

			// Logger.LogDebug("Skipping non-initialized input: " + inputsArray[to_skip]);
		}

		return to_skip;
	}

	private static string[] parseInputs(byte[] bytes, int length, ref int index, ref ReplayData replay) {
		byte[] compressedReplayLZMA = new byte[length];
		Buffer.BlockCopy(bytes, index, compressedReplayLZMA, 0, length);

		replay.CompressedInputs = compressedReplayLZMA;

		if (length == 0)
			return null;

		MemoryStream inputs = DecompressFileLZMA(compressedReplayLZMA);

		byte[] inputsBytes = new byte[inputs.Length];
		inputs.Read(inputsBytes);
		string str = System.Text.Encoding.UTF8.GetString(inputsBytes, 0, inputsBytes.Length);
		string[] inputsArray = str.Split(',');

		replay.StartDelay = GetStartDelay(inputsArray);

		int to_skip = SkipNonGameplayInputs(inputsArray);

		string[] splittedInput;
		splittedInput = inputsArray[to_skip].Split('|');

		int holdTime = int.Parse(splittedInput[0]); // 1st input holdTime
		int timeStamp = 0;
		int input;
		int max_input = 0;

		// input counter
		int inputChangeCounter = 0;
		int currentNbKeys;
		int lastNbKeys = int.MaxValue;
		int lastInput = int.MaxValue;

		for (int i = to_skip ; i < inputsArray.Length - 2; i++) {
			splittedInput = inputsArray[i].Split('|');
			input = int.Parse(splittedInput[1]);
			while (int.Parse(splittedInput[1]) == input && i < inputsArray.Length - 3) {
				i++;
				splittedInput = inputsArray[i].Split('|');
				holdTime += int.Parse(splittedInput[0]);
			}
			if (i < inputsArray.Length - 3)
				i--;
			timeStamp += holdTime;
			if (input > max_input)
				max_input = input;
			replay.Inputs.Add(new Input(timeStamp, input, holdTime));

			// input counter
			currentNbKeys = replay.Inputs.Last().GetNbKeys();
			if (currentNbKeys > lastNbKeys)
				inputChangeCounter += currentNbKeys - lastNbKeys;
			else if (currentNbKeys == lastNbKeys && input != lastInput)
				inputChangeCounter += currentNbKeys;
			lastNbKeys = currentNbKeys;
			lastInput = input;
			holdTime = 0;
		}

		replay.TotalKeyPresses = inputChangeCounter;
		index += length;

		return inputsArray;
	}

}
