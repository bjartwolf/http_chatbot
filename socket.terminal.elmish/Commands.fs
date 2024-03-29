﻿namespace socket.terminal 

open System.Net

module Commands =
    open ConnectionController
    open TcpMailbox
    open Terminal.Gui.Elmish
    open Messages
    open Server
    open Model

    let config = { port = uint16 13001;
                   ipAddress= IPAddress.Parse("127.0.0.1");
                   certPemFilePath = "C:\code\http_chatbot\Certs\fullchain.pem";
                   keyPemFilePath = "C:\code\http_chatbot\Certs\privkey.pem";
                   insecure= false}

    let server = listeningServer(config)
    
    let listenForConnection =
        fun dispatch ->
                async {
                    let! reply = server.PostAndAsyncReply(fun channel -> (GetNewConnection channel))
                    match reply with 
                        | Some (client, endpoint) -> dispatch (ConnectionEstablished (ListenMessages client, endpoint)) 
                        | None -> dispatch RefreshSentReceived 
                    }
                |> Async.Start
        |> Cmd.ofSub

    let getSentAndRecieved ((client, _): Connection) =
        fun dispatch ->
                async {
                    let! (sentdata,statusClient)= client.PostAndAsyncReply(fun channel -> (GetRecieved channel))
                    let! (recievedData,statusServer)= client.PostAndAsyncReply(fun channel -> (GetSent channel))
                    dispatch (ConnectionDataReceived (sentdata, recievedData, statusClient, statusServer))
                }
                |> Async.Start
        |> Cmd.ofSub

    let sendData ((client, _): Connection) (dataToSend: string) =
        fun dispatch ->
                client.Post(TcpMailbox.MesssageToSend dataToSend)
                dispatch RefreshSentReceived
        |> Cmd.ofSub

    let closeCurrent ((client, _): Connection) = 
        fun dispatch ->
                client.Post(Close)
                dispatch ClosedCurrent
        |> Cmd.ofSub




