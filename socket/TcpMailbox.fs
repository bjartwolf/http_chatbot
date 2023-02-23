module TcpMailbox

open System.Net.Sockets
open FSharp.Control
open System.Threading.Tasks
open System.Threading
open System
open System.IO

open socket.core.TcpWrappers

let readBufferAsync (bytes: byte[]) (networkStream: Stream): Async<int option> = async {
    try 
        //if (networkStream.CanRead && networkStream.DataAvailable) then
        if (networkStream.CanRead) then
            let token = (new CancellationTokenSource(TimeSpan.FromMilliseconds(5))).Token
            let! i = networkStream.ReadAsync(bytes, 0, bytes.Length, token) |> Async.AwaitTask
            return Some i
        else 
            return None
    with
            | :? ObjectDisposedException -> return Some 0
            | :? SocketException -> return Some 0
            | :? TaskCanceledException -> return None
            | :? OperationCanceledException -> return None
            | ex -> //printfn "Some unknown exception  %A" ex
                    return None
}  

// TODO Consider closing both ways to close connection, but we are a server
type ConnectionStatus = Open | Closed
type lineFeed = MesssageToSend of string | GetRecieved of AsyncReplyChannel<string*ConnectionStatus> | GetSent of AsyncReplyChannel<string*ConnectionStatus>  | Close 
let ListenMessages (client : ITcpClient) = MailboxProcessor<lineFeed>.Start( fun inbox ->
    let networkStream = client.GetStream()
    let bytes: byte [] = Array.zeroCreate 4096 
    let mutable recievedString = ""
    let mutable sentString = ""
    let mutable streamState = Open 
    let rec innerLoop () = async {
        let! msg = inbox.TryReceive(10)
        match msg with 
            | Some (MesssageToSend msg) -> 
                match streamState with
                    | Open -> 
                            let msgWithNewline = msg + System.Environment.NewLine
                            let bytesToSend = System.Text.Encoding.ASCII.GetBytes(msgWithNewline)
                            // need to handle writing errors...
                            try 
                                do! networkStream.WriteAsync(bytesToSend, 0, bytesToSend.Length) |> Async.AwaitTask
                                sentString <- sentString + msgWithNewline;
                            with 
                                | :? ObjectDisposedException -> streamState <- Closed 
                                | :? SocketException -> streamState <- Closed 
                                | :? TaskCanceledException -> streamState <- Closed 
                                | :? OperationCanceledException -> streamState <- Closed 
                                | :? AggregateException -> streamState <- Closed 
                    | Closed -> () 
                return! innerLoop()
            | Some (GetSent reply) ->
                reply.Reply((sentString, streamState)) 
                return! innerLoop()
            | Some (GetRecieved(reply)) -> 
                reply.Reply(recievedString, streamState)
                return! innerLoop()
            | Some (Close) -> 
                   streamState <- Closed 
                   do! networkStream.FlushAsync() |> Async.AwaitTask
                   networkStream.Close()
                   client.Close();
                   client.Dispose()
                   return! innerLoop()
            | None -> 
                   let! bytesRead = readBufferAsync bytes networkStream
                   match bytesRead with
                     | Some 0 -> streamState <- Closed
                                 networkStream.Close()
                                 client.Dispose()
                                 return! innerLoop()
                     | Some bytesRead ->
                            recievedString <- recievedString + System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead)
                            return! innerLoop()
                     | None -> return! innerLoop()
    }
    innerLoop ())