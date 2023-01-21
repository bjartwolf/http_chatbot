module Counter

open Terminal.Gui.Elmish

type Model = {
    IsSpinning: bool
}

type Msg =
    | Increment

let init () : Model * Cmd<Msg> =
    let model = {
        IsSpinning = false
    }
    model, Cmd.none



let update (msg:Msg) (model:Model) =
    model, Cmd.none

let view (model:Model) (dispatch:Msg->unit) =
    View.page [
        page.menuBar [
        ]
    ]


Program.mkProgram init update view  
|> Program.run

