#load @".paket/load/net461/main.group.fsx"

open System
open Akkling

let system = System.create "my-system" <| Configuration.defaultConfig()

type Message =
  | Greet of string
  | Hi

let greetingActor = 
  spawnAnonymous system 
  <| props (fun mailbox -> 
    let rec loop () = actor {
      let! message = mailbox.Receive()
      match message with
      | Greet name -> printfn "Hello %s" name
      | Hi -> printfn "Hello from F#!"
      return! loop ()
    }
    loop ())

greetingActor <! Hi
greetingActor <! Greet "Paul"


let handler message =
    match message with
    | Greet(name) -> printfn "Hello %s" name
    | Hi -> printfn "Hello from F#!"
    |> ignored

let greetingActor2 = spawn system "actor2" <| props (actorOf handler)

greetingActor2 <! Hi
greetingActor2 <! Greet "Jane"


let accumulator = 
  spawnAnonymous system 
  <| props (fun mailbox -> 
    let rec loop sum = actor {
      let! message = mailbox.Receive()
      let newSum = sum + message
      printfn "Sum = %d" newSum
      return! loop newSum
    }
    loop 0)

accumulator <! 1
