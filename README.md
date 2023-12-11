﻿<h1 align="center">CosmosHTTP Client 🚀</h1>
<p>
  <a href="https://www.nuget.org/packages/CosmosHttpClient/" target="_blank">
    <img alt="Version" src="https://img.shields.io/nuget/v/CosmosHttpClient.svg" />
  </a>
  <a href="https://github.com/CosmosOS/CosmosHttp/blob/main/LICENSE.txt" target="_blank">
    <img alt="License: BSD Clause 3 License" src="https://img.shields.io/badge/license-BSD License-yellow.svg" />
  </a>
</p>

> CosmosHTTP is a HTTP client made in C# for the Cosmos operating system construction kit.

## Usage

Install the Nuget Package from [Nuget](https://www.nuget.org/packages/CosmosHttpClient/) or [Github](https://github.com/CosmosOS/CosmosHttpClient/packages/):

```PM
Install-Package CosmosHttpClient -Version 1.0.0
```

```PM
dotnet add PROJECT package CosmosHttpClient --version 1.0.0
```

Or add these lines to your Cosmos kernel .csproj:

```
<ItemGroup>
    <PackageReference Include="CosmosHttpClient" Version="1.0.0" NoWarn="NU1604;NU1605" />
</ItemGroup>
```

You can find more information about the HTTP client and how to connect to a remote server in the [Cosmos Documentation](https://cosmosos.github.io/articles/Kernel/Network.html#http).

## Authors

👤 **[@valentinbreiz](https://github.com/valentinbreiz)**

## 🤝 Contributing

Contributions, issues and feature requests are welcome!<br />Feel free to check [issues page](https://github.com/CosmosOS/CosmosHttp/issues). 

## Show your support

Give a ⭐️ if this project helped you!

## 📝 License

Copyright © 2022 [CosmosOS](https://github.com/CosmosOS).<br />
This project is [BSD Clause 3](https://github.com/CosmosOS/CosmosHttp/blob/main/LICENSE.txt) licensed.
