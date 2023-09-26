namespace socket.terminal 

open Elmish
open Avalonia.Threading
open System

module Commands =
    open ConnectionController
    open TcpMailbox
    open Messages
    open Server

    let server = listeningServer
    
    let listenForConnection =
        fun dispatch ->
                async {
                    let! reply = server.PostAndAsyncReply(fun channel -> (GetNewConnection channel))
                    match reply with 
                        | Some (client, endpoint) -> 
                                       let invoke() = ConnectionEstablished (ListenMessages client, endpoint) |> dispatch
                                       Dispatcher.UIThread.Invoke(Action(invoke))
                        | None -> let invoke() = RefreshSentReceived |> dispatch 
                                  Dispatcher.UIThread.Invoke(Action(invoke))
                     }
                |> Async.Start 
        |> Cmd.ofEffect

    let getSentAndRecieved ((client, _): Connection) =
        fun dispatch ->
                async {
                    let! (sentdata,statusClient)= client.PostAndAsyncReply(fun channel -> (GetRecieved channel))
                    let! (recievedData,statusServer)= client.PostAndAsyncReply(fun channel -> (GetSent channel))
                    let invoke() = ConnectionDataReceived (sentdata, recievedData, statusClient, statusServer)|> dispatch
                    Dispatcher.UIThread.Invoke(Action(invoke))
                }
                |> Async.Start
        |> Cmd.ofEffect

    let sendData ((client, _): Connection) (dataToSend: string) =
        fun dispatch ->
                client.Post(TcpMailbox.MesssageToSend dataToSend)
                // todo possibly UI thread?
                dispatch RefreshSentReceived
        |> Cmd.ofEffect

    let closeCurrent ((client, _): Connection) = 
        fun dispatch ->
                client.Post(Close)
                // todo possibly UI thread?
                dispatch ClosedCurrent
        |> Cmd.ofEffect





