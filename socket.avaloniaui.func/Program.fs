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

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "Artisanal WebServer For Uniquely Handcrafted HTTP"
        base.Icon <- WindowIcon(System.IO.Path.Combine("Assets","Icons", "icon.ico"))
        base.Height <- 500.0
        base.Width <- 1000.0

        let subscriptions (_state: Model) : Sub<Msg> =
            let timerSub (dispatch: Msg -> unit) =
                let invoke() =
                    Msg.Tick |> dispatch
                    true

                DispatcherTimer.Run(Func<bool>(invoke), TimeSpan.FromMilliseconds 1000.0)

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
        Elmish.Program.mkProgram Update.init Update.update SocketViews.mainView
        |> Program.withHost this
        |> Program.withSubscription subscriptions
        |> Program.withConsoleTrace
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Default
        //this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            let mainWindow = MainWindow()
            desktopLifetime.MainWindow <- mainWindow
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
