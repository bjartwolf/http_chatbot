module ConfigParser

open Argu

type Arguments =
    | [<Mandatory>] Port of port:uint16
    | Ip of string
    | Darkmode
    | Insecure 
    | CertPemFilePath of string
    | KeyPemFilePath of string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Port _ -> "specify a port, for example 13001"
            | Ip _ -> "IP address"
            | Darkmode -> "Enable darkmode"
            | Insecure -> "Run in HTTP mode" 
            | CertPemFilePath _ -> "Absolute path to PEM certificate"
            | KeyPemFilePath _ -> "Absolute ath to PEM key file"

