# <div id="main">EfiritPro Retail ProductModule ProductApi</div>

## <div id="content">Содержание</div>

- [ProductApi](#main)
    - [Содержание](#content)
    - [Использование](#usage)
        - [Получение списка продуктов](#usage-get-list)
        - [Получение информации о продукте](#usage-get)
        - [Создание продукта](#usage-create)
        - [Изменение продукта](#usage-change)
        - [Удаление продукта](#usage-remove)

## <div id="usage">Использование</div>

### <div id="usage-get-list">Получения списка продуктов</div>

#### API Endpoint
HttpGet [apiHost]/product/product/getProductList

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- Совпадают organizationId в токене и запросе (для работников)
- В токене есть разрешение product/product/getProductList (для работников)

####  Request

```
Query
{
    "clientId": string,       | обязательное
    "organizationId": string, | обязательное
}
```

####  Response 401 (Пользователь без токена)
####  Response 403 (Пользователь не прошёл ограничения)
####  Response 200

```
Body
Content-Type: "application/json"

{
    "products": [
        {
            "id": string,
            "name": string,
            "vendorCode": string,
            "barCode": string | null,
            "description": string | null,
            "purchasePrice": number, // Цена покупки
            "sellingPrice": number, // Цена продажи
            "promoPrice": number, // Цена продажи по акции
            "excise": boolean, // Акциз
            "hidden": boolean,
            "vat": {
                "id": string,
                "name": string,
                "percent": number,
            },
            "productType": {
                "id": string,
                "name": string,
            },
            "unit": {
                "id": string,
                "name": string,
                "code": number,
            },
            "markingType: {
                "id": string,
                "name": string,
            } | null,
        }, ...
    ]
}
```

### <div id="usage-get">Получение информации о продукте</div>

#### API Endpoint
HttpGet [apiHost]/product/product/getProductById

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- Совпадают organizationId в токене и запросе (для работников)
- В токене есть разрешение product/product/getProductById (для работников)

####  Request

```
Query
{
    "clientId": string,       | обязательное
    "organizationId": string, | обязательное
    "productId": string,      | обязательное
}
```

####  Response 401 (Пользователь без токена)
####  Response 403 (Пользователь не прошёл ограничения)
####  Response 404 (Товар не найден)
####  Response 200

```
Body
Content-Type: "application/json"

{
    "id": string,
    "name": string,
    "vendorCode": string,
    "barCode": string | null,
    "description": string | null,
    "purchasePrice": number, // Цена покупки
    "sellingPrice": number, // Цена продажи
    "promoPrice": number, // Цена продажи по акции
    "excise": boolean, // Акциз
    "hidden": boolean,
    "vat": {
        "id": string,
        "name": string,
        "percent": number,
    },
    "productType": {
        "id": string,
        "name": string,
    },
    "unit": {
        "id": string,
        "name": string,
        "code": number,
    },
    "markingType: {
        "id": string,
        "name": string,
    } | null,
}
```

### <div id="usage-create">Создание продукта</div>

HttpPost [apiHost]/product/product/createProduct

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- Совпадают organizationId в токене и запросе (для работников)
- В токене есть разрешение product/product/createProduct (для работников)

####  Request

```
Query
{
    "clientId": string,       | обязательное
    "organizationId": string, | обязательное
}

Body
Content-Type: "application/json"

{
    "name": string,                  | обязательное
    "vendorCode": string,            | обязательное
    "barCode": string | null,
    "description": string | null,
    "purchasePrice": number,         | обязательное (Цена покупки)
    "sellingPrice": number,          | обязательное (Цена продажи)
    "promoPrice": number,            | обязательное (Цена продажи по акции)
    "excise": boolean,               | обязательное (Акциз)

    "vatId": string,                 | обязательное
    "productTypeId": string,         | обязательное
    "unitId": string,                | обязательное
    "markingTypeId": string | null,
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
    "vendorCode": string,
    "barCode": string | null,
    "description": string | null,
    "purchasePrice": number, // Цена покупки
    "sellingPrice": number, // Цена продажи
    "promoPrice": number, // Цена продажи по акции
    "excise": boolean, // Акциз
    "hidden": boolean,
    "vat": {
        "id": string,
        "name": string,
        "percent": number,
    },
    "productType": {
        "id": string,
        "name": string,
    },
    "unit": {
        "id": string,
        "name": string,
        "code": number,
    },
    "markingType: {
        "id": string,
        "name": string,
    } | null,
}
```

### <div id="usage-change">Изменение продукта</div>

HttpPatch [apiHost]/product/product/updateProduct

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- Совпадают organizationId в токене и запросе (для работников)
- В токене есть разрешение product/product/updateProduct (для работников)

####  Request

```
Query
{
    "clientId": string,       | обязательное
    "organizationId": string, | обязательное
    "productId": string,      | обязательное
}

Body
Content-Type: "application/json"

{
    "name": string,                  | обязательное
    "vendorCode": string,            | обязательное
    "barCode": string | null,
    "description": string | null,
    "purchasePrice": number,         | обязательное (Цена покупки)
    "sellingPrice": number,          | обязательное (Цена продажи)
    "promoPrice": number,            | обязательное (Цена продажи по акции)
    "excise": boolean,               | обязательное (Акциз)

    "vatId": string,                 | обязательное
    "productTypeId": string,         | обязательное
    "unitId": string,                | обязательное
    "markingTypeId": string | null,
}
```

####  Response 401 (Пользователь без токена)
####  Response 403 (Пользователь не прошёл ограничения)
####  Response 400 (Изменение не удалось)
####  Response 200

```
Body
Content-Type: "application/json"

{
    "id": string,
    "name": string,
    "vendorCode": string,
    "barCode": string | null,
    "description": string | null,
    "purchasePrice": number, // Цена покупки
    "sellingPrice": number, // Цена продажи
    "promoPrice": number, // Цена продажи по акции
    "excise": boolean, // Акциз
    "hidden": boolean,
    "vat": {
        "id": string,
        "name": string,
        "percent": number,
    },
    "productType": {
        "id": string,
        "name": string,
    },
    "unit": {
        "id": string,
        "name": string,
        "code": number,
    },
    "markingType: {
        "id": string,
        "name": string,
    } | null,
}
```

### <div id="usage-remove">Удаление продукта</div>

HttpDelete [apiHost]/product/product/removeProduct

####  Ограничения

- В Headers есть поле Authorization с токеном "Bearer [token]"
- Совпадают clientId в токене и запросе
- Совпадают organizationId в токене и запросе (для работников)
- В токене есть разрешение product/product/removeProduct (для работников)

####  Request

```
Query
{
    "clientId": string,       | обязательное
    "organizationId": string, | обязательное
    "productId": string,      | обязательное
}
```

####  Response 401 (Пользователь без токена)
####  Response 403 (Пользователь не прошёл ограничения)
####  Response 200


```
Body
Content-Type: "application/json"

{
    "products": [
        {
            "id": string,
            "name": string,
            "vendorCode": string,
            "barCode": string | null,
            "description": string | null,
            "purchasePrice": number, // Цена покупки
            "sellingPrice": number, // Цена продажи
            "promoPrice": number, // Цена продажи по акции
            "excise": boolean, // Акциз
            "hidden": boolean,
            "vat": {
                "id": string,
                "name": string,
                "percent": number,
            },
            "productType": {
                "id": string,
                "name": string,
            },
            "unit": {
                "id": string,
                "name": string,
                "code": number,
            },
            "markingType: {
                "id": string,
                "name": string,
            } | null,
        }, ...
    ]
}
```
