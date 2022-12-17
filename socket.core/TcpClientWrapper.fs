namespace socket.core

module TcpWrappers =
    open System
    open System.Threading
    open System.Net
    open System.Net.Sockets

    type ITcpClient =
       abstract member GetStream: unit -> IO.Stream 
       abstract member Close: unit -> unit 
       inherit IDisposable

    type TcpClientWrapper (innerClient: TcpClient) = 
       let mutable _innerClient = innerClient

       interface ITcpClient with
           member this.Close(): unit = 
               _innerClient.Close() 
           member this.GetStream(): IO.Stream = 
               _innerClient.GetStream() 
       interface IDisposable with
           member this.Dispose() =
                _innerClient.Dispose()

    type ITcpListener = 
       abstract member Start: unit -> unit
       abstract member AcceptTcpClientAsync: CancellationToken -> Async<ITcpClient*IPEndPoint> 

    type TcpListenerWrapper (tcpListener: TcpListener) =
       let mutable _innerTcpLisneter = tcpListener
       interface ITcpListener with
           member this.AcceptTcpClientAsync(ct: CancellationToken): Async<ITcpClient*IPEndPoint> = 
                  async {
                    let! tcpClient = _innerTcpLisneter.AcceptTcpClientAsync(ct).AsTask() |> Async.AwaitTask 
                    return (new TcpClientWrapper(tcpClient), tcpClient.Client.RemoteEndPoint :?> IPEndPoint)
                  }

           member this.Start(): unit = 
               _innerTcpLisneter.Start() 

       
