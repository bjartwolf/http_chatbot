module TcpMailboxTests
open System
open Xunit
open socket.core.TcpWrappers
open System.Threading
open System.Net
open System.IO

open TcpMailbox
type FakeClientWrapper () = 
   let mutable ms = new MemoryStream()
   member this.GetMs() = ms
   interface ITcpClient with
       member this.Close(): unit = ()
       member this.GetStream(): IO.Stream = 
           ms 
   interface IDisposable with
       member this.Dispose() =
           ms.Dispose()


[<Fact>]
let ``Write something returns the same and a newline`` () =
     async {
        let message = "foo"
        let fakeClient = new FakeClientWrapper()
        let foo = ListenMessages (fakeClient)
        foo.Post(MesssageToSend(message))
        
        do! Async.Sleep(10)
        let ms = fakeClient.GetMs() 
        ms.Position <- 0
        let tr = new StreamReader(ms)
        let foo = tr.ReadToEnd()
        Assert.Equal(foo, message+System.Environment.NewLine)
     }
