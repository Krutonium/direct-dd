{
  description = "direct-dd: write raw disk images from URL using O_DIRECT";

  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  inputs.flake-utils.url = "github:numtide/flake-utils";

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs {
          inherit system;
          config.allowUnfree = true;
        };
      in {
        packages.default = pkgs.callPackage ./default.nix {};
        apps.default = {
          type = "app";
          program = "${self.packages.${system}.default}/bin/direct-dd";
        };
      });
}
