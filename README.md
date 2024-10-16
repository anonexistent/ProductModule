# <div id="main">EfiritPro Retail ProductModule - Модуль товаров</div>

## <div id="content">Содержание</div>

- [ProductModule](#main)
    - [Содержание](#content)
    - [Предназначение](#target)
    - [Установка](#install)
    - [Использование](#usage)

## <div id="target">Предназначение</div>

Модуль ответственен за список товаров в организации 

## <div id="install">Установка</div>

### Предварительные требования

- docker версии ^24.0.0
- efirit-dotnet-workspace:0.14 из проекта EfiritPro Retail Packages

### Процесс установки

1. Создать образ модуля

```bash
docker build -t efirit-product-module:0.23 .
```

2. Применить этот образ в проекте EfiritPro Retail Backend

## <div id="usage">Использование</div>

### Внешнее API

apiHost - http адрес API сервера

- [Product API](usageManuals/externalAPI/product_api.md)
- [ProductType API](usageManuals/externalAPI/product_type_api.md)
- [Unit API](usageManuals/externalAPI/unit_api.md)
- [VAT API](usageManuals/externalAPI/vat_api.md)
- [MarkingType API](usageManuals/externalAPI/marking_type_api.md)

### RabbitMQ

Прослушиваемые очереди и их объекты (см. Miro)

- product/eventAck => RabbitEvent
- product/removeOrganization => OrganizationEvent
