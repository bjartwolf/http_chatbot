open Terminal.Gui
open System
open NStack
open System.Net
open TcpMailbox
open ConnectionController
open System.Threading
open System.Net.Sockets
open socket.core.TcpWrappers

let ustr (x: string) = ustring.Make(x)

Application.Init();
let top = Application.Top;
Views.setupViews top
let inputMenu = new Window(X = Pos.At 0, Y = Pos.At 1, Width = Dim.Fill (), Height = 4)

top.Add(inputMenu)
let button = new Button(X=0,Y=1, Width = 7, Text="Send")
let close = new Button(X=10,Y=1, Width = 7, Text="Close")
button.IsDefault <- true
button.Enabled <- true

inputMenu.Add button
inputMenu.Add close
let input = new TextField(
    X = 0,
    Y = 0,
    Width = Dim.Fill(1),
    Text = ustr "")

inputMenu.Add input

let connections = 
    new Window (
        ustr ("Connections"),
        X = Pos.At 0,
        Y = Pos.At 5,
        Width = 20,
        Height = Dim.Fill ())

let activeConnection = 
    new Window (
        ustr ("Active Connection"),
        X = Pos.At 20,
        Y = Pos.At 5,
        Width = Dim.Fill (),
        Height = Dim.Fill ())
top.Add connections

let scrollClient = new ScrollView(X = 0, Y=0, Width = Dim.Fill(1), Height=Dim.Percent(float32 50.0))
scrollClient.ContentSize <- new Size(160, 100)
let activeClient = new TextView(X = 0, Y=0, Width = Dim.Fill(1), Height=Dim.Fill(1))
activeClient.ReadOnly <- true
activeClient.CanFocus <- false
scrollClient.Add activeClient
activeConnection.Add scrollClient

let scrollServer = new ScrollView(X = 0, Y=15, Width = Dim.Fill(1), Height=Dim.Percent(float32 50.0))
scrollServer.ContentSize <- new Size(160, 100)
let activeServer = new TextView(X = 0, Y=0, Width = Dim.Fill(1), Height=Dim.Fill(1))
activeServer.ReadOnly <- true
activeServer.CanFocus <- false
scrollServer.Add activeServer

activeConnection.Add scrollServer 
top.Add(activeConnection)

let connectionList : IPEndPoint ResizeArray = ResizeArray []

let leftMenu: ListView = new ListView(connectionList)
leftMenu.Width <- 20
leftMenu.Height <- Dim.Fill(1)
leftMenu.AllowsMultipleSelection <- false
connections.Add leftMenu
type Connection = { client: MailboxProcessor<lineFeed>;
                    remoteEndpoint: IPEndPoint;
                    input: ustring;
                    output: ustring }

let mutable mailboxes: MailboxProcessor<lineFeed> list= [] 
let mutable selectedConnection: int option = None
let cts = new CancellationTokenSource()
let port = 13001;
let localAddr = IPAddress.Parse("127.0.0.1");
let ep = new IPEndPoint(localAddr, port);

let tcpListener : ITcpListener = new TcpListenerWrapper(new TcpListener(ep)) 

let server = ListenConnections(tcpListener, cts.Token)
let listenHandle = Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(20), 
    fun mainloop -> mainloop.Invoke(
                fun () -> 
                    let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
                    match reply with 
                        | Some (client, iPEndPoint) ->
                                            let clientWithId = { client =  ListenMessages client;
                                                                 remoteEndpoint = iPEndPoint;
                                                                 input = ustr "";
                                                                 output = ustr ""
                                                               }
                                            connectionList.Add(clientWithId.remoteEndpoint)
                                            mailboxes <- clientWithId.client :: mailboxes
                                            selectedConnection <- Some (mailboxes.Length - 1)
                                            leftMenu.SelectedItem <- mailboxes.Length - 1
                        | None -> ());true)
                    

let ct2 = Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(20), 
    fun mainloop -> mainloop.Invoke(
        fun () -> 
            match selectedConnection with
                | None -> ()
                | Some i -> let selectedMailbox = mailboxes.[i] 
                            let (sentdata,status)= selectedMailbox.PostAndReply(fun channel -> (GetRecieved channel))
                            activeClient.Text <- ustr (sprintf "%s and stream is %A" sentdata status)
                            let (receiveddata, status2) = selectedMailbox.PostAndReply(fun channel -> (GetSent channel))
                            activeServer.Text <- ustr (sprintf "%s and stream is %A" receiveddata status2));true)


button.add_Clicked(fun _ -> 
            match selectedConnection with
                | None -> ()
                | Some i -> let selectedMailbox = mailboxes.[i] 
                            selectedMailbox.Post(TcpMailbox.MesssageToSend (string input.Text)); 
                            input.Text <- ustr ""
                            input.SetFocus())

close.add_Clicked(fun _ ->
            match selectedConnection with
                | None -> ()
                | Some i -> let selectedMailbox = mailboxes.[i] 
                            selectedMailbox.Post(TcpMailbox.Close))

leftMenu.add_SelectedItemChanged( fun conn -> selectedConnection <- Some conn.Item )
Application.Run()

Application.Shutdown();