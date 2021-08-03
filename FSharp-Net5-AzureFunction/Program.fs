module Program

open System.Net
open Microsoft.Extensions.Hosting
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http

type TestJson = {
    A: string
    B: int
    C: double}

let lstJson:TestJson list = [
    {A="a"; B=1 ;C=1.1}
    {A="b"; B=2 ;C=2.2}
    {A="c"; B=3 ;C=3.3} 
   ]

let stringResponse (str,(req: HttpRequestData))  = 
  let r = req.CreateResponse(HttpStatusCode.OK)
  r.WriteString(str)
  r
  
let jsonResponse (a,(req: HttpRequestData))  =
  async {
    let r = req.CreateResponse(HttpStatusCode.OK)
    do! (r.WriteAsJsonAsync a).AsTask()  |> Async.AwaitTask
    return r
  } |> Async.StartAsTask


[<Function("Test1")>]
let test1 ([<HttpTrigger(AuthorizationLevel.Function, "GET", Route = "test1")>] req: HttpRequestData) =
  async {
    return stringResponse("Hello World", req)
  }
  |> Async.StartAsTask


[<Function("Test2")>]
let test2 ([<HttpTrigger(AuthorizationLevel.Function, "GET", Route = "test2")>] req: HttpRequestData) = 
  jsonResponse(lstJson, req)

[<EntryPoint>]
let main argv =
  async {
    do! Async.SwitchToThreadPool ()

    let host = HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build()

    return! host.RunAsync() |> Async.AwaitTask
  }
  |> Async.RunSynchronously

  0
