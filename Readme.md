# Project Layout

There are 3 parts, the library, a terminal application and a WASM application.

## MiniCalc.Lib

This is the core library and contains the calculator logic, e.g.: parsing and evaluating expressions.
This is done via tokenization of a math expression and converting to Reverse Polish Notation (RPN)
and then evaluating the RPN expression.

## MiniCalc.Term

MiniCalc.Term is a very simple terminal application that uses the MiniCalc.Lib to evaluate expressions.
This can be used to test the library as well as use it as a simple calculator in the terminal instead of MiniCalc.Web,
which can only be used via a web browser.

## MiniCalc.Web

A very simple Blazor WASM application that provides a very simplistic web interface to the MiniCalc.Lib.
UI is not my passion nor my strong suit, that's why it has been kept very simple and uses the default Blazor templates.

# Building the Project

The projects can be build using the dotnet CLI, e.g.:

```sh
dotnet build .
```

Alternatively one can built the docker container for the WASM application:

```sh
docker build -t minicalc-web -f MiniCalc.Web.Dockerfile .
docker run -d -p 8080:80 --name minicalc-web minicalc-web
```

## Artefacts

Build artefacts such as the terminal application can be found in the actions artefacts section.
Furthermore the WASM application can be found as a docker image in the container registry.
e.g.: *ghcr.io/gaschenk/MiniCalc:latest*
