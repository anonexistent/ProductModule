# <div id="main"> EfiritPro Retail ProductModule VAT API</div>

## <div id="content">Содержание</div>

- [VAT API](#main)
  - [Содержание](#content)
  - [Использование](#usage)
    - [Получение списка НДС](#usage-get-list)
    - [Получение информации о НДС](#usage-get)

## <div id="usage">Использование</div>

### <div id="usage-get-list">Получение списка НДС</div>

#### API Endpoint
HttpGet [apiHost]/product/vat/getVATList

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/vat/getVATList (для работников)

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
    "vats": [
        {
            "id": string,
            "name": string,
            "percent": number,
        }, ...
    ]
}
```

### <div id="usage-get">Получение информации о НДС</div>

#### API Endpoint
HttpGet [apiHost]/product/vat/getVATById

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/vat/getVATById (для работников)

####  Request

```
Query
{
    "clientId": string,    | обязательное
    "vatId": string,       | обязательное
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
    "percent": number,
}
```