# <div id="main"> EfiritPro Retail ProductModule MarkingTypeAPI</div>

## <div id="content">Содержание</div>

- [MarkingTypeAPI](#main)
  - [Содержание](#content)
  - [Использование](#usage)
    - [Получение списка типов маркировок](#usage-get-list)
    - [Получение информации о типе маркировки](#usage-get)

## <div id="usage">Использование</div>

### <div id="usage-get-list">Получение списка типов маркировок</div>

#### API Endpoint
HttpGet [apiHost]/product/markingType/getMarkingTypeList

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/markingType/getMarkingTypeList (для работников)

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
    "markingTypes": [
        {
            "id": string,
            "name": string,
        }, ...
    ]
}
```

### <div id="usage-get">Получение информации о типе маркировки</div>

#### API Endpoint
HttpGet [apiHost]/product/unit/getUnitById

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/markingType/getMarkingTypeById (для работников)

####  Request

```
Query
{
    "clientId": string,      | обязательное
    "markingTypeId": string, | обязательное
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
}
```