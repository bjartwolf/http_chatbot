namespace socket.core

open System.Security.Cryptography.X509Certificates
open System.Net.Security

module TcpWrappers =
    open System
    open System.Threading
    open System.Net
    open System.Net.Sockets

    type ITcpClient =
       abstract member GetStream: unit -> IO.Stream 
       abstract member Close: unit -> unit 
       inherit IDisposable

    type TcpClientWrapper (innerClient: TcpClient, serverCertificate: X509Certificate2 option) = 
       let mutable _innerClient = innerClient

       interface ITcpClient with
           member this.Close(): unit = 
               _innerClient.Close() 
           member this.GetStream(): IO.Stream = 
               let tcpStream = _innerClient.GetStream() 
               match serverCertificate with 
                    | None -> tcpStream
                    | Some certificate -> 
                               let sslStream = new SslStream(tcpStream, true)
                               sslStream.ReadTimeout <- 1000000;
                               sslStream.WriteTimeout <- 1000000;
                               sslStream.AuthenticateAsServer(certificate, clientCertificateRequired = false, checkCertificateRevocation = false) 
                               sslStream

       interface IDisposable with
           member this.Dispose() =
                _innerClient.Dispose()

    type ITcpListener = 
       abstract member Start: unit -> unit
       abstract member Stop: unit -> unit
       abstract member AcceptTcpClientAsync: CancellationToken -> Async<(ITcpClient*IPEndPoint) option> 

    type TcpListenerWrapper (tcpListener: TcpListener, serverCertificate: X509Certificate2 option) =
       let mutable _innerTcpLisneter = tcpListener
       interface ITcpListener with
           member this.Stop(): unit = 
               _innerTcpLisneter.Stop() 
           member this.AcceptTcpClientAsync(ct: CancellationToken): Async<(ITcpClient*IPEndPoint) option> = 
                  async {
                    try 
                        let! tcpClient = _innerTcpLisneter.AcceptTcpClientAsync(ct).AsTask() |> Async.AwaitTask 
                        return Some (new TcpClientWrapper(tcpClient, serverCertificate), tcpClient.Client.RemoteEndPoint :?> IPEndPoint)
                    with    
                        _ -> return None 
                  }

           member this.Start(): unit = 
               _innerTcpLisneter.Start() 

       
