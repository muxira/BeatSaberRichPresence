using HarmonyLib;
using Zenject;
using DiscordRichPresence.Services;

namespace DiscordRichPresence.HarmonyPatches
{
    [HarmonyPatch(typeof(ResultsViewController), "Init")]
    internal class ResultsViewControllerPatch
    {
        private static void Postfix(LevelCompletionResults levelCompletionResults, IReadonlyBeatmapData transformedBeatmapData, in BeatmapKey beatmapKey, BeatmapLevel beatmapLevel, bool practice, bool newHighScore)
        {
            var container = ProjectContext.Instance?.Container;
            if (container == null) return;
            
            var presenceManager = container.TryResolve<DiscordPresenceManager>();
            if (presenceManager == null) return;

            string rankStr = levelCompletionResults.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared 
                ? levelCompletionResults.rank.ToString() 
                : "Fail";
                
            int totalNotes = levelCompletionResults.goodCutsCount + levelCompletionResults.badCutsCount + levelCompletionResults.missedCount;

            var activity = new Discord.Activity
            {
                Details = $"Пройдено: {beatmapLevel.songName} - {beatmapLevel.songAuthorName}",
                State = $"Ранг: {rankStr} | Очки: {levelCompletionResults.modifiedScore} | Комбо: {levelCompletionResults.maxCombo}x",
                Assets = new Discord.ActivityAssets
                {
                    LargeImage = "default_icon",
                    LargeText = $"{beatmapLevel.songName} [{beatmapKey.difficulty}]",
                    SmallImage = "passed",
                    SmallText = "Пройдено"
                }
            };

            presenceManager.SetActivity(activity, immediate: true);
        }
    }
}
