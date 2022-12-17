namespace socket.core

module TcpWrappers =
    open System
    open System.Threading
    open System.Net
    open System.Net.Sockets

    type ITcpClient =
       abstract member GetStream: unit -> IO.Stream 
       abstract member Close: unit -> unit 

    type TcpClientWrapper (innerClient: TcpClient) = 
       let mutable _innerClient = innerClient

       interface ITcpClient with
           member this.Close(): unit = 
               _innerClient.Close() 
           member this.GetStream(): IO.Stream = 
               _innerClient.GetStream() 

    type ITcpListener = 
       abstract member Start: unit -> unit
       abstract member AcceptTcpClientAsync: CancellationToken -> Async<ITcpClient> 

    type TcpListenerWrapper (tcpListener: TcpListener) =
       let mutable _innerTcpLisneter = tcpListener
       interface ITcpListener with
           member this.AcceptTcpClientAsync(ct: CancellationToken): Async<ITcpClient> = 
                  async {
                    let! tcpClient = _innerTcpLisneter.AcceptTcpClientAsync(ct).AsTask() |> Async.AwaitTask 
                    return new TcpClientWrapper(tcpClient) 
                  }

           member this.Start(): unit = 
               raise (System.NotImplementedException())

       
