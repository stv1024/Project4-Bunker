using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

public class UManager : NetworkManager
{
    public static UManager Instance
    {
        get
        {
            return singleton as UManager;
        }
    }

    public int PlayerCampCount;
    public int[] CampOccupationList = new int[16];
    int _nextPlayerID = 1;
    public int ConnectedPlayerCount;
    public BattlefieldInfo BattlefieldInfo;

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.LogFormat("OnServerSceneChanged({0})", sceneName);
        base.OnServerSceneChanged(sceneName);
        ConnectedPlayerCount = 0;
        _nextPlayerID = 1;
        Debug.LogFormat("3_nextPlayerID={0}", _nextPlayerID);
        BattlefieldInfo = FindObjectOfType<BattlefieldInfo>();
        PlayerCampCount = BattlefieldInfo.BaseBunkerList.Length;
        for (int i = 1; i < PlayerCampCount + 1; i++)
        {
            CampOccupationList[i] = 0;
        }
    }
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.LogFormat("OnClientSceneChanged({0})", conn.address);
        base.OnClientSceneChanged(conn);
        BattlefieldInfo = FindObjectOfType<BattlefieldInfo>();
    }

    //Server
    public void DidAddPlayer(Unit playerUnit)
    {
        ConnectedPlayerCount++;
        var camp = 1;
        var min = CampOccupationList[camp];
        for (int i = 2; i < PlayerCampCount+1; i++)
        {
            if (CampOccupationList[i] < min)
            {
                camp = i;
                min = CampOccupationList[i];
            }
        }
        CampOccupationList[camp]++;
        Debug.LogFormat("DidAddPlayer ID={0} Camp={1}", _nextPlayerID, camp);
        playerUnit.ResetCampInfo(_nextPlayerID, camp);
        _nextPlayerID++;
    }

    public void DidRemovePlayer(Unit playerUnit)
    {
        ConnectedPlayerCount--;
        CampOccupationList[playerUnit.Data.Camp]--;
    }

    void FixedUpdate()
    {
        if (matchMaker) matchMaker.SetProgramAppID((AppID) 175251);
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.LogFormat("OnStopClient");
        var hud = GetComponent<UManagerHUG>();
        hud.showGUI = true;
    }
}