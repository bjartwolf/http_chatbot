namespace socket.terminal 

module Commands =
    open ConnectionController
    open TcpMailbox
    open Terminal.Gui.Elmish
    open Messages

    let server = Server.server 
    
    let listenForConnection =
        fun dispatch ->
            async {
                let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
                match reply with 
                    | Some (client, endpoint) -> dispatch (ConnectionEstablished (ListenMessages client, endpoint)) 
                    | None -> () 
            }
            |> Async.StartImmediate
        |> Cmd.ofSub

    let getSentAndRecieved ((client, _): Connection) =
        fun dispatch ->
            async {
                let (sentdata,statusClient)= client.PostAndReply(fun channel -> (GetRecieved channel))
                let (recievedData,statusServer)= client.PostAndReply(fun channel -> (GetSent channel))
                dispatch (ConnectionDataReceived (sentdata, recievedData, statusClient, statusServer))
            }
            |> Async.StartImmediate
        |> Cmd.ofSub

    let sendData ((client, _): Connection) (dataToSend: string) =
        fun dispatch ->
            async {
                client.Post(TcpMailbox.MesssageToSend dataToSend)
                dispatch (Tick)
            }
            |> Async.StartImmediate
        |> Cmd.ofSub



