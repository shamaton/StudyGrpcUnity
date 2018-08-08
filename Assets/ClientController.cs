using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grpc.Core;
using Pj.Grpc.Sample;

public class ClientController : MonoBehaviour {

  public Button button;
  public Text text;

  public string ip = "127.0.0.1";
  public string port = "9999";

  private void Start() {
    Debug.Log("start .....");
    button.onClick.AddListener(Say);
  }

  private void Say() {
    Debug.Log("say.....");
    Channel channel = new Channel(ip + ":" + port, ChannelCredentials.Insecure);

    var client = new Greeter.GreeterClient(channel);
    string user = "you";

    var reply = client.SayHello(new HelloRequest { Name = user });
    text.text = "reply : " + reply.Message;

    channel.ShutdownAsync().Wait();
  }
}