﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grpc.Core;
using Pj.Grpc.Sample;
using Pj.Grpc.Chat;
using System.Threading.Tasks;

public class ChatSample : MonoBehaviour {

  public Button buttonConnect;
  public Button buttonLeave;
  public Button buttonSendMsg;
  public Text text;

  public string ip = "127.0.0.1";
  public string port = "9999";

  public string userName = "shamaton";

  private Google.Protobuf.ByteString sid;

  private Channel channel;
  private Chat.ChatClient client;

  private string recvMsg = "";


  private void Start() {
    Debug.Log("start .....");
    buttonConnect.onClick.AddListener(Auth);
    buttonLeave.onClick.AddListener(Leave);
    buttonSendMsg.onClick.AddListener(Say);

    StartCoroutine(setMsg());
  }

  private IEnumerator setMsg() {
    var w = new WaitForSeconds(0.1f);
    while(true) {
      if (recvMsg.Length > 0) {
        // unity api must be called in main thread!!
        text.text = recvMsg;
        recvMsg = "";
      }
      yield return w;
    }
  }

  private void Auth() {
    channel = new Channel(ip + ":" + port, ChannelCredentials.Insecure);
    client = new Chat.ChatClient(channel);

    var req = new RequestAuthorize();
    req.Name = userName;

    var res = client.Authorize(req);
    sid = res.SessionId;


    Task.Run(() => Connect());
  }

  private async Task Connect() {
    var req = new RequestConnect();
    req.SessionId = sid;

    using (var call = client.Connect(req)) {
      while (await call.ResponseStream.MoveNext()) {
        var stream = call.ResponseStream.Current;
        if (stream.Join != null) {
          Debug.Log("join : " + stream.Join.Name);
        }
        else if (stream.Leave != null) {
          Debug.Log("leave : " + stream.Leave.Name);
        }
        else if (stream.Log != null) {
          Debug.Log("Say :" + stream.Log.Name + " - " + stream.Log.Message);
          recvMsg = stream.Log.Name + " - " + stream.Log.Message;
        }
      }
      Debug.Log("server accepted your leave signal");
    }

    channel.ShutdownAsync().Wait();
  }

  private void Say() {
    var req = new CommandSay();
    req.SessionId = sid;
    req.Message = userName + " -> " + System.DateTime.Now.ToString();

    /*var res = */client.Say(req);
  }

  private void Leave() {
    var req = new CommandLeave();
    req.SessionId = sid;

    /*var res = */client.Leave(req);
  }

}