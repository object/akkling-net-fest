#load @".paket/load/netstandard2.0/main.group.fsx"

open System
open Akkling
open Akkling.Persistence

// Source: https://github.com/Horusiath/Akkling/blob/master/examples/persistence.fsx

type CounterChanged =
    { Delta : int }

type CounterCommand =
    | Inc
    | Dec
    | GetState

type CounterMessage =
    | Command of CounterCommand
    | Event of CounterChanged

let system = System.create "my-system" <| Configuration.defaultConfig()
let counter =
    spawn system "counter-1" <| propsPersist(fun mailbox ->
        let rec loop state =
            actor {
                let! msg = mailbox.Receive()
                match msg with
                | Event(changed) -> 
                  printfn "Received event %A" changed
                  return! loop (state + changed.Delta)
                | Command(cmd) ->
                    printfn "Received command %A" cmd
                    match cmd with
                    | GetState ->
                        mailbox.Sender() <! state
                        return! loop state
                    | Inc -> return Persist (Event { Delta = 1 })
                    | Dec -> return Persist (Event { Delta = -1 })
            }
        loop 0)

counter <! (Command Inc)
let state : int = counter <? (Command GetState) |> Async.RunSynchronously

