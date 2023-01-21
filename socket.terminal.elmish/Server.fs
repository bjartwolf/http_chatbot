module Server

open System.Threading
open System.Net
open socket.core.TcpWrappers
open ConnectionController
open System.Net.Sockets

let private cts = new CancellationTokenSource()
let private port = 13001;
let private localAddr = IPAddress.Parse("127.0.0.1");
let private ep = new IPEndPoint(localAddr, port);

let private tcpListener : ITcpListener = new TcpListenerWrapper(new TcpListener(ep)) 

let server = ListenConnections(tcpListener, cts.Token)
