﻿// This thing will listen for connections on the local endpoint
// and let the consumer loop over active connections and get new connections
// without blocking.
module ConnectionController

open System.Net
open System.Net.Sockets
open System.Threading
open socket.core.TcpWrappers

type ConnectionMsg = GetNewConnection of AsyncReplyChannel< (ITcpClient*IPEndPoint) option > | CloseConnnection of IPEndPoint // Not sure I will need this yet| GetAllConnections of IPEndPoint list 

let ListenConnections () = MailboxProcessor<ConnectionMsg>.Start( fun inbox ->
    let port = 13001;
    let localAddr = IPAddress.Parse("127.0.0.1");
    let ep = new IPEndPoint(localAddr, port);
    let foo = new TcpListener(ep)
    let server: ITcpListener = new TcpListenerWrapper(foo) 
    server.Start();
    let rec innerLoop () = async {
        let! msg = inbox.TryReceive(10)
        match msg with 
            | Some (GetNewConnection(reply)) -> 
                let token = (new CancellationTokenSource(System.TimeSpan.FromMilliseconds(1))).Token
                try 
                    let! connection, ipEndpoint = server.AcceptTcpClientAsync(token)
                    reply.Reply(Some (connection, ipEndpoint))
                with 
                    | _ -> reply.Reply(None)
                return! innerLoop()
            | Some (CloseConnnection ip) -> ()// TODO CLOSE CONNECTion
                
            | None -> ()
        return! innerLoop()
    }
    innerLoop ())

