// include Fake libs
#I @"packages\FAKE\tools\"
#r @"packages\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open System.IO
open Fake.Paket

//Project config
let projectName = "Xrm.Oss.Interfacing"
let projectDescription = "A Dynamics CRM / Dynamics365 template for messaging interfaces using RabbitMQ"
let authors = ["Florian Kroenert"]

// Directories
let buildDir  = @".\build\"
let interfaceBuildDir = buildDir + @"interface\"
let crmConsumerBuildDir = interfaceBuildDir + @"crmConsumer";
let crmListenerBuildDir = interfaceBuildDir + @"crmListener";
let crmPublisherBuildDir = interfaceBuildDir + @"crmPublisher";
let demoCrmPublisherBuildDir = interfaceBuildDir + @"demoCrmPublisher";
let domainBuildDir = interfaceBuildDir + @"domain";
let thirdPartyConsumerBuildDir = interfaceBuildDir + @"thirdPartyConsumer";
let thirdPartyPublisherBuildDir = interfaceBuildDir + @"thirdPartyPublisher";
let workflowActivityBuildDir = interfaceBuildDir + @"workflowActivity";
let testDir   = @".\test\"

let deployDir = @".\Publish\"
let interfaceDeployDir = deployDir + @"interface\"
let crmConsumerDeployDir = interfaceDeployDir + @"crmConsumer";
let crmListenerDeployDir = interfaceDeployDir + @"crmListener";
let crmPublisherDeployDir = interfaceDeployDir + @"crmPublisher";
let demoCrmPublisherDeployDir = interfaceDeployDir + @"demoCrmPublisher";
let domainDeployDir = interfaceDeployDir + @"domain";
let thirdPartyConsumerDeployDir = interfaceDeployDir + @"thirdPartyConsumer";
let thirdPartyPublisherDeployDir = interfaceDeployDir + @"thirdPartyPublisher";
let workflowActivityDeployDir = interfaceDeployDir + @"workflowActivity";

let nugetDir = @".\nuget\"
let packagesDir = @".\packages\"

// version info
let mutable majorversion    = "1"
let mutable minorversion    = "0"
let mutable build           = buildVersion
let mutable nugetVersion    = ""
let mutable asmVersion      = ""
let mutable asmInfoVersion  = ""

// Targets
Target "Clean" (fun _ ->

    CleanDirs [buildDir; testDir; deployDir; nugetDir]
)

Target "BuildVersions" (fun _ ->
    asmVersion      <- majorversion + "." + minorversion + "." + build
    asmInfoVersion  <- asmVersion

    let nugetBuildNumber = if not isLocalBuild then build else "0"
    
    nugetVersion    <- majorversion + "." + minorversion + "." + nugetBuildNumber

    SetBuildNumber nugetVersion   // Publish version to TeamCity
)

Target "AssemblyInfo" (fun _ ->
    BulkReplaceAssemblyInfoVersions "src" (fun f -> 
                                              {f with
                                                  AssemblyVersion = asmVersion
                                                  AssemblyInformationalVersion = asmInfoVersion
                                                  AssemblyFileVersion = asmVersion})
)

Target "BuildCrmConsumer" (fun _ ->
    !! @"src\interface\Xrm.Oss.CrmConsumer\*.csproj"
        |> MSBuildRelease crmConsumerBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildCrmListener" (fun _ ->
    !! @"src\interface\Xrm.Oss.CrmListener\*.csproj"
        |> MSBuildRelease crmListenerBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildCrmPublisher" (fun _ ->
    !! @"src\interface\Xrm.Oss.CrmPublisher\*.csproj"
        |> MSBuildRelease crmPublisherBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildDemoCrmPublisher" (fun _ ->
    !! @"src\interface\Xrm.Oss.DemoPublisher\*.csproj"
        |> MSBuildRelease demoCrmPublisherBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildDomain" (fun _ ->
    !! @"src\domain\Xrm.Oss.Interfacing.Domain\*.csproj"
        |> MSBuildRelease domainBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildWorkflowActivity" (fun _ ->
    !! @"src\plugin\Xrm.Oss.WorkflowActivities\*.csproj"
        |> MSBuildRelease workflowActivityBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildThirdPartyConsumer" (fun _ ->
    !! @"src\interface\Xrm.Oss.ThirdPartyConsumer\*.csproj"
        |> MSBuildRelease thirdPartyConsumerBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildThirdPartyPublisher" (fun _ ->
    !! @"src\interface\Xrm.Oss.ThirdPartyPublisher\*.csproj"
        |> MSBuildRelease thirdPartyPublisherBuildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildTest" (fun _ ->
    !! @"src\test\**\*.csproj"
      |> MSBuildDebug testDir "Build"
      |> Log "Build Log: "
)

Target "Publish" (fun _ ->
    CreateDir interfaceDeployDir
    CreateDir crmConsumerDeployDir
    CreateDir crmListenerDeployDir
    CreateDir crmPublisherDeployDir
    CreateDir thirdPartyConsumerDeployDir 
    CreateDir thirdPartyPublisherDeployDir
    CreateDir demoCrmPublisherDeployDir
    CreateDir domainDeployDir
    CreateDir workflowActivityDeployDir
    
    !! (crmConsumerBuildDir @@ @"*.*")
        |> CopyTo crmConsumerDeployDir

    !! (crmListenerBuildDir @@ @"*.*")
        |> CopyTo crmListenerDeployDir

    !! (crmPublisherBuildDir @@ @"*.*")
        |> CopyTo crmPublisherDeployDir

    !! (thirdPartyConsumerBuildDir @@ @"*.*")
        |> CopyTo thirdPartyConsumerDeployDir

    !! (thirdPartyPublisherBuildDir @@ @"*.*")
        |> CopyTo thirdPartyPublisherDeployDir

    !! (demoCrmPublisherBuildDir @@ @"*.*")
        |> CopyTo demoCrmPublisherDeployDir

    !! (domainBuildDir @@ @"*.*")
        |> CopyTo domainDeployDir

    !! (workflowActivityBuildDir @@ @"*.*")
        |> CopyTo workflowActivityDeployDir
)

Target "CreateNuget" (fun _ ->
    Pack (fun p ->
            {p with
                Version = nugetVersion
                
            })
)

// Dependencies
"Clean"
  ==> "BuildVersions"
  =?> ("AssemblyInfo", not isLocalBuild )
  ==> "BuildDomain"
  ==> "BuildCrmConsumer"
  ==> "BuildCrmListener"
  ==> "BuildCrmPublisher"
  ==> "BuildThirdPartyConsumer"
  ==> "BuildThirdPartyPublisher"
  ==> "BuildDemoCrmPublisher"
  ==> "BuildWorkflowActivity"
  ==> "BuildTest"
  ==> "Publish"
  ==> "CreateNuget"

// start build
RunTargetOrDefault "Publish"
