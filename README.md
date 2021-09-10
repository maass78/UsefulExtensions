# UsefulExtensions
[![NuGet version](https://badge.fury.io/nu/maass78.UsefulExtensions.svg)](https://badge.fury.io/nu/maass78.UsefulExtensions)
[![Nuget installs](https://img.shields.io/nuget/dt/maass78.UsefulExtensions.svg)](https://www.nuget.org/packages/maass78.UsefulExtensions/) 
 
Библиотека, позволяющая очень просто выполнять действия, часто необходимые в создании чекеров/регеров и прочего софта для автоматизации каких-либо процессов на сайтах. 
## Возможности
- Решение капчи через сервисы [RuCaptcha](https://rucaptcha.com) и [AntiCaptcha](https://anti-captcha.com)
- Интерфейс взаимодействия с сервисами смс-активации ([smshub](https://smshub.org/), [sms-activate](https://sms-activate.ru/), [5sim](https://5sim.net/), [vak-sms](https://vak-sms.com/))
- Парс аккаунтов из строки, из файла (формат `login:password` или `login;password`)
- Парс проксей из строки, из файла, по ссылке на список прокси (формат `ip:port:login:password`)
- Генератор случайных строк с возможностью переопределения словаря
- Генератор случайных [User-Agent](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent) от декстоп браузеров Chrome, Firefox, Opera
- Перечисление списка объектов по порядку для многопотока
## Примеры
### Капча
Предположим, мы решаем капчу на сайте https://www.google.com/recaptcha/api2/demo. Вид капчи на сайте - google reCaptcha V2.
Для начала определяем решатель капчи. Сделать это можно двумя способами:

```csharp
ICaptchaSolver rucaptchaSolver = new RucapthcaSolver("ваш апи ключ на сервисе"); 
ICaptchaSolver anticapthcaSolver = new AntiCaptchaSolver("ваш апи ключ на сервисе");
```
или
```csharp
CaptchaSolverType rucaptchaType = CaptchaSolverType.Rucaptcha;
ICaptchaSolver rucaptchaSolver = rucaptchaType.GetCaptchaSolverByType("ваш апи ключ на сервисе");

CaptchaSolverType anticapthcaType = CaptchaSolverType.AntiCaptcha;
ICaptchaSolver anticapthcaSolver = anticapthcaSolver.GetCaptchaSolverByType("ваш апи ключ на сервисе");
```
Второй способ удобно использовать при работе с визуальным интерфейсом (в WPF/WinForms элемент ComboBox)

Теперь непосредственно решим капчу:
```csharp
ICaptchaSolver rucaptchaSolver = new RucapthcaSolver("ваш апи ключ на сервисе"); 
string gRecaptchaResponse = rucaptchaSolver.SolveRecaptchaV2("6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-", "https://www.google.com/recaptcha/api2/demo", false);
```
Подробное описание по параметрам методов можно посмотреть в подсказках к ним. Также это хорошо описано в [документации к api рукапчи](https://rucaptcha.com/api-rucaptcha)
### Сервисы смс-активации
Определяем один из сервисов:
```csharp
ISmsActivator smsHubActivator = new SmsHubActivator("ваш апи ключ на сервисе");
ISmsActivator sim5activator = new Sim5Activator("ваш апи ключ на сервисе");
ISmsActivator smsActivateRuActivator = new SmsActivateRuActivator("ваш апи ключ на сервисе");
ISmsActivator vakSmsComActivator = new VakSmsComActivator("ваш апи ключ на сервисе");
```
или
```csharp
SmsActivatorType activatorType = SmsActivatorType.SmsHub;
ISmsActivator smsHubActivator = activatorType.GetSmsActivatorByType("ваш апи ключ на сервисе");
//с остальными сервисами делаем по аналогии
```
Второй способ, так же как и с решением капчи, удобно использовать при работе с визуальным интерфейсом.  

Берем номер в аренду и ждем смс:
```csharp
var number = smsHubActivator.GetNumber("ot");

Console.WriteLine(number.PhoneNumber); // или любая другая логика взаимодействия с полученным номером

var status = smsHubActivator.GetStatus(number.Id);
while(status.StatusEnum == StatusEnum.StatusWaitCode)
{
	status = smsHubActivator.GetStatus(number.Id);
	Thread.Sleep(5000);
}

Console.WriteLine(status.SmsCode); // или любая другая логика взаимодействия с полученным кодом
```

### Генератор случайных User-Agent
Сгенерировать случайный User-Agent от браузеров Chrome, Firefox, Opera, основываясь на их популярности:
```csharp
string randomUserAgent = RandomUserAgentGenerator.GenerateRandomUserAgent();
```
Сгенерировать случайный User-Agent от браузеров Chrome, Firefox, Opera соответственно:
```csharp
string randomChromeUserAgent = RandomUserAgentGenerator.GenerateChromeUserAgent();
string randomOperaUserAgent = RandomUserAgentGenerator.GenerateOperaUserAgent();
string randomFirefoxUserAgent = RandomUserAgentGenerator.GenerateFirefoxUserAgent();
```
### Генератор случайных строк
Для начала создадим новый экземляр класса `RandomStringGenerator`. При необходимости, переопределим словарь или воспользуемся уже готовыми:
```csharp
var randomStringGenerator = new RandomStringGenerator(); // в этом строка будет генерироваться из символов латинского алфавита нижнего регистра и цифр
var randomStringGeneratorWithYourDictionary = new RandomStringGenerator("abcdef"); // в этом случае строка будет генерироваться из символов a, b, c, d, e, f
var numbersGenerator = RandomStringGenerator.NumbersGenerator; // в этом случае строка будет генерироваться только из цифр
```
Теперь сгенерируем случайную строку, указав необходимую длину:
```csharp
string randomString = randomStringGenerator.Generate(10); // будет сгенерирована строка длиной в 10 символов
```

