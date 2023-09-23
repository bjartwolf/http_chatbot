// This thing will listen for connections on the local endpoint
// and let the consumer loop over active connections and get new connections
// without blocking.
module ConnectionController

open System.Net
open System.Threading
open socket.core.TcpWrappers
open System.Security.Cryptography.X509Certificates

type ConnectionMsg = GetNewConnection of AsyncReplyChannel< (ITcpClient*IPEndPoint) option > 
                   | CloseConnnection of IPEndPoint // Not sure I will need this yet| GetAllConnections of IPEndPoint list 

let ListenConnections (server: ITcpListener, ct: CancellationToken) = MailboxProcessor<ConnectionMsg>.Start( fun inbox ->
    server.Start();
    let rec innerLoop () = async {
        let! msg = inbox.TryReceive(20)
        match msg with 
            | Some (GetNewConnection(reply)) -> 
                let token = (new CancellationTokenSource(System.TimeSpan.FromMilliseconds(15))).Token
                try 
                    let! x = server.AcceptTcpClientAsync(token)
                    match x with 
                        | Some (connection, ipEndpoint)-> reply.Reply(Some (connection, ipEndpoint))
                        | None -> reply.Reply(None)
                with 
                    | _ -> reply.Reply(None)
                return! innerLoop()
            | Some (CloseConnnection ip) -> ()// TODO CLOSE CONNECTion
                
            | None -> ()
        if (ct.IsCancellationRequested) then 
            server.Stop()
        else
            return! innerLoop()
    }
    innerLoop ())


