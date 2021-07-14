using Epic.OnlineServices;
using Epic.OnlineServices.Friends;
using Epic.OnlineServices.UserInfo;
using UnityEngine;

public class Friends : MonoBehaviour
{
    void Start()
    {
        EpicPlatform.Instance.OnAuthenticatedWithEpic += ListFriends;
    }

    void ListFriends()
    {
        var friendsInterface = EpicPlatform.Instance.PlatformInterface.GetFriendsInterface();

        QueryFriendsOptions options = new QueryFriendsOptions()
        {
            LocalUserId = EpicPlatform.Instance.EpicLocalUserId
        };

        Log("Querying Friends");
        friendsInterface.QueryFriends(options, null, queryFriendsCallbackInfo =>
        {
            Log($"Refreshing Friends is: {queryFriendsCallbackInfo.ResultCode}");

            GetFriendsCountOptions countOptions = new GetFriendsCountOptions()
            {
                LocalUserId = EpicPlatform.Instance.EpicLocalUserId
            };

            int friendCount = friendsInterface.GetFriendsCount(countOptions);

            Log($"Number of Friends: {friendCount}");

            for (int i = 0; i < friendCount; i++)
            {
                var friendAtIndexOption = new GetFriendAtIndexOptions()
                {
                    Index = i,
                    LocalUserId = EpicPlatform.Instance.EpicLocalUserId
                };

                var friendId = friendsInterface.GetFriendAtIndex(friendAtIndexOption);
                GetAndPrintFriendDetails(friendId);
            }
        });
    }

    private void GetAndPrintFriendDetails(EpicAccountId friendId)
    {
        var queryUserInterface = EpicPlatform.Instance.PlatformInterface.GetUserInfoInterface();

        var queryUserInfoOptions = new QueryUserInfoOptions()
        {
            LocalUserId = EpicPlatform.Instance.EpicLocalUserId,
            TargetUserId = friendId
        };

        queryUserInterface.QueryUserInfo(queryUserInfoOptions, null,
            queryUserInfoCallbackInfo =>
            {
                if (queryUserInfoCallbackInfo.ResultCode == Result.Success)
                {
                    var copyUserInfoOptions = new CopyUserInfoOptions()
                    {
                        LocalUserId = queryUserInfoCallbackInfo.LocalUserId,
                        TargetUserId = queryUserInfoCallbackInfo.TargetUserId
                    };

                    queryUserInterface.CopyUserInfo(copyUserInfoOptions, out var userData);

                    var friendsInterface = EpicPlatform.Instance.PlatformInterface.GetFriendsInterface();

                    var getStatusOptions = new GetStatusOptions()
                    {
                        LocalUserId = EpicPlatform.Instance.EpicLocalUserId,
                        TargetUserId = friendId
                    };

                    var status = friendsInterface.GetStatus(getStatusOptions);

                    Log($"Got Friend: {userData.DisplayName} We are: {status}");
                }
            });
    }

    private void Log(string log)
    {
        Debug.Log($"<b>[EpicFriends]</b> {log}");
    }
}