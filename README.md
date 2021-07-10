# UsefulExtensions
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

```
ICaptchaSolver rucaptchaSolver = new RucapthcaSolver("ваш апи ключ на сервисе"); 
ICaptchaSolver anticapthcaSolver = new AntiCaptchaSolver("ваш апи ключ на сервисе");
```
или
```
CaptchaSolverType rucaptchaType = CaptchaSolverType.Rucaptcha;
ICaptchaSolver rucaptchaSolver = rucaptchaType.GetCaptchaSolverByType("ваш апи ключ на сервисе");

CaptchaSolverType anticapthcaSolver = CaptchaSolverType.AntiCaptcha;
ICaptchaSolver anticapthcaSolver = anticapthcaSolver.GetCaptchaSolverByType("ваш апи ключ на сервисе");
```
Второй способ удобно использовать при работе с визуальным интерфейсом (в WPF/WinForms элемент ComboBox)

Теперь непосредственно решим капчу:
```
ICaptchaSolver rucaptchaSolver = new RucapthcaSolver("ваш апи ключ на сервисе"); 
string gRecaptchaResponse = rucaptchaSolver.SolveRecaptchaV2("6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-", "https://www.google.com/recaptcha/api2/demo", false);
```
Подробное описание по параметрам методов можно посмотреть в подсказках к ним. Также это хорошо описано в [документации к api рукапчи](https://rucaptcha.com/api-rucaptcha)
### Генератор случайных User-Agent
Сгенерировать случайный User-Agent от браузеров Chrome, Firefox, Opera, основываясь на их популярности:
```
string randomUserAgent = RandomUserAgentGenerator.GenerateRandomUserAgent();
```
Сгенерировать случайный User-Agent от браузеров Chrome, Firefox, Opera соответственно:
```
string randomChromeUserAgent = RandomUserAgentGenerator.GenerateChromeUserAgent();
string randomOperaUserAgent = RandomUserAgentGenerator.GenerateOperaUserAgent();
string randomFirefoxUserAgent = RandomUserAgentGenerator.GenerateFirefoxUserAgent();
```
### Генератор случайных строк
Для начала создадим новый экземляр класса `RandomStringGenerator`. При необходимости, переопределим словарь или воспользуемся уже готовыми:
```
var randomStringGenerator = new RandomStringGenerator(); // в этом строка будет генерироваться из символов латинского алфавита нижнего регистра и цифр
var randomStringGeneratorWithYourDictionary = new RandomStringGenerator("abcdef"); // в этом случае строка будет генерироваться из символов a, b, c, d, e, f
var numbersGenerator = RandomStringGenerator.NumbersGenerator; // в этом случае строка будет генерироваться только из цифр
```
Теперь сгенерируем случайную строку, указав необходимую длину:
```
string randomString = randomStringGenerator.Generate(10); // будет сгенерирована строка длиной в 10 символов
```
