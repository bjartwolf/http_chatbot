module Model

open System.Net
open TcpMailbox

type Connection = MailboxProcessor<lineFeed>*IPEndPoint

type Model = {
    Connections: Connection list 
    SelectedItem: Connection option
    SelectedConnectionSent: string
    SelectedConnectionRecieved: string
    TextToSend: string
}
