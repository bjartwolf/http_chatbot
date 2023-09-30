module Server

open System.Threading
open System.Net
open socket.core.TcpWrappers
open ConnectionController
open System.Net.Sockets
open System.Security.Cryptography.X509Certificates

// TODO read parameters from command line on IPs, ports to listen too, 
// establishing TLS or not, paths to certs and the possibility to run 
// without TLS.
let listeningServer () = 
    let cts = new CancellationTokenSource()
    let port = 13001;
    let localAddr = IPAddress.Parse("127.0.0.1");
    //let private localAddr = IPAddress.Parse("192.168.10.144");
    let ep = new IPEndPoint(localAddr, port);

    let serverCertificate = X509Certificate2.CreateFromPemFile("../../../../certs/fullchain.pem", "../../../../certs/privkey.pem");
    let reformattedServerCertificate = new X509Certificate2(serverCertificate.Export(X509ContentType.Pkcs12)) 
    let tcpListener = new TcpListenerWrapper(new TcpListener(ep), reformattedServerCertificate) 
    
    ListenConnections(tcpListener, cts.Token)
