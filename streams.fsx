#load @".paket/load/net461/main.group.fsx"

open System
open Akkling
open Akkling.Streams

let system = System.create "my-system" <| Configuration.defaultConfig()
let mat = system.Materializer()

// Source: https://github.com/Horusiath/Akkling/blob/master/examples/streams.fsx

let text = """
       Lorem Ipsum is simply dummy text of the printing and typesetting industry.
       Lorem Ipsum has been the industry's standard dummy text ever since the 1500s,
       when an unknown printer took a galley of type and scrambled it to make a type
       specimen book."""

Source.ofArray (text.Split())
|> Source.map (fun x -> x.ToUpper())
|> Source.filter (String.IsNullOrWhiteSpace >> not)
|> Source.runForEach mat (printfn "%s")
|> Async.RunSynchronously
