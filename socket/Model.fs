module Model
open Messages

type Model = {
    Connections: Connection list 
    SelectedItem: Connection option
    SelectedConnectionSent: string
    SelectedConnectionRecieved: string
    TextToSend: string
}



