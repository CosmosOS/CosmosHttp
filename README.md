﻿<h1 align="center">CosmosHTTP Client [WIP]</h1>
<p>
  <a href="https://www.nuget.org/packages/CosmosHttp/" target="_blank">
    <img alt="Version" src="https://img.shields.io/nuget/v/CosmosHttp.svg" />
  </a>
  <a href="https://github.com/CosmosOS/CosmosHttp/blob/main/LICENSE.txt" target="_blank">
    <img alt="License: BSD Clause 3 License" src="https://img.shields.io/badge/license-BSD License-yellow.svg" />
  </a>
</p>

> CosmosHTTP is a HTTP client made in C# for the Cosmos operating system construction kit. GET and PUT are currently supported.

### Todo
See [this issue](https://github.com/CosmosOS/CosmosHttp/issues/1) for todo list.

## Usage

### Installation

Install the Nuget Package from [Nuget](https://www.nuget.org/packages/CosmosHttp/):

```PM
Install-Package CosmosHttp -Version 1.0.2
```

```PM
dotnet add PROJECT package CosmosHttp --version 1.0.2
```

Or add these lines to your Cosmos kernel .csproj:

```
<ItemGroup>
    <PackageReference Include="CosmosHttp" Version="1.0.2" />
</ItemGroup>
```

### Examples

```CS
using CosmosHttp.Client;

HttpRequest request = new();
request.IP = "34.223.124.45";
request.Path = "/";
request.Method = "GET";
request.Send();
Console.WriteLine(request.Response.Content);
```

Here is a basic wget command implementation using CosmosHttp: [github.com/aura-systems/Aura-Operating-System](https://github.com/aura-systems/Aura-Operating-System/blob/master/SRC/Aura_OS/System/Interpreter/Commands/Network/Wget.cs#L63).

## Authors

👤 **[@valentinbreiz](https://github.com/valentinbreiz)**

## 🤝 Contributing

Contributions, issues and feature requests are welcome!<br />Feel free to check [issues page](https://github.com/CosmosOS/CosmosHttp/issues). 

## Show your support

Give a ⭐️ if this project helped you!

## 📝 License

Copyright © 2022 [CosmosOS](https://github.com/CosmosOS).<br />
This project is [BSD Clause 3](https://github.com/CosmosOS/CosmosHttp/blob/main/LICENSE.txt) licensed.
