namespace Socket.AvaloniaUi 

open Avalonia
open Avalonia.Themes.Fluent
open Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.Controls.ApplicationLifetimes
open Messages
open Model
open Avalonia.Threading
open System
open Avalonia.Controls
open Argu
open ConfigParser
open socket.terminal
open Server
open System.Net
open ConnectionController


type MainWindow(config: Config) as this =
    inherit HostWindow()
    do
        base.Title <- "Artisanal WebServer For Uniquely Handcrafted HTTP"
        base.Icon <- WindowIcon(System.IO.Path.Combine("Assets","Icons", "HTTP.ico"))
        // Option to override font with command line 
        //base.FontFamily <- Avalonia.Media.FontFamily("Fira Code") 
        base.FontFamily <- Avalonia.Media.FontFamily("avares://socket.avaloniaui.funcui.elmish/Assets/Fonts/Go-Mono.ttf#Go Mono") 
        printfn "Listening to IP %A at port %A" config.ipAddress config.port
        if (config.insecure) then printfn "Insecure mode, only HTTP"
        base.Height <- 500.0
        base.Width <- 1000.0

        let subscriptions (_state: Model) : Sub<Msg> =
            let timerSub (dispatch: Msg -> unit) =
                let invoke() =
                    Msg.Tick |> dispatch
                    true

                DispatcherTimer.Run(Func<bool>(invoke), TimeSpan.FromMilliseconds 100.0)

            let onClosedSub (dispatch: Msg -> unit) =
                this.Closed.Subscribe(fun e ->
                    printfn "The window has been closed."
                )

            [
                [ nameof timerSub ], timerSub
                [ nameof onClosedSub ], onClosedSub
            ]
        //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
        //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true

        let server: MailboxProcessor<ConnectionMsg> = listeningServer config 
        Elmish.Program.mkProgram (Update.init server) (Update.update server) SocketViews.mainView
        |> Program.withHost this
        |> Program.withSubscription subscriptions
//        |> Program.withConsoleTrace
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Default

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.Startup.Add(fun (args: ControlledApplicationLifetimeStartupEventArgs) -> 
                let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
                let parser = ArgumentParser.Create<Arguments>(programName = "httpchatbot.exe", errorHandler=errorHandler)
                let results = parser.Parse args.Args 
                if (results.Contains Darkmode) then this.RequestedThemeVariant <- Styling.ThemeVariant.Dark
                let config: Config = { ipAddress = IPAddress.Parse(results.GetResult(Ip, "127.0.0.1"));
                                       port = results.GetResult Port;
                                       certPemFilePath = results.GetResult (CertPemFilePath, "");
                                       keyPemFilePath = results.GetResult (KeyPemFilePath, "");
                                       insecure = results.Contains Insecure }
                
                let mainWindow = MainWindow(config)
                desktopLifetime.MainWindow <- mainWindow
                ()) 
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
