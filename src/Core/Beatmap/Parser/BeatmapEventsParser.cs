using System;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	static readonly Dictionary<EventType, Action<List<BeatmapEvent>, string[]>> eventToParser = new() {
		{EventType.Background, (events, parameters) => { events.Add(ParseBackgroundEvent(parameters)); } },
		{EventType.Video, (events, parameters) => { events.Add(ParseVideoEvent(parameters)); } },
		{EventType.Break, (events, parameters) => { events.Add(ParseBreakEvent(parameters)); } },
	};

	private static BeatmapEvent[] ParseEventsSection(string[] lines) {
		List<BeatmapEvent> events = new();

		foreach (string line in lines) {
			var parameters = Array.ConvertAll(line.Split(','), (string s) => s.Trim());
			var eventType = BeatmapEvent.GetEventType(parameters[0]);

			if (eventToParser.TryGetValue(eventType, out Action<List<BeatmapEvent>, string[]> parser))
				parser(events, parameters);
			else
				Logger.LogWarning($"Could not find parser for event type {eventType}");
		}

		return events.ToArray();
	}

	private static BackgroundEvent ParseBackgroundEvent(string[] parameters) {
		BackgroundEvent backgroundEvent = new();

		if (parameters.Length != 5)
			throw new ArgumentException($"Background event must have 5 parameters, but got {parameters.Length}");

		backgroundEvent.StartTime = int.Parse(parameters[1]);
		backgroundEvent.Filename = parameters[2];
		backgroundEvent.XOffset = int.Parse(parameters[3]);
		backgroundEvent.YOffset = int.Parse(parameters[4]);

		return backgroundEvent;
	}

	private static VideoEvent ParseVideoEvent(string[] parameters) {
		VideoEvent videoEvent = new();

		if (parameters.Length != 5)
			throw new ArgumentException($"Video event must have 5 parameters, but got {parameters.Length}");

		videoEvent.StartTime = int.Parse(parameters[1]);
		videoEvent.Filename = parameters[2];
		videoEvent.XOffset = int.Parse(parameters[3]);
		videoEvent.YOffset = int.Parse(parameters[4]);

		return videoEvent;
	}

	private static BreakEvent ParseBreakEvent(string[] parameters) {
		BreakEvent breakEvent = new();

		if (parameters.Length != 3)
			throw new ArgumentException($"Video event must have 3 parameters, but got {parameters.Length}");

		breakEvent.StartTime = int.Parse(parameters[1]);
		breakEvent.EndTime = int.Parse(parameters[2]);

		return breakEvent;
	}
}
