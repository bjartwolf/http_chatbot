module Counter

open Terminal.Gui
open Terminal.Gui.Elmish
open System

type Model = {
    Counter:int
    IsSpinning: bool
}

type Msg =
    | Increment
    | Decrement
    | Reset
    | StartSpin
    | StopSpin
    | Spinned

let init () : Model * Cmd<Msg> =
    let model = {
        Counter = 0
        IsSpinning = false
    }
    model, Cmd.none


module Commands =
    let startSpinning isSpinning =
        fun dispatch ->
            async {
                do! Async.Sleep 20
                if isSpinning then
                    dispatch Increment
                    dispatch Spinned
            }
            |> Async.StartImmediate
        |> Cmd.ofSub

let update (msg:Msg) (model:Model) =
    match msg with
    | Increment ->
        {model with Counter = model.Counter + 1}, Cmd.none
    | Decrement ->
        {model with Counter = model.Counter - 1}, Cmd.none
    | Reset ->
        {model with Counter = 0}, Cmd.none
    | StartSpin ->
        {model with IsSpinning = true}, Commands.startSpinning true
    | StopSpin ->
        {model with IsSpinning = false}, Cmd.none
    | Spinned ->
        model, Commands.startSpinning model.IsSpinning
        


let view (model:Model) (dispatch:Msg->unit) =
    View.page [
        page.menuBar [
            menubar.menus [
                menu.menuBarItem [
                    menu.prop.title "Menu 1"
                    menu.prop.children [
                        menu.submenuItem [
                            menu.prop.title "Sub Menu 1"
                            menu.prop.children [
                                menu.menuItem ("Sub Item 1", (fun () -> System.Diagnostics.Debug.WriteLine($"Sub menu 1 triggered")))
                                menu.menuItem [
                                    menu.prop.title "Sub Item 2"
                                    menu.item.action (fun () -> System.Diagnostics.Debug.WriteLine($"Sub menu 2 triggered"))
                                    menu.item.itemstyle.check
                                    menu.item.isChecked true
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
        prop.children [
            View.label [
                prop.position.x.center
                prop.position.y.at 1
                prop.textAlignment.centered
                prop.color (Color.BrightYellow, Color.Green)
                label.text "'F#ncy' Counter!"
            ] 

            View.button [
                prop.position.x.center
                prop.position.y.at 5
                label.text "Up"
                button.onClick (fun () -> dispatch Increment)
            ] 

            View.label [
                let c = (model.Counter |> float) / 100.0
                let x = (16.0 * Math.Cos(c)) |> int 
                let y = (8.0 * Math.Sin(c)) |> int

                prop.position.x.at (x + 20)
                prop.position.y.at (y + 10)
                prop.textAlignment.centered
                prop.color (Color.Magenta, Color.BrightYellow)
                label.text $"The Count of 'Fancyness' is {model.Counter}"
            ] 


            View.button [
                prop.position.x.center
                prop.position.y.at 7
                label.text "Down"
                button.onClick (fun () -> dispatch Decrement)
            ] 

            View.button [
                prop.position.x.center
                prop.position.y.at 9
                label.text "Start Spinning"
                button.onClick (fun () -> dispatch StartSpin)
            ] 

            View.button [
                prop.position.x.center
                prop.position.y.at 11
                label.text "Stop Spinning"
                button.onClick (fun () -> dispatch StopSpin)
            ] 

            View.button [
                prop.position.x.center
                prop.position.y.at 13
                label.text "Reset"
                button.onClick (fun () -> dispatch Reset)
            ]
        ]
    ]


Program.mkProgram init update view  
|> Program.run

