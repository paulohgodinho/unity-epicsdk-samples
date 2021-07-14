using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Achievements;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    private ProductUserId _productUserId;

    void Start()
    {
        EpicPlatform.Instance.OnConnectedToEpicOnlineServices += OnConnectedToEpicOnlineServices;
    }

    private void OnConnectedToEpicOnlineServices(ProductUserId productUserId)
    {
        _productUserId = productUserId;
        ListAchievementsAndProgress();
    }

    private void ListAchievementsAndProgress()
    {
        var achievementInterface = EpicPlatform.Instance.PlatformInterface.GetAchievementsInterface();

        Log("Listing Achievements and Progress");

        var queryPlayerAchievementsOptions = new QueryPlayerAchievementsOptions
        {
            LocalUserId = _productUserId,
            TargetUserId = _productUserId
        };

        achievementInterface.QueryPlayerAchievements(queryPlayerAchievementsOptions, null, _ =>
        {
            uint achievementCount = achievementInterface.GetPlayerAchievementCount(new GetPlayerAchievementCountOptions
            {
                UserId = _productUserId
            });

            Log($"Found {achievementCount} achievements");

            StringBuilder achievementLog = new StringBuilder("Achievement List: \n");
            for (uint i = 0; i < achievementCount; i++)
            {
                var copyOptions = new CopyPlayerAchievementByIndexOptions
                {
                    AchievementIndex = i,
                    LocalUserId = _productUserId,
                    TargetUserId = _productUserId
                };
                
                achievementInterface.CopyPlayerAchievementByIndex(copyOptions, out var achievement);

                achievementLog.AppendLine($"{achievement.AchievementId} Progress: {achievement.Progress}");
            }

            Log(achievementLog.ToString());
        });
    }

    public void UnlockAchievement(string achievementId)
    {
        var achievementInterface = EpicPlatform.Instance.PlatformInterface.GetAchievementsInterface();

        UnlockAchievementsOptions unlockAchievementsOptions = new UnlockAchievementsOptions
        {
            AchievementIds = new[] {achievementId},
            UserId = _productUserId
        };

        achievementInterface.UnlockAchievements(unlockAchievementsOptions, null,
            data => { Log($"Achievement unlock: {achievementId}  {data.ResultCode}"); });
    }

    private void Log(string log)
    {
        Debug.Log($"<b>[EpicAchievements]</b> {log}");
    }
}