namespace socket.terminal 

open Elmish
open Avalonia.Threading

module Commands =
    open ConnectionController
    open TcpMailbox
    open Messages
    open Server
    open Model 

    let mutable server: MailboxProcessor<ConnectionMsg> = MailboxProcessor<ConnectionMsg>.Start( fun _ ->  async {()})

    let initServerWithConfig(config: Config) : unit = 
        server <- listeningServer(config)
        ()

    let listenForConnection =
        fun dispatch ->
                async {
                    let! reply = server.PostAndAsyncReply(fun channel -> (GetNewConnection channel))
                    match reply with 
                        | Some (client, endpoint) -> Dispatcher.UIThread.Invoke(fun () -> ConnectionEstablished (ListenMessages client, endpoint) |> dispatch)
                        | None -> Dispatcher.UIThread.Invoke(fun () -> dispatch RefreshSentReceived)
                     }
                |> Async.Start 
        |> Cmd.ofEffect

    let getSentAndRecieved ((client, _): Connection) =
        fun dispatch ->
                async {
                    let! (sentdata,statusClient)= client.PostAndAsyncReply(fun channel -> (GetRecieved channel))
                    let! (recievedData,statusServer)= client.PostAndAsyncReply(fun channel -> (GetSent channel))
                    Dispatcher.UIThread.Invoke(fun () -> dispatch (ConnectionDataReceived (sentdata, recievedData, statusClient, statusServer))) 
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





