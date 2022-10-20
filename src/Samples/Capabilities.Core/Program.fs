module Elmish.WPF.Samples.Capabilities.Program

open Serilog
open Serilog.Extensions.Logging

open Elmish.WPF

open AppModule

let main window =

  let logger =
    LoggerConfiguration()
      .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Verbose)
      .WriteTo.Console()
      .CreateLogger()

  let createWindow1 () = createWindow1.Invoke()
  let createWindow2 () =
    let window = createWindow2.Invoke()
    window.Owner <- mainWindow
    window

  let bindings = App.bindings createWindow1 createWindow2

  WpfProgram.mkSimple (fun () -> App.init) App.update bindings
  |> WpfProgram.withLogger (new SerilogLoggerFactory(logger))
  |> WpfProgram.startElmishLoop window