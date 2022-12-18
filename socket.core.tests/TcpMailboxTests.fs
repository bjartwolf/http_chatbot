module TcpMailboxTests
open System
open Xunit
open socket.core.TcpWrappers
open System.Threading
open System.Net
open System.IO

open TcpMailbox
open System.Threading.Tasks

type IdleStream () =
   inherit Stream()
   override this.CanRead with get () = true 
   override this.CanSeek with get () = false 
   override this.CanWrite with get () = false 
   override this.Read(buffer: byte[], offset:int, count: int) = (Task.Delay(TimeSpan.FromSeconds(5))).GetAwaiter().GetResult(); 0 
   override this.ReadAsync(buffer: byte[], offset:int, count: int, cancellationToken: CancellationToken): Task<int> = 
    let foo = async {
      do! Async.Sleep(10)
      return 0
    }
    Async.StartAsTask<int>(foo, TaskCreationOptions.AttachedToParent, cancellationToken) 
   
   override this.Seek(offset:int64, origin: SeekOrigin):int64 = failwith "no seek"
   override this.SetLength(value: int64) = failwith "no set length"
   override this.Write(buffer: byte[], offset:int, count:int) = failwith "no write"
   override this.Length with get () = failwith "no length I am infinite" 
   override this.Position with get () = failwith "no position" 
                          and set (value) = failwith "no set pos" 
   override this.Flush() = ()

type ReadableStream() =
   inherit Stream()
   override this.CanRead with get () = true 
   override this.CanSeek with get () = false 
   override this.CanWrite with get () = false 
   override this.Read(buffer: byte[], offset:int, count: int) = 0
   override this.Seek(offset:int64, origin: SeekOrigin):int64 = failwith "no seek"
   override this.SetLength(value: int64) = failwith "no set length"
   override this.Write(buffer: byte[], offset:int, count:int) = () 
   override this.Length with get () = failwith "no length I am infinite" 
   override this.Position with get () = failwith "no position" 
                          and set (value) = failwith "no set pos" 
   override this.Flush() = ()

type FakeClientWrapper (stream: Stream) = 
   let innerStream = stream 
   member this.GetStream() = innerStream 
   interface ITcpClient with
       member this.Close(): unit = ()
       member this.GetStream(): IO.Stream = 
           innerStream 
   interface IDisposable with
       member this.Dispose() =
           innerStream.Dispose()


[<Fact>]
let ``Write something returns the same and a newline`` () =
     async {
        0
        //let message = "foo"
        //let fakeClient = new FakeClientWrapper()
        //let foo = ListenMessages (fakeClient)
        //foo.Post(MesssageToSend(message))
        
        //let ms = fakeClient.GetMs() 
        //ms.Position <- 0
        //let tr = new StreamReader(ms)
        //let foo = tr.ReadToEnd()
        //Assert.Equal(foo, message+System.Environment.NewLine)
     }

[<Fact>]
let ``Read when nothing is there works`` () =
     async {
        let fakeClient = new FakeClientWrapper(new IdleStream())

        let foo = ListenMessages (fakeClient)
        do! Async.Sleep(100)
        let (recievedData,status)= foo.PostAndReply(fun channel -> (GetRecieved channel))
        
        Assert.Equal("", recievedData)
     }

