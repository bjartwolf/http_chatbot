module ControlsView

open Avalonia.FuncUI.DSL
open Model
open Avalonia.Controls
open Messages
open Avalonia.Layout
open Avalonia.Input

let view (model:Model) (dispatch: Msg -> unit) =
    StackPanel.create [
        StackPanel.dock Dock.Bottom
        StackPanel.children [
            TextBox.create [
                TextBox.background "Gray"
                TextBox.horizontalAlignment HorizontalAlignment.Stretch
                TextBox.text model.TextToSend
                TextBox.onTextChanged (fun x -> dispatch(ChangeTextToSend x))
                TextBox.onKeyDown( fun x -> 
                    if x.Key = Avalonia.Input.Key.Enter then 
                        x.Handled <- true
                        dispatch SendText)
            ]
            Button.create [
                Button.onClick (fun _ -> dispatch CloseCurrent)
                Button.content "Close connection to current socket (escape)"
                Button.horizontalAlignment HorizontalAlignment.Stretch
                Button.hotKey (new KeyGesture (Key.Escape, KeyModifiers.None))
            ]                
            Button.create [
                Button.dock Dock.Bottom
                Button.onClick (fun _ -> dispatch SendText)
                Button.content "Send text to selected socket (Enter)"
                Button.horizontalAlignment HorizontalAlignment.Stretch
            ]                
        ]                
    ]                
