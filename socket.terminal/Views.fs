module Views
open Terminal.Gui
open NStack

let ustr (x: string) = ustring.Make(x)
let setupViews (top:Toplevel) :unit =
    let Quit () =
        MessageBox.Query (50, 7, ustr "Quit Demo", ustr "Are you sure you want to quit the TCP chatbot?", ustr "Yes", ustr "No") = 0

    let mutable menu = Unchecked.defaultof<MenuBar>
    menu <- new MenuBar (
                [| MenuBarItem(ustr "_File",
                        [| 
                           MenuItem (ustr "_Quit", ustring.Empty, (fun () -> if Quit() then top.Running <- false)) |])
                                               |])
    top.Add(menu);

