using System;
using System.Collections.Generic;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	static readonly Dictionary<BeatmapEventType, Action<List<BeatmapEvent>, string[]>> eventToParser = new() {
		{BeatmapEventType.Background, (events, parameters) => { events.Add(ParseBackgroundEvent(parameters)); } },
		{BeatmapEventType.Video, (events, parameters) => { events.Add(ParseVideoEvent(parameters)); } },
		{BeatmapEventType.Break, (events, parameters) => { events.Add(ParseBreakEvent(parameters)); } },
	};

	private static BeatmapEvent[] ParseEventsSection(string[] lines) {
		List<BeatmapEvent> events = new();

		foreach (string line in lines) {
			var parameters = Array.ConvertAll(line.Split(','), (string s) => s.Trim());
			var eventType = BeatmapEvent.TryGetEventType(parameters[0]);

			if (!eventType.HasValue) {
				Logger.LogWarning($"[BeatmapParser] Unknown event type {parameters[0]}");
				continue;
			}

			if (eventToParser.TryGetValue(eventType.Value, out Action<List<BeatmapEvent>, string[]> parser))
				parser(events, parameters);
			else
				Logger.LogWarning($"[BeatmapParser] Could not find parser for event type {eventType}");
		}

		return events.ToArray();
	}

	private static BackgroundEvent ParseBackgroundEvent(string[] parameters) {
		BackgroundEvent backgroundEvent = new();

		if (parameters.Length < 3)
			throw new ArgumentException($"Background event must have at least 3 parameters, but got {parameters.Length}");

		backgroundEvent.StartTime = int.Parse(parameters[1]);
		backgroundEvent.Filename = parameters[2];
		backgroundEvent.XOffset = parameters.Length >= 4 ? int.Parse(parameters[3]) : 0;
		backgroundEvent.YOffset = parameters.Length >= 5 ? int.Parse(parameters[4]) : 0;

		return backgroundEvent;
	}

	private static VideoEvent ParseVideoEvent(string[] parameters) {
		VideoEvent videoEvent = new();

		if (parameters.Length < 3)
			throw new ArgumentException($"Video event must have at least 3 parameters, but got {parameters.Length}");

		videoEvent.StartTime = int.Parse(parameters[1]);
		videoEvent.Filename = parameters[2];
		videoEvent.XOffset = parameters.Length >= 4 ? int.Parse(parameters[3]) : 0;
		videoEvent.YOffset = parameters.Length >= 5 ? int.Parse(parameters[4]) : 0;

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
