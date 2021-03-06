#load @".paket/load/netstandard2.0/main.group.fsx"

open System
open Akka.Actor
open Akkling

let system = System.create "my-system" <| Configuration.defaultConfig()

type Command = 
    | CreateActor of string
    | ActorCommand of string * string

let workerActor (mailbox:Actor<_>) =
    let actorId = mailbox.Self.Path.Name
    let rec loop commands =
        actor {
            printfn "Actor %s has commands history: [%s]" actorId <| String.concat "|" commands
            let! message = mailbox.Receive ()

            match message with
            | "null" -> raise <| ArgumentNullException()
            | cmd when cmd.StartsWith "-" -> raise<| ArgumentOutOfRangeException()
            | cmd when Char.IsPunctuation(cmd.[0]) -> raise <| ArgumentException()
            | _ -> ()

            return! loop (message :: commands)
        }

    loop ([])

let supervisingActor (mailbox:Actor<_>) =
    let rec loop () =
        actor {
            let! message = mailbox.Receive ()
            
            match message with
            | CreateActor actorId -> 
                spawn mailbox actorId <| props workerActor |> ignore
            | ActorCommand (actorId, cmd) ->
                let actor = select mailbox actorId
                actor <! cmd

            return! loop ()
        }

    loop()

let strategy () = 
    Strategy.OneForOne((fun ex ->
    printfn "Invoking supervision strategy"
    match ex with 
    | :? ArgumentNullException -> 
        printfn "Stopping actor"
        Directive.Stop
    | :? ArgumentOutOfRangeException -> 
        printfn "Restarting actor"
        Directive.Restart
    | :? ArgumentException -> 
        printfn "Resuming actor"
        Directive.Resume
    | _ -> Directive.Escalate), 3, TimeSpan.FromSeconds(10.))

let supervisor = spawn system "runner" <| { props supervisingActor with SupervisionStrategy = Some (strategy ()) }

supervisor <! CreateActor "1"
supervisor <! CreateActor "2"

supervisor <! ActorCommand ("1", "1")
supervisor <! ActorCommand ("2", "1")
supervisor <! ActorCommand ("2", "-5")
supervisor <! ActorCommand ("2", "2")



