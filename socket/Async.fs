namespace System
open System
open System.Threading.Tasks

[<AutoOpen>]
module Async =
    type Async with
        static member AwaitTaskCorrect(task : Task) : Async<unit> =
            Async.FromContinuations(fun (sc,ec,cc) ->
                task.ContinueWith(fun (task:Task) ->
                    if task.IsFaulted then
                        let e = task.Exception
                        if e.InnerExceptions.Count = 1 then ec e.InnerExceptions.[0]
                        else ec e
                    elif task.IsCanceled then
                        ec(TaskCanceledException())
                    else
                        sc ())
                |> ignore)

        static member AwaitTaskCorrect(task : Task<'T>) : Async<'T> =
            Async.FromContinuations(fun (sc,ec,cc) ->
                task.ContinueWith(fun (task:Task<'T>) ->
                    if task.IsFaulted then
                        let e = task.Exception
                        if e.InnerExceptions.Count = 1 then ec e.InnerExceptions.[0]
                        else ec e
                    elif task.IsCanceled then
                        ec(TaskCanceledException())
                    else
                        sc task.Result)
                |> ignore)
