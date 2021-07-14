using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Stats;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private ProductUserId _productUserId;
    
    void Start()
    {
        EpicPlatform.Instance.OnConnectedToEpicOnlineServices += OnConnectedToEpicOnlineServices;
    }

    private void OnConnectedToEpicOnlineServices(ProductUserId productUserId)
    {
        _productUserId = productUserId;
        ListStatsAndTheirProgress();
    }

    private void ListStatsAndTheirProgress()
    {
        var statsInterface = EpicPlatform.Instance.PlatformInterface.GetStatsInterface();

        Log("Listing Stats and Progress");
        
        var queryStatsOptions = new QueryStatsOptions()
        {
            LocalUserId = _productUserId,
            TargetUserId = _productUserId
        };
        
        statsInterface.QueryStats(queryStatsOptions, null, _ =>
        {
            uint statsCount = statsInterface.GetStatsCount(new GetStatCountOptions() {TargetUserId = _productUserId});
            
            Log($"Found {statsCount} stats");
            
            StringBuilder statsLog = new StringBuilder("Stats List: \n");
            for (uint i = 0; i < statsCount; i++)
            {
                var copyOptions = new CopyStatByIndexOptions()
                {
                    StatIndex = i,
                    TargetUserId = _productUserId
                };

                statsInterface.CopyStatByIndex(copyOptions, out var stat);
                
                statsLog.AppendLine($"{stat.Name} Progress: {stat.Value}");
            }
            
            Log(statsLog.ToString());
        });
    }
    
    private void Log(string log)
    {
        Debug.Log($"<b>[EpicStats]</b> {log}");
    }
}
