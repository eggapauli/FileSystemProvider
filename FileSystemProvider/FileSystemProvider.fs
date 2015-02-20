namespace FileSystemProvider

open System.IO
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

[<TypeProvider>]
type FileSystemProvider(config: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces()

    let ns = "FSharp.TypeProvider"
    let thisAssembly = Assembly.GetExecutingAssembly()

    let parameters = [ ProvidedStaticParameter("path", typeof<string>) ]

    let t = ProvidedTypeDefinition(thisAssembly, ns, "FileSystemProvider", Some typeof<obj>)

    let buildTypes typeName (args: obj[]) =
        let path = args.[0] :?> string |> Path.GetFullPath
        let ty = ProvidedTypeDefinition(thisAssembly, ns, typeName, Some typeof<obj>)

        let rec getProperties (dir: string) () =
            let dirMembers =
                Directory.GetDirectories dir
                |> Seq.map (fun subDir ->
                    let subType = ProvidedTypeDefinition(Path.GetFileName subDir, Some typeof<obj>)
                    subType.AddMember (ProvidedLiteralField("Path", typeof<string>, subDir))
                    subType.AddMembersDelayed (getProperties subDir)
                    subType)
                |> Seq.cast<MemberInfo>

            let fileMembers =
                Directory.GetFiles dir
                |> Seq.map (fun file -> ProvidedProperty(Path.GetFileName file, typeof<FileInfo>, GetterCode = (fun args -> <@@ FileInfo file @@>), IsStatic = true))
                |> Seq.cast<MemberInfo>

            Seq.append dirMembers fileMembers
            |> Seq.toList

        ty.AddMembersDelayed (getProperties path)

        ty

    do t.DefineStaticParameters(parameters, buildTypes)

    do this.AddNamespace(ns, [ t ])

[<assembly:TypeProviderAssembly>]
do()