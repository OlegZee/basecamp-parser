// Learn more about F# at http://fsharp.org

open System
open FSharp.Data
open System.Text.RegularExpressions
open System.IO

type Basecamp = JsonProvider<"../todos.json">

let grabEmail s =
    Regex.Matches(s, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value)
    |> Seq.tryHead

let grabName (s:string) =
    match s.IndexOf("(") with
    | p when p > 0 -> s.Substring(0, p).Trim();
    | _ -> s

[<EntryPoint>]
let main argv =

    let selectedSpeakers = [
        "Тоболь"
        "Панченко Иван Евгеньевич"
        "Фастов"
        "Чечель"
        "Чапоргин"
        "Неволин"
        "Кочепасов"
        "Абилов Вагиф"
        "Ярийчук"
        "Кирпичников"
        "Аршинов"
        "Dylan Beattie"
        "Валеев Тагир"
        "Финкельштейн Павел"
        "Александров Дмитрий"
        "Липский Никита"
        "Кошелев Григорий"
        "Паньгин"
        "Толстопятов"
        "Плизга"
    ]

    // https://3.basecamp.com/3152562/buckets/8281413/todolists/1303394617/todos.json?completed=true
    // https://3.basecamp.com/3152562/buckets/8281413/todolists/1303394617/todos.json?completed=true&page=2
    // https://3.basecamp.com/3152562/buckets/8281413/todolists/1303394617/groups.json
    // https://github.com/basecamp/bc3-api/blob/master/sections/todolist_groups.md#to-do-list-groups

    let readTodos path =
      [ for file in Directory.GetFiles path do
           for issue in Basecamp.Load file do
               let name = grabName issue.Title
               if not(selectedSpeakers |> List.exists (name.Contains)) then
                   let email = grabEmail issue.Description |> Option.defaultValue ""
                   yield name + "," + email ]

    let todos = readTodos "C:\\Codefest\\Data" |> List.distinct

    File.AppendAllLines("c:\\Codefest\\decline.csv", todos)
    for issue in todos do
        printfn "%s" issue
    Console.ReadLine() |> ignore
    0 