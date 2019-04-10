barber
===

Barber is simple wrapper for mustache template engine. 


## Installation 

Barber is installed as a dotnet global tool.

```shell
dotnet tool install -g barber
```

## Usage

```shell
barber mustache -i template.mustache -o index.html -- -Name="Homer"
```

**Template File**

```html
<html>
<body>
    <h1>{{ Name }}</h1>
</body>
</html>
```

**Result File**
```html
<html>
<body>
    <h1>Homer</h1>
</body>
</html>
```