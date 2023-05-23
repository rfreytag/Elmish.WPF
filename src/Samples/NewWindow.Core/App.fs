module Elmish.WPF.Samples.NewWindow.AppModule

open System.Windows

open Elmish.WPF

open Window1Module
open Window2Module


type App =
  { Window1: WindowState<string>
    Window2: Window2 option }

type AppMsg =
  | Window1Show
  | Window1Hide
  | Window1Close
  | Window1SetInput of string
  | Window2Show
  | Window2Close
  | Window2Msg of Window2Msg


module App =
  module Window1 =
    let get (m: App) : WindowState<string> =
      m.Window1
    let set (v: WindowState<string>) (m: App) : App =
      { m with Window1 = v }
    let map : ((WindowState<string> -> WindowState<string>) -> App -> App) =
      map get set
  module Window2 =
    let get (m: App) : Window2 option =
      m.Window2
    let set (v: Window2 option) (m: App) : App =
      { m with Window2 = v }
    let map_Window2 : ((Window2 option -> Window2 option) -> App -> App) =
      map get set
    let mapOutMsg (msg: Window2OutMsg) : AppMsg =
      match msg with
      | Window2OutMsg.Close -> Window2Close
    let mapInOutMsg : InOut<Window2Msg,Window2OutMsg> -> AppMsg =
      InOut.cata Window2Msg mapOutMsg

let init =
  { Window1 = WindowState.Closed
    Window2 = None }

let update (msg: AppMsg) : (App -> App) =
  match msg with
  | Window1Show -> "" |> WindowState.toVisible |> App.Window1.map
  | Window1Hide -> "" |> WindowState.toHidden  |> App.Window1.map
  | Window1Close -> WindowState.Closed |> App.Window1.set
  | Window1SetInput s -> s |> WindowState.set |> App.Window1.map
  | Window2Show -> Window2.init |> Some |> App.Window2.set
  | Window2Close -> None |> App.Window2.set
  | Window2Msg msg -> msg |> Window2.update |> Option.map |> App.Window2.map_Window2

let bindings (createWindow1: unit -> #Window) (createWindow2: unit -> #Window) () : Binding<App,AppMsg> list = [
  "Window1Show" |> Binding.cmd Window1Show
  "Window1Hide" |> Binding.cmd Window1Hide
  "Window1Close" |> Binding.cmd Window1Close
  "Window2Show" |> Binding.cmd Window2Show
  "Window1" |> Binding.subModelWin(
    (fun m -> m.Window1),
    snd,
    id,
    Window1.bindings >> Bindings.mapMsg Window1SetInput,
    createWindow1)
  "Window2" |> Binding.subModelWin(
    App.Window2.get >> WindowState.ofOption,
    snd,
    App.Window2.mapInOutMsg,
    Window2.bindings,
    createWindow2,
    isModal = true)
]

let private fail _ = failwith "never called"
let designVm : obj = ViewModel.designInstance init (bindings fail fail ())