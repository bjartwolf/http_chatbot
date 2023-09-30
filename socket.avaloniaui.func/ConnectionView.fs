module ConnectionView

open Avalonia.FuncUI.DSL
open Model
open Avalonia.FuncUI
open Avalonia.Controls
open Messages
open Avalonia.FuncUI.Types

let connectionView (connections: Connection list) (dispatch: Msg -> unit) : IView<ListBox> =
            ListBox.create [
                ListBox.dock Dock.Left
                ListBox.dataItems connections 
                ListBox.itemTemplate (
                    DataTemplateView.create<_,_>(fun ((_,ip): Connection) -> 
                        TextBlock.create [
                        TextBlock.text (sprintf "%A" ip)
                        ]
                    )
                ) 
                ListBox.onSelectedItemChanged( fun (x) -> 
                   if x <> null then   
                       let conn = x :?> Connection
                       dispatch (ConnectionSelected conn))
                ListBox.selectionMode SelectionMode.AlwaysSelected
            ] 
