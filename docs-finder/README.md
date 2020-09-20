# Поисковик документов по вложениям в диалогах ВК
Консольное приложение, помогающее найти необходимый документ среди всех ваших диалогов вконтакте.

## Сборка
Для сборки необходимо наличие .net core 3.1

```shell script
git clone https://github.com/MihailITPlace/docs-finder
cd docs-finder/docs-finder
dotnet restore
```
Возможно, что потребуется выполнить миграцию (но это неточно).
```shell script
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```
Для linux:
```shell script
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true
```
Для windows:
```shell script
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true
```

## Принцип работы
Приложение умеет индексировать названия документов с ссылками на них в локальную sqlite базу данных.
Поиск производится с помощью LIKE запроса по названию документа. 

### Авторизация
Для автоматической авторизации необходимо создать подобный _.env_ в одном каталоге с исполняемым файлом.
```dotenv
LOGIN=login@login.com
PASS=password
```

### Обновление и запросы
Если вы хотите перед поиском обновить локальную бд, то необходимо запустить приложение с флагом _-i_.
Запрос можно указать через атрибут _-q_, либо ввести в интерактивном режиме.

```shell script
$./docs-finder -i -q .pdf
```