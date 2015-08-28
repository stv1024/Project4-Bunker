
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

    [EditorBrowsable(EditorBrowsableState.Never)]
[RequireComponent(typeof (NetworkManager))]
public class UManagerHUG : MonoBehaviour
{
        /// <summary>
        /// 
        /// <para>
        /// Whether to show the default control HUD at runtime.
        /// </para>
        /// 
        /// </summary>
        [SerializeField]
        public bool showGUI = true;
        /// <summary>
        /// 
        /// <para>
        /// The NetworkManager associated with this HUD.
        /// </para>
        /// 
        /// </summary>
        public NetworkManager manager;
        /// <summary>
        /// 
        /// <para>
        /// The horizontal offset in pixels to draw the HUD runtime GUI at.
        /// </para>
        /// 
        /// </summary>
        [SerializeField]
        public int offsetX;
        /// <summary>
        /// 
        /// <para>
        /// The vertical offset in pixels to draw the HUD runtime GUI at.
        /// </para>
        /// 
        /// </summary>
        [SerializeField]
        public int offsetY;

        private bool showServer;
        private float zoomRatio = 1;

        private void Awake()
        {
            this.manager = this.GetComponent<NetworkManager>();
        }

        private void Update()
        {
            if (!this.showGUI)
                return;
            if (!NetworkClient.active && !NetworkServer.active && (UnityEngine.Object)this.manager.matchMaker == (UnityEngine.Object)null)
            {
                if (Input.GetKeyDown(KeyCode.S))
                    this.manager.StartServer();
                if (Input.GetKeyDown(KeyCode.H))
                    this.manager.StartHost();
                if (Input.GetKeyDown(KeyCode.C))
                    this.manager.StartClient();
            }
            if (!NetworkServer.active || !NetworkClient.active || !Input.GetKeyDown(KeyCode.X))
                return;
            this.manager.StopHost();
        }

