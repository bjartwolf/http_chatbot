namespace Socket.AvaloniaUi 

module SocketViews = 
    open Avalonia.Controls
    open Model
    open Avalonia.FuncUI.DSL

    // https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Themes.Fluent/Accents/FluentControlResources.xaml
    let mainView (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                ControlsView.view model dispatch
                ConnectionView.view model.Connections dispatch
                RequestReplyView.view model.SelectedConnectionRecieved model.SelectedConnectionSent 
             ]
        ]
