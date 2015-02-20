#r @"C:\Users\Hannes\Workspace\FileSystemProvider\FileSystemProvider\bin\Debug\FileSystemProvider.dll"

type Desktop = FSharp.TypeProvider.FileSystemProvider<"C:\Users\Hannes\Desktop\FileSystemProviderTest">
printfn "%O" Desktop.``test 1``.Path
printfn "%O" Desktop.``test 1.txt``