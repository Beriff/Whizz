{
  pkgs ? import <nixpkgs> { },
}:

pkgs.mkShell {
  buildInputs = [
    pkgs.dotnetCorePackages.sdk_10_0-bin
    pkgs.omnisharp-roslyn

    pkgs.SDL2
    pkgs.SDL2_image
    pkgs.SDL2_mixer
    pkgs.SDL2_ttf
  ];

  DOTNET_NOLOGO = "1";
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1";

  shellHook = ''
    export DOTNET_CLI_HOME="$PWD/.dotnet"
    export NUGET_PACKAGES="$PWD/.nuget/packages"

    export LD_LIBRARY_PATH="${pkgs.SDL2}/lib:${pkgs.SDL2_image}/lib:${pkgs.SDL2_mixer}/lib:${pkgs.SDL2_ttf}/lib:$LD_LIBRARY_PATH"

    mkdir -p "$DOTNET_CLI_HOME" "$NUGET_PACKAGES"
  '';
}
