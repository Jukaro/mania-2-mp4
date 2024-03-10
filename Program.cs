
using Rythmify.Core.Beatmap;

var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/400078 Kurokotei - Galaxy Collapse/Kurokotei - Galaxy Collapse (Mat) [Cataclysmic Hypernova].osu";
var beatmap = BeatmapParser.Parse(filePath);
Logger.LogSuccess(beatmap.ToString());