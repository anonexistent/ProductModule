# <div id="main"> EfiritPro Retail ProductModule UnitAPI</div>

## <div id="content">Содержание</div>

- [UnitAPI](#main)
  - [Содержание](#content)
  - [Использование](#usage)
    - [Получение списка единиц измерения](#usage-get-list)
    - [Получение информации о единице измерения](#usage-get)

## <div id="usage">Использование</div>

### <div id="usage-get-list">Получение списка единиц измерения</div>

#### API Endpoint
HttpGet [apiHost]/product/unit/getUnitList

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/unit/getUnitList (для работников)

####  Request

```
Query
{
    "clientId": string,       | обязательное
}
```

####  Response 401 (Пользователь без токена)
####  Response 403 (Пользователь не прошёл ограничения)
####  Response 200

```
Body
Content-Type: "application/json"

{
    "units": [
        {
            "id": string,
            "name": string,
            "code": number,
        }, ...
    ]
}
```

### <div id="usage-get">Получение информации о единице измерения</div>

#### API Endpoint
HttpGet [apiHost]/product/unit/getUnitById

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/unit/getUnitById (для работников)

####  Request

```
Query
{
    "clientId": string,     | обязательное
    "unitId": string,       | обязательное
}
```

####  Response 401 (Пользователь без токена)
####  Response 403 (Пользователь не прошёл ограничения)
####  Response 200

```
Body
Content-Type: "application/json"

{
    "id": string,
    "name": string,
    "code": number,
}
```