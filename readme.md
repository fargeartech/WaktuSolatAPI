# Introduction

This is a simple API that 
request to check solat time in Malaysia time zone. Data retrieve from JAKIM api and can be use to get the whole year solat.
## JAKIM zone referrer

The zone can be refer to this page [JAKIM E-SOLAT](https://www.e-solat.gov.my/index.php?siteId=24&pageId=24).

## Usage

```csharp

# returns 'by zone and whole year'
url/api/solattime/SGR01

# returns 'zone and specific date'
url/api/solattime/SGR01/01-01-2023 or 01-jan-2023

```
## Date Filter
make sure the date format only need to be parse either 01-01-2023 or 01-JAN-2023

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

## License
❤️ by Faris