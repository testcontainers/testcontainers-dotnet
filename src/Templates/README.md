# .NET module template

The .NET module template scaffolds Testcontainers for .NET modules. While simple modules can inherit from `ContainerBuilder`, more advanced modules usually require additional properties and methods to set up a running configuration. To scaffold a new module, install the template `dotnet new --install ./src/Templates` and run `dotnet new tcm --name ${module_name} --output ${output_directory}`.
