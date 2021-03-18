# Maki

### What is Maki?

#### Maki is the software that runs inside a Docker container to interact with the UCI chess engine. It exposes a few APIs over an HTTP connection, in addition to a websocket interface for subscribing to a realtime stream of the engine output. At the time of writing, the supported APIs are:

* GetEngineId
* ListSupportedOption
* SetEngineDebugFlag
* StartEvaluation
* SubscribeToEngineStream
* UnsubscribeToEngineStream

The most up-to-date list can most likely be obtained by looking at [IMakiClient source](blob/master/packages/projects/Maki.Client/Maki.Client/IMakiClient.cs).

## Components

### Maki Server

The Maki server is the target project for this monorepo - i.e., the final result of building this entire repository is the docker container containing the Maki server software.

### Maki API Model

The API model is package shared by the Maki server and the Maki client.

### Maki Client

The client is used in two circumstances - by the Janus request router component, and by the Maki server tests.

### Maki Embedded

The embedded project references the Maki server and allows the use of the server outside of Docker. By simply referencing this library, unit/integration tests can run an entire instance of the Maki engine wrapper locally.