        private void OnGUI()
        {
            if (!this.showGUI)
                return;
            zoomRatio = Screen.height / 400f;
            GUI.skin.label.fontSize = (int)zoomRatio*15;
            GUI.skin.button.fontSize = (int)zoomRatio*15;
            GUI.skin.textField.fontSize = (int)zoomRatio*15;
            //GUI.skin = customSkin;
            int num1 = 10 + this.offsetX;
            int num2 = 40 + this.offsetY;
            int num3 = 24;
            if (!NetworkClient.active && !NetworkServer.active && (UnityEngine.Object)this.manager.matchMaker == (UnityEngine.Object)null)
            {
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num2 * zoomRatio, 200f * zoomRatio, 20f*zoomRatio), "LAN Host(H)"))
                    this.manager.StartHost();
                int num4 = num2 + num3;
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num4 * zoomRatio, 105f * zoomRatio, 20f * zoomRatio), "LAN Client(C)"))
                    this.manager.StartClient();
                this.manager.networkAddress =
                    GUI.TextField(
                        new Rect((float) (num1 + 100)*zoomRatio, (float) num4*zoomRatio, 95f*zoomRatio, 20f*zoomRatio),
                        this.manager.networkAddress);
                int num5 = num4 + num3;
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num5 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "LAN Server Only(S)"))
                    this.manager.StartServer();
                num2 = num5 + num3;
            }
            else
            {
                if (NetworkServer.active)
                {
                    GUI.Label(new Rect((float)num1 * zoomRatio, (float)num2 * zoomRatio, 300f * zoomRatio, 20f * zoomRatio), "Server: port=" + (object)this.manager.networkPort);
                    num2 += num3;
                }
                if (NetworkClient.active)
                {
                    Rect position = new Rect((float)num1 * zoomRatio, (float)num2 * zoomRatio, 300f * zoomRatio, 20f * zoomRatio);
                    object[] objArray = new object[4];
                    int index1 = 0;
                    string str1 = "Client: address=";
                    objArray[index1] = (object)str1;
                    int index2 = 1;
                    string networkAddress = this.manager.networkAddress;
                    objArray[index2] = (object)networkAddress;
                    int index3 = 2;
                    string str2 = " port=";
                    objArray[index3] = (object)str2;
                    int index4 = 3;
                    // ISSUE: variable of a boxed type
                    int local = this.manager.networkPort;
                    objArray[index4] = (object)local;
                    string text = string.Concat(objArray);
                    GUI.Label(position, text);
                    num2 += num3;
                }
            }
            if (NetworkClient.active && !ClientScene.ready)
            {
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num2 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Client Ready"))
                {
                    ClientScene.Ready(this.manager.client.connection);
                    if (ClientScene.localPlayers.Count == 0)
                        ClientScene.AddPlayer((short)0);
                }
                num2 += num3;
            }
            if (NetworkServer.active || NetworkClient.active)
            {
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num2 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Stop (X)"))
                    this.manager.StopHost();
                num2 += num3;
            }
            if (NetworkServer.active || NetworkClient.active)
                return;
            int num6 = num2 + 10;
            int num7;
            if ((UnityEngine.Object)this.manager.matchMaker == (UnityEngine.Object)null)
            {
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num6 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Enable Match Maker (M)"))
                    this.manager.StartMatchMaker();
                num7 = num6 + num3;
            }
            else
            {
                if (this.manager.matchInfo == null)
                {
                    if (this.manager.matches == null)
                    {
                        if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num6 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Create Internet Match"))
                            this.manager.matchMaker.CreateMatch(this.manager.matchName, this.manager.matchSize, true, string.Empty, new NetworkMatch.ResponseDelegate<CreateMatchResponse>(this.manager.OnMatchCreate));
                        int num4 = num6 + num3;
                        GUI.Label(new Rect((float)num1 * zoomRatio, (float)num4 * zoomRatio, 100f * zoomRatio, 20f * zoomRatio), "Room Name:");
                        this.manager.matchName = GUI.TextField(new Rect((float)(num1 + 100) * zoomRatio, (float)num4 * zoomRatio, 100f * zoomRatio, 20f * zoomRatio), this.manager.matchName);
                        int num5 = num4 + num3 + 10;
                        if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num5 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Find Internet Match"))
                            this.manager.matchMaker.ListMatches(0, 20, string.Empty, new NetworkMatch.ResponseDelegate<ListMatchResponse>(this.manager.OnMatchList));
                        num6 = num5 + num3;
                    }
                    else
                    {
                        using (List<MatchDesc>.Enumerator enumerator = this.manager.matches.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                MatchDesc current = enumerator.Current;
                                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num6 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Join Match:" + current.name))
                                {
                                    this.manager.matchName = current.name;
                                    this.manager.matchSize = (uint)current.currentSize;
                                    this.manager.matchMaker.JoinMatch(current.networkId, string.Empty, new NetworkMatch.ResponseDelegate<JoinMatchResponse>(this.manager.OnMatchJoined));
                                }
                                num6 += num3;
                            }
                        }
                    }
                }
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num6 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Change MM server"))
                    this.showServer = !this.showServer;
                if (this.showServer)
                {
                    int num4 = num6 + num3;
                    if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num4 * zoomRatio, 100f * zoomRatio, 20f * zoomRatio), "Local"))
                    {
                        this.manager.SetMatchHost("localhost", 1337, false);
                        this.showServer = false;
                    }
                    int num5 = num4 + num3;
                    if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num5 * zoomRatio, 100f * zoomRatio, 20f * zoomRatio), "Internet"))
                    {
                        this.manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
                        this.showServer = false;
                    }
                    num6 = num5 + num3;
                    if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num6 * zoomRatio, 100f * zoomRatio, 20f * zoomRatio), "Staging"))
                    {
                        this.manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
                        this.showServer = false;
                    }
                }
                int num8 = num6 + num3;
                GUI.Label(new Rect((float)num1 * zoomRatio, (float)num8 * zoomRatio, 300f * zoomRatio, 20f * zoomRatio), "MM Uri: " + (object)this.manager.matchMaker.baseUri);
                int num9 = num8 + num3;
                if (GUI.Button(new Rect((float)num1 * zoomRatio, (float)num9 * zoomRatio, 200f * zoomRatio, 20f * zoomRatio), "Disable Match Maker"))
                    this.manager.StopMatchMaker();
                num7 = num9 + num3;
            }
        }
    
}