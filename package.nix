{ lib, buildDotnetModule, dotnetCorePackages }:

buildDotnetModule rec {
  pname = "direct-dd";
  version = "1.0";

  src = ./.;

  projectFile = "./direct-dd.sln";
  dotnet-sdk = dotnetCorePackages.sdk_9_0;
  dotnet-runtime = dotnetCorePackages.sdk_9_0;
  dotnetFlags = [ "" ];
  executables = [ "direct-dd" ];
  runtimeDeps = [ "" ];
}