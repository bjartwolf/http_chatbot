module Server

open System.Threading
open System.Net
open socket.core.TcpWrappers
open ConnectionController
open System.Net.Sockets
open System.Security.Cryptography.X509Certificates

type Config = { port: uint16;
                ipAddress: IPAddress;
                certPemFilePath: string;
                keyPemFilePath: string;
                insecure: bool }

let listeningServer (config: Config) = 
    let cts = new CancellationTokenSource()
    let ep = new IPEndPoint(config.ipAddress, int config.port);

    let serverCertificate: X509Certificate2 option  = if config.insecure then
                                                           None
                                                      else let cert = X509Certificate2.CreateFromPemFile(config.certPemFilePath, config.keyPemFilePath)
                                                           Some (new X509Certificate2(cert.Export(X509ContentType.Pkcs12)))

    let tcpListener = new TcpListenerWrapper(new TcpListener(ep), serverCertificate) 
    
    ListenConnections(tcpListener, cts.Token)
