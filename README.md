# .NET Fest Akkling

Code samples for the presentation "Akka + F# = Akkling" at .NET Fest, Kiev 2018.

The samples are written and tested using Visual Studio Code on Mac/mono

Run the following command from Terminal before running samples

Mono: 
mono .paket/paket.exe install
mono .paket/paket.exe generate-load-scripts --group Main --framework netstandard2.0 --type fsx

Windows:
.paket\paket.exe install
.paket\paket.exe generate-load-scripts --group Main --framework net461 --type fsx
