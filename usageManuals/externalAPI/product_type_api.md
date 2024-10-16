# <div id="main"> EfiritPro Retail ProductModule ProductTypeAPI</div>

## <div id="content">Содержание</div>

- [ProductTypeAPI](#main)
  - [Содержание](#content)
  - [Использование](#usage)
    - [Получение списка типов продукта](#usage-get-list)
    - [Получение информации о типе продукта](#usage-get)

## <div id="usage">Использование</div>

### <div id="usage-get-list">Получение списка типов продукта</div>

#### API Endpoint
HttpGet [apiHost]/product/productType/getProductTypeList

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/productType/getProductTypeList (для работников)

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
    "productTypes": [
        {
            "id": string,
            "name": string,
        }, ...
    ]
}
```

### <div id="usage-get">Получение информации о типе продукта</div>

#### API Endpoint
HttpGet [apiHost]/product/productType/getProductTypeById

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- В токене есть разрешение product/productType/getProductTypeById (для работников)

####  Request

```
Query
{
    "clientId": string,       | обязательное
    "productTypeId": string,  | обязательное
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