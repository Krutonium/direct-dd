{ pkgs ? import <nixpkgs> {} }:

pkgs.stdenv.mkDerivation {
  pname = "direct-dd";
  version = "1.0.0";

  src = ./.;

  nativeBuildInputs = [ pkgs.dotnet-sdk_9 ];

  buildPhase = ''
    dotnet publish -c Release -o out
  '';

  installPhase = ''
    mkdir -p $out/bin
    cp out/direct-dd $out/bin/
  '';

  meta = {
    description = "Raw disk writer, similar to dd, but utilizing URL's instead of input files.";
    license = pkgs.lib.licenses.mit;
    platforms = pkgs.lib.platforms.linux;
  };
}
