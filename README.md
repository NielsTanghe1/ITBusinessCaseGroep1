# IT Business Case - Group 1

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Contributors](https://img.shields.io/github/contributors/NielsTanghe1/ITBusinessCaseGroep1)](https://github.com/NielsTanghe1/ITBusinessCaseGroep1/graphs/contributors)
[![Last Commit](https://img.shields.io/github/last-commit/NielsTanghe1/ITBusinessCaseGroep1)](https://github.com/NielsTanghe1/ITBusinessCaseGroep1/commits/main)

**Group 1** repository for the subject **IT Business Case**.

This repository contains an end-to-end integration project created as part of the IT Business Case course. The objective is to design and implement an application that communicates with **Salesforce** using **RabbitMQ** as a message broker.

**Goal**
Sales representatives can quickly enter an order using a web application. That order data is then asynchronously sent to Salesforce through RabbitMQ.

![Route visual][1]

---

## Getting Started

### Prerequisites

Before getting started, ensure your environment meets the following requirements:

- **Programming Language:** C#
- **Framework:** .NET
- **Package Manager:** NuGet
- **Message Broker:** RabbitMQ

### Installation

**Build from source:**

1. Clone the repository:
```bash
git clone https://github.com/NielsTanghe1/ITBusinessCaseGroep1
```

2. Navigate to the project directory:
```bash
cd ITBusinessCaseGroep1
```

3. Install the project dependencies:
**Using `NuGet`**&nbsp;[<img align="center" src="https://img.shields.io/badge/C%23-239120.svg?&logo=c-sharp&logoColor=white" />](https://docs.microsoft.com/en-us/dotnet/csharp/)
```sh
dotnet restore
```

**Build from ...:**

###  Usage
Run the application using the following command:
**Using `NuGet`**&nbsp;[<img align="center" src="https://img.shields.io/badge/C%23-239120.svg?&logo=c-sharp&logoColor=white" />](https://docs.microsoft.com/en-us/dotnet/csharp/)

```bash
dotnet run
```

Depending on the configuration, ensure RabbitMQ and Salesforce credentials are available before starting the application.

---

## Sources

### Helpful resources

* Salesforce Developer Documentation
* RabbitMQ Documentation
* Microsoft .NET Documentation
* [Inject JavaScript to open a URL](https://stackoverflow.com/a/79306770)
* [Install SQL Server on Ubuntu](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-ubuntu?view=sql-server-ver17&tabs=ubuntu2004%2C2505ubuntu2404%2Codbc-ubuntu-2404)
* ["Optional" inheriting method](https://stackoverflow.com/a/38841122)

### Interesting

* [general map method](https://stackoverflow.com/a/76894702)

### Used NuGet packages

- [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client/7.2.0?_src=template)

---

## Contributors

<a href="https://github.com/NielsTanghe1/ITBusinessCaseGroep1/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=NielsTanghe1/ITBusinessCaseGroep1" />
</a>

Made with [contrib.rocks](https://contrib.rocks).

---

[1]: Web/Resources/Images/route_visual_1.jpg "Route visual"
