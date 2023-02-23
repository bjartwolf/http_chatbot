module Tests

open System
open Xunit
open socket.core.TcpWrappers
open System.Threading
open System.Net
open System.IO

open ConnectionController 

type FakeClientWrapper () = 
   let mutable ms = new MemoryStream()
   interface ITcpClient with
       member this.Close(): unit = ()
       member this.GetStream(): IO.Stream = 
           ms 
   interface IDisposable with
       member this.Dispose() =
           ms.Dispose()

type FakeTcpListener(delay: int) =
   interface ITcpListener with
       member this.Stop(): unit = 
              ()
       member this.AcceptTcpClientAsync(ct: CancellationToken): Async<(ITcpClient*IPEndPoint) option> = 
              async {
                  do! Async.Sleep(delay) 
                  if (ct.IsCancellationRequested) then failwith "fooobar"
                  return Some (new FakeClientWrapper(), new IPEndPoint(1,1)) 
              }

       member this.Start(): unit = () 
 
[<Fact>]
let ``Connection controller returns a connection`` () =
    let cts = new CancellationTokenSource()
    let fakeListener = FakeTcpListener(0) 
    let server = ListenConnections (fakeListener, cts.Token) 
    let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
    let conn,ip = reply.Value
    Assert.Equal(1, ip.Port)
    cts.Cancel()

[<Fact>]
let ``Connection controller returns a connection even if we wait past message`` () =
    async {
        let cts = new CancellationTokenSource()
        let fakeListener = FakeTcpListener(0) 
        let server = ListenConnections (fakeListener, cts.Token) 
        do! Async.Sleep(60)
        let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
        let conn,ip = reply.Value
        Assert.Equal(1, ip.Port)
        cts.Cancel()
    }

[<Fact>]
let ``Connection failed to request within time returns none`` () =
    let cts = new CancellationTokenSource()
    let fakeListener = FakeTcpListener(50) 
    let server = ListenConnections (fakeListener, cts.Token) 
    let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
    Assert.Equal(None, reply)
    cts.Cancel()

[<Fact>]
let ``Wait 40 ms to send message should recieve value in the end `` () =
    async {
        let cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100))
        let fakeListener = FakeTcpListener(20) 
        let server = ListenConnections (fakeListener, cts.Token) 
        do! Async.Sleep(40)
        let rec loop() = 
            let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
            match reply with 
                        | Some (_, endpoint) -> Assert.Equal(1, endpoint.Port)
                        | _ -> loop() 
        loop()
        cts.Cancel()
    }

[<Fact>]
let ``If server stops we should recieve no message`` () =
    async {
        let cts = new CancellationTokenSource()
        cts.Cancel()
        let fakeListener = FakeTcpListener(50) 
        let server = ListenConnections (fakeListener, cts.Token) 
        let reply = server.TryPostAndReply((fun channel -> (GetNewConnection channel)),20)
        Assert.Equal(None, reply)
    }

