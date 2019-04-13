barber
===

<!-- TOC -->

- [Installation & Usage](#installation--usage)
- [Commands](#commands)
    - [Render](#render)
        - [Arguments](#arguments)
        - [JSON](#json)
    - [OpenAPI](#openapi)
        - [Create Configuration](#create-configuration)
        - [Process](#process)

<!-- /TOC -->

Barber is cli-tool for generating files based on mustache templates. In addition it can read and process OpenAPI 3 specifications and generate code based on templates.


# Installation & Usage

Barber is installed as a dotnet global tool.

```shell
dotnet tool install -g barber

# Show help
barber --help

# Render
barber render -i template.mustache -- -Var1=A212

# OpenAPI create configuration
barber openapi init

# OpenAPI process
barber openapi
```

# Commands

## Render

You can use this command for processing any mustache template. Either you pass the variables per arguments or by passing in a json file.

### Arguments

```shell
barber render -i template.mustache -o index.html -- -Var1="Foo"
```

### JSON

```shell
barber render -i template.mustache -o index.html -j data.json"
```

```json
{
    "Var1": "Foo"
}
```

**Template File**

```html
<html>
<body>
    <h1>{{ Var1 }}</h1>
</body>
</html>
```

**Result File**
```html
<html>
<body>
    <h1>Foo</h1>
</body>
</html>
```

## OpenAPI

### Create Configuration

Create initial configuration file.

```shell
barber openapi init"
```

### Process

Read OpenAPI specification file and generate code based on templates

```shell
barber openapi init"
```