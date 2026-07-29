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
                Details = $"Результат: {rankStr}",
                State = $"Комбо: {levelCompletionResults.maxCombo}x | Кубов: {levelCompletionResults.goodCutsCount}/{totalNotes} | Очки: {levelCompletionResults.modifiedScore}",
                Assets = new Discord.ActivityAssets
                {
                    LargeImage = "default_icon",
                    LargeText = beatmapLevel.songName
                }
            };

            presenceManager.SetActivity(activity, immediate: true);
        }
    }
}
