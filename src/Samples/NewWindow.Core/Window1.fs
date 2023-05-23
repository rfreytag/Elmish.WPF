﻿module Elmish.WPF.Samples.NewWindow.Window1Module

open Elmish.WPF


module Window1 =
  let init = ""

  let bindings unit : Binding<'b,'b> list = [
    "Input" |> Binding.twoWay (id, id)
  ]

let designVm : obj = ViewModel.designInstance Window1.init (Window1.bindings ())