namespace Socket.AvaloniaUi 

open Avalonia
open Avalonia.Themes.Fluent
open Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.Controls.ApplicationLifetimes
open ConnectionController

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "Showcase components"
        base.Height <- 1000
        base.Width <- 1000.0

        let server: MailboxProcessor<ConnectionMsg> = MailboxProcessor<ConnectionMsg>.Start( fun _ ->  async {()})

        Elmish.Program.mkProgram FakeUpdate.init (Update.update server) DesignSystem.mainView
        |> Program.withHost this
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

