module Server

open System.Threading
open System.Net
open socket.core.TcpWrappers
open ConnectionController
open System.Net.Sockets
open System.Security.Cryptography.X509Certificates

let private cts = new CancellationTokenSource()
let private port = 13001;
let private localAddr = IPAddress.Parse("127.0.0.1");
//let private localAddr = IPAddress.Parse("192.168.10.144");
let private ep = new IPEndPoint(localAddr, port);


let serverCertificate = X509Certificate2.CreateFromPemFile("../../../../certs/fullchain.pem", "../../../../certs/privkey.pem");

let reformattedServerCertificate = new X509Certificate2(serverCertificate.Export(X509ContentType.Pkcs12)) 

let private tcpListener : ITcpListener = new TcpListenerWrapper(new TcpListener(ep), reformattedServerCertificate) 

let server = ListenConnections(tcpListener, cts.Token)
